const egyptBounds = L.latLngBounds(
  L.latLng(21.5, 24.5), // South-West
  L.latLng(32.0, 37.0)  // North-East
);

const map = L.map("map", { 
  zoomControl: true,
  maxBounds: egyptBounds,
  maxBoundsViscosity: 1.0,
  minZoom: 6
}).setView([30.0444, 31.2357], 12);

L.tileLayer("https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png", {
  maxZoom: 19,
  minZoom: 6,
  attribution: '&copy; <a href="https://carto.com/">CartoDB</a>'
}).addTo(map);

const fromInput = document.getElementById("fromInput");
const toInput = document.getElementById("toInput");
const findRouteBtn = document.getElementById("findRouteBtn");
const statusText = document.getElementById("statusText");
const resultCards = document.getElementById("resultCards");
const stationOptions = document.getElementById("stationOptions");

let fromMarker;
let toMarker;
let routeLine;
let stationMarkers = [];
let allStations = [];
const API_BASE = window.location.origin.includes("localhost:5000") ? "" : "http://localhost:5000";
const DEFAULT_COORDS = [30.0444, 31.2357];

function setStatus(message) {
  statusText.textContent = message;
}

async function loadStationOptions() {
  try {
    const response = await fetch(`${API_BASE}/api/bus/routes/stations`);
    if (!response.ok) {
      throw new Error("Could not load stations.");
    }

    allStations = await response.json();
  } catch (error) {
    console.error(error);
  }
}

function getDistance(lat1, lon1, lat2, lon2) {
  const R = 6371; // km
  const dLat = (lat2 - lat1) * Math.PI / 180;
  const dLon = (lon2 - lon1) * Math.PI / 180;
  const a = Math.sin(dLat/2) * Math.sin(dLat/2) +
            Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
            Math.sin(dLon/2) * Math.sin(dLon/2);
  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
  return R * c;
}

function isUsableCoord(coords) {
  if (!Array.isArray(coords) || coords.length !== 2) return false;
  const [lat, lon] = coords;
  if (!Number.isFinite(lat) || !Number.isFinite(lon)) return false;
  if (lat < 21.5 || lat > 32 || lon < 24.5 || lon > 37) return false;
  return Math.abs(lat - DEFAULT_COORDS[0]) > 0.0001 || Math.abs(lon - DEFAULT_COORDS[1]) > 0.0001;
}

function stationCoords(station) {
  if (!station) return null;
  const coords = [Number(station.latitude), Number(station.longitude)];
  return isUsableCoord(coords) ? coords : null;
}

function normalizeName(value) {
  return value
    .trim()
    .toLowerCase()
    .replace(/[أإآ]/g, "ا")
    .replace(/ة/g, "ه")
    .replace(/ى/g, "ي")
    .replace(/ؤ/g, "و")
    .replace(/ئ/g, "ي")
    .replace(/(^|\s)ال/g, "$1")
    .replace(/\s+/g, " ");
}

function findLocalStation(query) {
  const normalizedQuery = normalizeName(query);
  if (!normalizedQuery) return null;

  return allStations.find((station) => normalizeName(station.name) === normalizedQuery)
    || allStations.find((station) => normalizeName(station.name).includes(normalizedQuery))
    || allStations.find((station) => normalizedQuery.includes(normalizeName(station.name)));
}

function findNearestStation(coords) {
  let nearest = null;
  let nearestDistance = Infinity;

  allStations.forEach((station) => {
    const coordsForStation = stationCoords(station);
    if (!coordsForStation) return;

    const distanceKm = getDistance(coords[0], coords[1], coordsForStation[0], coordsForStation[1]);
    if (distanceKm < nearestDistance) {
      nearest = station;
      nearestDistance = distanceKm;
    }
  });

  return nearest ? { station: nearest, distanceKm: nearestDistance } : null;
}

const locationHelperText = document.getElementById("locationHelperText");

