const locationInput = document.getElementById("locationInput");
const useLocationBtn = document.getElementById("useLocationBtn");
const findNearestBtn = document.getElementById("findNearestBtn");
const statusText = document.getElementById("statusText");
const loadingBar = document.getElementById("loadingBar");
const resultCard = document.getElementById("resultCard");
const stationNameText = document.getElementById("stationName");
const stationDistanceText = document.getElementById("stationDistance");

const egyptBounds = L.latLngBounds(
  L.latLng(21.5, 24.5), // South-West
  L.latLng(32.0, 37.0)  // North-East
);

const map = L.map("map", { 
  zoomControl: true,
  maxBounds: egyptBounds,
  maxBoundsViscosity: 1.0,
  minZoom: 6
}).setView([30.0444, 31.2357], 11);
L.tileLayer("https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png", {
  maxZoom: 19,
  minZoom: 6,
  attribution: '&copy; <a href="https://carto.com/">CartoDB</a>'
}).addTo(map);

let userMarker = null;
let nearestLine = null;
let nearestStationHighlight = null;
let stationLinkLines = null;
let currentUserCoords = null;

const stationLayer = L.layerGroup().addTo(map);

async function getMetroStations() {
  try {
    // Calling the ASP.NET Core Backend Localhost explicitly
    const response = await fetch("http://localhost:5000/api/metrostations");
    if (!response.ok) {
      throw new Error("Could not fetch metro stations");
    }
    return await response.json();
  } catch (error) {
    console.error("Error fetching metro stations:", error);
    return [];
  }
}

const knownPlaces = {
  "tahrir square": [30.0459, 31.2356],
  "maadi": [29.9602, 31.2569],
  "dokki": [30.0384, 31.2123],
  "nasr city": [30.0626, 31.3302],
  "heliopolis": [30.0916, 31.3175],
  "zamalek": [30.0638, 31.2196],
  "giza": [30.0131, 31.2089]
};

function setStatus(message) {
  statusText.textContent = message;
}

function setLoading(isLoading) {
  findNearestBtn.disabled = isLoading;
  useLocationBtn.disabled = isLoading;
  loadingBar.classList.toggle("is-active", isLoading);
}

function haversineDistanceKm(lat1, lon1, lat2, lon2) {
  const toRad = (value) => value * (Math.PI / 180);
  const earthRadiusKm = 6371;
  const dLat = toRad(lat2 - lat1);
  const dLon = toRad(lon2 - lon1);

  const a =
    Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) *
    Math.sin(dLon / 2) * Math.sin(dLon / 2);

  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  return earthRadiusKm * c;
}

async function geocodeLocation(query) {
  const text = query.trim().toLowerCase();
  if (!text) throw new Error("Please enter a location.");

  if (knownPlaces[text]) {
    return knownPlaces[text];
  }

  const endpoint = `https://nominatim.openstreetmap.org/search?format=json&limit=1&q=${encodeURIComponent(query)}`;
  try {
    const response = await fetch(endpoint, { headers: { Accept: "application/json" } });
    if (!response.ok) throw new Error("Geocoding service unavailable.");
    const places = await response.json();
    if (!places.length) throw new Error("Location not found.");
    return [parseFloat(places[0].lat), parseFloat(places[0].lon)];
  } catch (_) {
    // Deterministic fallback for demo mode.
    const seed = Array.from(text).reduce((sum, char) => sum + char.charCodeAt(0), 0);
    return [30.0 + (seed % 90) / 1000, 31.2 + (seed % 120) / 1000];
  }
}

function renderStations(stations) {
  stationLayer.clearLayers();

  stations.forEach((station) => {
    L.circleMarker([station.lat, station.lng], {
      radius: 6,
      color: "#5a95ff",
      weight: 2,
      fillColor: "#79b8ff",
      fillOpacity: 0.85
    })
      .bindPopup(`🚇 ${station.name}`)
      .addTo(stationLayer);
  });

  if (stationLinkLines) {
    map.removeLayer(stationLinkLines);
  }

  stationLinkLines = L.polyline(
    stations.map((station) => [station.lat, station.lng]),
    { color: "#4ec6e0", weight: 2, opacity: 0.32, dashArray: "8, 8" }
  ).addTo(map);
}

