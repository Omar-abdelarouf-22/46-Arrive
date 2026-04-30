const locationInput = document.getElementById("locationInput");
const useLocationBtn = document.getElementById("useLocationBtn");
const findNearestBtn = document.getElementById("findNearestBtn");
const statusText = document.getElementById("statusText");
const loadingBar = document.getElementById("loadingBar");
const resultCard = document.getElementById("resultCard");
const stationNameText = document.getElementById("stationName");
const stationDistanceText = document.getElementById("stationDistance");

const map = L.map("map", { zoomControl: true }).setView([30.0444, 31.2357], 11);
L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
  maxZoom: 19,
  attribution: "&copy; OpenStreetMap contributors"
}).addTo(map);

let userMarker = null;
let nearestLine = null;
let nearestStationHighlight = null;
let stationLinkLines = null;
let currentUserCoords = null;

const stationLayer = L.layerGroup().addTo(map);

// Backend-ready API wrapper; replace this with fetch("/api/metro-stations") later.
async function getMetroStations() {
  return [
    { name: "Helwan", lat: 29.8489, lng: 31.3342 },
    { name: "Maadi", lat: 29.9602, lng: 31.2576 },
    { name: "Sadat", lat: 30.0444, lng: 31.2358 },
    { name: "Nasser", lat: 30.0535, lng: 31.2387 },
    { name: "Al Shohadaa", lat: 30.0610, lng: 31.2460 },
    { name: "Attaba", lat: 30.0523, lng: 31.2468 },
    { name: "Cairo University", lat: 30.0260, lng: 31.2013 },
    { name: "Dokki", lat: 30.0385, lng: 31.2121 },
    { name: "Opera", lat: 30.0419, lng: 31.2249 },
    { name: "Abbassia", lat: 30.0735, lng: 31.2831 },
    { name: "Haroun", lat: 30.1015, lng: 31.3331 },
    { name: "El Marg", lat: 30.1521, lng: 31.3384 }
  ];
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
    renderStations(stations);

    let userCoords = currentUserCoords;
    if (!userCoords) {
      userCoords = await geocodeLocation(locationInput.value);
      currentUserCoords = userCoords;
    }

    drawUserMarker(userCoords);
    const { station, distanceKm } = findNearestStation(userCoords, stations);
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
})();