async function useMyLocation() {
  if (!navigator.geolocation) {
    if (locationHelperText) locationHelperText.textContent = "Geolocation is not supported by your browser.";
    return;
  }
  
  if (locationHelperText) locationHelperText.textContent = "Locating you...";
  
  navigator.geolocation.getCurrentPosition(
    async (position) => {
      const userLat = position.coords.latitude;
      const userLon = position.coords.longitude;
      
      try {
        const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${userLat}&lon=${userLon}&accept-language=ar`);
        const data = await res.json();
        
        // Extract area/neighborhood
        const areaName = data.address.suburb || data.address.neighbourhood || data.address.city_district || data.address.town || data.address.city || data.display_name.split(',')[0];
        
        if (locationHelperText) {
            locationHelperText.innerHTML = `.أنت بالقرب من: <strong>${areaName}</strong>. يمكنك كتابتها في خانة البحث.`;
        }
        
        map.setView([userLat, userLon], 15);
        
        // Clear previous location markers
        if (window.userLocationMarker) map.removeLayer(window.userLocationMarker);
        
        window.userLocationMarker = L.marker([userLat, userLon], {
            icon: L.divIcon({
                className: 'user-location-marker',
                html: '<div class="user-dot"></div>',
                iconSize: [20, 20]
            })
        }).addTo(map).bindPopup(`موقعك الحالي: ${areaName}`).openPopup();
        
      } catch (err) {
        if (locationHelperText) locationHelperText.textContent = `Coordinates: ${userLat.toFixed(5)}, ${userLon.toFixed(5)}`;
      }
    },
    (err) => {
      if (locationHelperText) locationHelperText.textContent = "Unable to retrieve your location.";
    },
    { enableHighAccuracy: true }
  );
}

async function geocodePlace(query) {
  const trimmed = query.trim();
  if (!trimmed) {
    throw new Error("Missing location text.");
  }

  const localStation = findLocalStation(trimmed);
  const localCoords = stationCoords(localStation);
  if (localCoords) {
    return localCoords;
  }

  const endpoint = `https://nominatim.openstreetmap.org/search?format=json&limit=1&countrycodes=eg&viewbox=24.5,32,37,21.5&bounded=1&q=${encodeURIComponent(`${trimmed}, Egypt`)}`;
  const response = await fetch(endpoint, { headers: { "Accept": "application/json" } });
  if (!response.ok) {
    throw new Error("Geocoding service not available.");
  }

  const places = await response.json();
  if (!places.length) {
    throw new Error("No places found.");
  }

  const coords = [parseFloat(places[0].lat), parseFloat(places[0].lon)];
  if (!isUsableCoord(coords)) {
    throw new Error("The found place is outside the supported area.");
  }

  return coords;
}

async function useMyLocation() {
  if (!navigator.geolocation) {
    if (locationHelperText) locationHelperText.textContent = "Geolocation is not supported by your browser.";
    return;
  }

  if (locationHelperText) locationHelperText.textContent = "Locating you...";

  navigator.geolocation.getCurrentPosition(
    async (position) => {
      const userCoords = [position.coords.latitude, position.coords.longitude];

      try {
        if (!allStations.length) {
          await loadStationOptions();
        }

        const nearest = findNearestStation(userCoords);
        if (!nearest) {
          throw new Error("No stations with valid coordinates are loaded.");
        }

        fromInput.value = nearest.station.name;
        if (locationHelperText) {
          locationHelperText.innerHTML = `أنت بالقرب من: <strong>${nearest.station.name}</strong>. يمكنك كتابتها في خانة البحث.`;
        }

        map.setView(userCoords, 15);

        if (window.userLocationMarker) map.removeLayer(window.userLocationMarker);

        window.userLocationMarker = L.marker(userCoords, {
          icon: L.divIcon({
            className: "user-location-marker",
            html: '<div class="user-dot"></div>',
            iconSize: [20, 20]
          })
        }).addTo(map).bindPopup("Your current location").openPopup();
      } catch (err) {
        if (locationHelperText) {
          locationHelperText.textContent = `Your coordinates: ${userCoords[0].toFixed(5)}, ${userCoords[1].toFixed(5)}. No nearby station with valid coordinates was found.`;
        }
      }
    },
    () => {
      if (locationHelperText) locationHelperText.textContent = "Unable to retrieve your location.";
    },
    { enableHighAccuracy: true, timeout: 15000, maximumAge: 30000 }
  );
}

function clearMapLayers() {
  if (fromMarker) map.removeLayer(fromMarker);
  if (toMarker) map.removeLayer(toMarker);
  if (routeLine) map.removeLayer(routeLine);
  stationMarkers.forEach((marker) => map.removeLayer(marker));
  stationMarkers = [];
}

function drawRouteOnMap(fromCoords, toCoords, fromLabel, toLabel) {
  clearMapLayers();

  fromMarker = L.marker(fromCoords).addTo(map).bindPopup(`From: ${fromLabel}`).openPopup();
  toMarker = L.marker(toCoords).addTo(map).bindPopup(`To: ${toLabel}`);

  routeLine = L.polyline([fromCoords, toCoords], {
    color: "#5ac8ff",
    weight: 5,
    opacity: 0.8,
    lineJoin: "round"
  }).addTo(map);

  map.fitBounds(routeLine.getBounds(), { padding: [40, 40] });
}

const stationCoordsCache = {};

async function drawStationRouteOnMap(stations) {
  if (!stations.length) return;

  setStatus("Drawing route on map...");
  clearMapLayers();

  const mappedStations = [];
  
  for (const station of stations) {
    let coords = null;
    // If DB has real coords (not the dummy 30.0444), use them
    if (station.latitude !== 30.0444 || station.longitude !== 31.2357) {
      coords = [station.latitude, station.longitude];
    } else {
      try {
        coords = await geocodePlace(station.name + ", Cairo, Egypt");
      } catch (err) {
        continue;
      }
    }
    
    if (coords && isUsableCoord(coords)) {
        mappedStations.push({ station, coords });
    }
  }

  if (mappedStations.length < 2) {
    throw new Error("This route does not have enough valid station coordinates yet.");
  }

  const points = mappedStations.map(item => item.coords);

  // Transit Path
  routeLine = L.polyline(points, {
    color: "#3b82f6",
    weight: 6,
    opacity: 0.8,
    lineJoin: "round"
  }).addTo(map);

  const glowLine = L.polyline(points, {
    color: "#60a5fa",
    weight: 12,
    opacity: 0.2,
    lineJoin: "round"
  }).addTo(map);

  mappedStations.forEach(({ station, coords }, idx) => {
    const isEdge = idx === 0 || idx === mappedStations.length - 1;
    
    const marker = L.circleMarker(coords, {
      radius: isEdge ? 7 : 4,
      color: "#ffffff",
      weight: 2,
      fillColor: isEdge ? "#2563eb" : "#3b82f6",
      fillOpacity: 1
    }).addTo(map);

    marker.bindTooltip(station.name, {
      permanent: true,
      direction: 'right',
      offset: [10, 0],
      className: 'premium-station-label'
    });

    if (isEdge) {
      const edgeMarker = L.marker(coords, {
        icon: L.divIcon({
          className: 'edge-dot-outer',
          html: '<div class="edge-dot-inner"></div>',
          iconSize: [20, 20]
        })
      }).addTo(map);
      stationMarkers.push(edgeMarker);
    }
    stationMarkers.push(marker);
  });
  
  stationMarkers.push(glowLine);
  map.fitBounds(routeLine.getBounds(), { padding: [60, 60] });
  
  const skipped = stations.length - mappedStations.length;
  if (skipped > 0) {
      setStatus(`Route mapped. ${skipped} station(s) couldn't be located on map.`);
  } else {
      setStatus("Route mapped with precise coordinates.");
  }
}

async function getBusRoute(from, to) {
  const response = await fetch(`${API_BASE}/api/bus/routes/search`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({ from, to })
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || "No bus route found.");
  }

  const routes = await response.json();

  return routes.map(route => ({
    busNumber: route.busNumber,
    waitStation: route.waitStation,
    destinationStation: route.destinationStation,
    stations: route.stations || []
  }));
}

function renderRouteResults(routes) {
  resultCards.innerHTML = "";
  
  resultCards.style.display = "flex";
  resultCards.style.flexDirection = "column";
  resultCards.style.gap = "1rem";

  const countCard = document.createElement("div");
  countCard.className = "result-card is-visible";
  countCard.style.transform = "none";
  countCard.style.opacity = "1";
  countCard.innerHTML = `<h3 style="margin: 0; color: #7ca5ff; font-size: 1rem;">${routes.length} matching bus route${routes.length === 1 ? "" : "s"}</h3>`;
  resultCards.appendChild(countCard);

  routes.forEach((route, rIndex) => {
    const wrapper = document.createElement("div");
    wrapper.className = "result-card is-visible";
    wrapper.style.display = "flex";
    wrapper.style.flexDirection = "column";
    wrapper.style.gap = "0.8rem";
    wrapper.style.animationDelay = `${0.08 + rIndex * 0.1}s`;
    wrapper.style.transform = "none";
    wrapper.style.opacity = "1";
    
    const header = document.createElement("div");
    header.style.display = "flex";
    header.style.justifyContent = "space-between";
    header.style.alignItems = "center";
    header.style.borderBottom = "1px solid rgba(255,255,255,0.1)";
    header.style.paddingBottom = "0.5rem";

    const titleInfo = document.createElement("div");
    titleInfo.innerHTML = `<h3 style="margin: 0; color: #7ca5ff; font-size: 1.1rem;">Option ${rIndex + 1} - Bus ${route.busNumber}</h3>`;
    
    const priceInfo = document.createElement("div");
    priceInfo.innerHTML = `<strong style="font-size: 1.1rem; color: #49d2ff;">20 EGP</strong>`;

    header.appendChild(titleInfo);
    header.appendChild(priceInfo);
    wrapper.appendChild(header);

    const details = document.createElement("div");
    details.style.display = "flex";
    details.style.flexWrap = "wrap";
    details.style.gap = "1.5rem";
    details.innerHTML = `
      <div style="flex: 1; min-width: 200px;">
        <span style="color: #9eb0dd; font-size: 0.85rem;">Wait At</span>
        <p style="margin: 0.2rem 0 0; font-weight: 700;">${route.waitStation}</p>
      </div>
      <div style="flex: 1; min-width: 200px;">
        <span style="color: #9eb0dd; font-size: 0.85rem;">Destination Station</span>
        <p style="margin: 0.2rem 0 0; font-weight: 700;">${route.destinationStation}</p>
      </div>
    `;
    wrapper.appendChild(details);

    if (route.stations.length) {
      const stationsList = document.createElement("div");
      stationsList.style.marginTop = "0.5rem";
      stationsList.innerHTML = `
        <span style="color: #9eb0dd; font-size: 0.85rem;">Stations On Your Way</span>
        <div style="margin-top: 0.4rem; color: var(--text-main); font-size: 0.9rem; line-height: 1.5;">
          ${route.stations.map(s => s.name).join(" <span style='color:#7ca5ff; margin:0 4px;'>➔</span> ")}
        </div>
      `;
      wrapper.appendChild(stationsList);
    }
    
    resultCards.appendChild(wrapper);
  });
}

async function handleFindRoute() {
  const fromText = fromInput.value.trim();
  const toText = toInput.value.trim();

  if (!fromText || !toText) {
    setStatus("Please enter both FROM and TO locations.");
    return;
  }

  setStatus("Searching for route and stations...");
  findRouteBtn.disabled = true;
  findRouteBtn.textContent = "Finding...";

  try {
    const routesData = await getBusRoute(fromText.toLowerCase(), toText.toLowerCase());
    
    if (routesData.length && routesData[0].stations.length) {
      await drawStationRouteOnMap(routesData[0].stations);
    } else {
      const [fromCoords, toCoords] = await Promise.all([
        geocodePlace(fromText),
        geocodePlace(toText)
      ]);
      drawRouteOnMap(fromCoords, toCoords, fromText, toText);
    }
    renderRouteResults(routesData);

    setStatus("Route found. Check your suggested bus details below.");
  } catch (error) {
    setStatus("Unable to find route right now. Please try different places.");
    console.error(error);
  } finally {
    findRouteBtn.disabled = false;
    findRouteBtn.textContent = "Find Route";
  }
}

findRouteBtn.addEventListener("click", handleFindRoute);

const useLocationBtn = document.getElementById("useLocationBtn");
if (useLocationBtn) {
  useLocationBtn.addEventListener("click", useMyLocation);
}

function setupAutocomplete(inputEl, dropdownEl) {
  if (!inputEl || !dropdownEl) return;
  
  inputEl.addEventListener("input", () => {
    const val = inputEl.value.toLowerCase();
    dropdownEl.innerHTML = "";
    
    let matches = allStations;
    if (val) {
      matches = allStations.filter(s => s.name.toLowerCase().includes(val));
    }
    
    if (matches.length === 0) {
      dropdownEl.classList.remove("active");
      return;
    }
    
    matches = [...matches].sort((a, b) => a.name.localeCompare(b.name));
    
    matches.slice(0, 80).forEach(match => {
      const div = document.createElement("div");
      div.className = "autocomplete-item";
      div.textContent = match.name;
      div.addEventListener("click", () => {
        inputEl.value = match.name;
        dropdownEl.classList.remove("active");
      });
      dropdownEl.appendChild(div);
    });
    
    dropdownEl.classList.add("active");
  });

  inputEl.addEventListener("focus", () => {
    inputEl.dispatchEvent(new Event('input'));
  });

  document.addEventListener("click", (e) => {
    if (e.target !== inputEl && e.target !== dropdownEl && !dropdownEl.contains(e.target)) {
      dropdownEl.classList.remove("active");
    }
  });
}

const fromDropdown = document.getElementById("fromDropdown");
const toDropdown = document.getElementById("toDropdown");
setupAutocomplete(fromInput, fromDropdown);
setupAutocomplete(toInput, toDropdown);

[fromInput, toInput].forEach((input) => {
  if (input) {
    input.addEventListener("keydown", (event) => {
      if (event.key === "Enter") {
        handleFindRoute();
      }
    });
  }
});

loadStationOptions();