function drawUserMarker(coords) {
  if (userMarker) map.removeLayer(userMarker);
  userMarker = L.circleMarker(coords, {
    radius: 8,
    color: "#22a469",
    weight: 2,
    fillColor: "#70f5af",
    fillOpacity: 0.95
  }).addTo(map).bindPopup("📍 Your location");
}

function highlightNearestStation(station, distanceKm, userCoords) {
  if (nearestStationHighlight) map.removeLayer(nearestStationHighlight);
  if (nearestLine) map.removeLayer(nearestLine);

  nearestStationHighlight = L.circleMarker([station.lat, station.lng], {
    radius: 10,
    color: "#e9a31f",
    weight: 3,
    fillColor: "#ffca5f",
    fillOpacity: 0.95
  }).addTo(map).bindPopup(`Nearest: ${station.name}`);

  nearestLine = L.polyline([userCoords, [station.lat, station.lng]], {
    color: "#ffca5f",
    weight: 4,
    opacity: 0.85
  }).addTo(map);

  map.fitBounds(nearestLine.getBounds(), { padding: [50, 50], maxZoom: 14 });

  stationNameText.textContent = `Nearest Station: ${station.name}`;
  stationDistanceText.textContent = `Distance: ${distanceKm.toFixed(2)} km`;
  resultCard.classList.add("is-visible");
}

function findNearestStation(userCoords, stations) {
  let nearest = null;
  let shortestDistance = Number.POSITIVE_INFINITY;

  stations.forEach((station) => {
    const distance = haversineDistanceKm(
      userCoords[0],
      userCoords[1],
      station.lat,
      station.lng
    );
    if (distance < shortestDistance) {
      shortestDistance = distance;
      nearest = station;
    }
  });

  return { station: nearest, distanceKm: shortestDistance };
}

function getBrowserLocation() {
  return new Promise((resolve, reject) => {
    if (!navigator.geolocation) {
      reject(new Error("Geolocation is not supported on this browser."));
      return;
    }

    navigator.geolocation.getCurrentPosition(
      (position) => resolve([position.coords.latitude, position.coords.longitude]),
      () => reject(new Error("Unable to retrieve current location.")),
      { enableHighAccuracy: true, timeout: 8000 }
    );
  });
}

async function useCurrentLocation() {
  setLoading(true);
  setStatus("Detecting your current location...");
  try {
    currentUserCoords = await getBrowserLocation();
    drawUserMarker(currentUserCoords);
    map.setView(currentUserCoords, 13);
    locationInput.value = `${currentUserCoords[0].toFixed(5)}, ${currentUserCoords[1].toFixed(5)}`;
    setStatus("Current location detected. You can now find nearest station.");
  } catch (error) {
    setStatus(error.message);
  } finally {
    setLoading(false);
  }
}

async function handleFindNearestStation() {
  setLoading(true);
  setStatus("Calculating nearest metro station...");

  try {
    const stations = await getMetroStations();
    if (!stations || stations.length === 0) {
      throw new Error("No stations available. Is the backend server running?");
    }
    renderStations(stations);

    let userCoords = currentUserCoords;
    if (!userCoords) {
      userCoords = await geocodeLocation(locationInput.value);
      currentUserCoords = userCoords;
    }

    drawUserMarker(userCoords);
    const { station, distanceKm } = findNearestStation(userCoords, stations);
    if (!station) {
      throw new Error("Could not determine the nearest station.");
    }
    highlightNearestStation(station, distanceKm, userCoords);
    setStatus("Nearest station found.");
  } catch (error) {
    setStatus(error.message || "Could not find nearest station.");
  } finally {
    setLoading(false);
  }
}

useLocationBtn.addEventListener("click", useCurrentLocation);
findNearestBtn.addEventListener("click", handleFindNearestStation);
locationInput.addEventListener("keydown", (event) => {
  if (event.key === "Enter") {
    currentUserCoords = null;
    handleFindNearestStation();
  }
});
locationInput.addEventListener("input", () => {
  currentUserCoords = null;
});

(async function init() {
  const stations = await getMetroStations();
  renderStations(stations);
  setStatus("Enter a location or use your current location.");
  
  // Populate Ticket Calculator Dropdowns
  if (stations && stations.length > 0) {
    const sortedStations = [...stations].sort((a, b) => a.name.localeCompare(b.name));
    
    setupAutocomplete(
      document.getElementById("startStationInput"),
      document.getElementById("startStationId"),
      document.getElementById("startDropdown"),
      sortedStations
    );

    setupAutocomplete(
      document.getElementById("endStationInput"),
      document.getElementById("endStationId"),
      document.getElementById("endDropdown"),
      sortedStations
    );
  }
})();

function setupAutocomplete(inputEl, idEl, dropdownEl, stationsData) {
  if (!inputEl || !dropdownEl) return;
  
  inputEl.addEventListener("input", () => {
    const val = inputEl.value.toLowerCase();
    dropdownEl.innerHTML = "";
    if (val !== inputEl.value) {
      // Don't clear id if we just selected an item
      idEl.value = ""; 
    }
    
    let matches = stationsData;
    if (val) {
      matches = stationsData.filter(s => s.name.toLowerCase().includes(val));
    }
    
    if (matches.length === 0) {
      dropdownEl.classList.remove("active");
      return;
    }
    
    matches.slice(0, 100).forEach(match => {
      const div = document.createElement("div");
      div.className = "autocomplete-item";
      div.textContent = match.name;
      div.addEventListener("click", () => {
        inputEl.value = match.name;
        idEl.value = match.id;
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

// === Ticket Calculator Logic ===
const calculateTicketBtn = document.getElementById("calculateTicketBtn");
const startStationIdEl = document.getElementById("startStationId");
const endStationIdEl = document.getElementById("endStationId");
const ticketResult = document.getElementById("ticketResult");
const ticketStops = document.getElementById("ticketStops");
const ticketPrice = document.getElementById("ticketPrice");

calculateTicketBtn.addEventListener("click", async () => {
  const fromId = startStationIdEl.value;
  const toId = endStationIdEl.value;
  
  if (!fromId || !toId) {
    alert("Please select both Start and End stations.");
    return;
  }
  
  if (fromId === toId) {
    alert("Start and End stations cannot be the same.");
    return;
  }
  
  calculateTicketBtn.disabled = true;
  calculateTicketBtn.textContent = "Calculating...";
  
  try {
    const response = await fetch(`http://localhost:5000/api/metrostations/route?fromId=${fromId}&toId=${toId}`);
    
    if (!response.ok) {
      const errText = await response.text();
      throw new Error(`Server Error (${response.status}): ${errText}`);
    }
    
    const result = await response.json();
    
    // Check if the property names are capitalized (Newtonsoft.Json might do this depending on settings)
    const count = result.stationCount || result.StationCount;
    const price = result.priceEgp || result.PriceEgp;
    const path = result.path || result.Path;
    
    if (count === undefined || price === undefined) {
      throw new Error("Invalid data format received from the server.");
    }
    
    ticketStops.textContent = `Number of stops: ${count} stations`;
    ticketPrice.textContent = `${price} EGP`;
    ticketResult.style.display = "block";
    
    // Force a reflow so the transition works, then set opacity and transform
    ticketResult.offsetHeight;
    ticketResult.style.opacity = "1";
    ticketResult.style.transform = "translateY(0)";
    
    // Highlight the path on the map
    if (stationLinkLines) map.removeLayer(stationLinkLines);
    if (nearestLine) map.removeLayer(nearestLine);
    if (nearestStationHighlight) map.removeLayer(nearestStationHighlight);
    
    if (typeof L.polyline.antPath === 'function') {
      stationLinkLines = L.polyline.antPath(
        path.map((station) => [station.lat || station.Lat, station.lng || station.Lng]),
        {
          delay: 400,
          dashArray: [20, 20],
          weight: 6,
          color: "#000000",
          pulseColor: "#ffca5f",
          paused: false,
          reverse: false,
          opacity: 0.8
        }
      ).addTo(map);
    } else {
      stationLinkLines = L.polyline(
        path.map((station) => [station.lat || station.Lat, station.lng || station.Lng]),
        { color: "#ffca5f", weight: 5, opacity: 0.9 }
      ).addTo(map);
    }
    
    map.fitBounds(stationLinkLines.getBounds(), { padding: [40, 40] });
    
  } catch (error) {
    console.error("Ticket Calculation Error:", error);
    ticketStops.textContent = "Error calculating route";
    ticketPrice.textContent = "-- EGP";
    ticketResult.style.display = "block";
    ticketResult.offsetHeight;
    ticketResult.style.opacity = "1";
    ticketResult.style.transform = "translateY(0)";
    alert("Error: " + error.message);
  } finally {
    calculateTicketBtn.disabled = false;
    calculateTicketBtn.textContent = "Calculate Ticket";
  }
});
