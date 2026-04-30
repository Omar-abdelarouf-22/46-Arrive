const map = L.map("map", { zoomControl: true }).setView([30.0444, 31.2357], 12);

L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
  maxZoom: 19,
  attribution: "&copy; OpenStreetMap contributors"
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

const mockLocations = {
  "cairo": [30.0444, 31.2357],
  "nasr city": [30.0626, 31.3302],
  "maadi": [29.9602, 31.2569],
  "zamalek": [30.0638, 31.2196],
  "heliopolis": [30.0916, 31.3175],
  "dokki": [30.0384, 31.2123],
  "giza": [30.0131, 31.2089]
};

function setStatus(message) {
  statusText.textContent = message;
}

async function loadStationOptions() {
  if (!stationOptions) return;

  try {
    const response = await fetch("/api/bus/routes/stations");
    if (!response.ok) {
      throw new Error("Could not load stations.");
    }

    const stations = await response.json();
    stationOptions.innerHTML = "";

    stations.forEach((station) => {
      const option = document.createElement("option");
      option.value = station.name;
      stationOptions.appendChild(option);
    });
  } catch (error) {
    console.error(error);
  }
}

async function geocodePlace(query) {
  const trimmed = query.trim();
  if (!trimmed) {
    throw new Error("Missing location text.");
  }

  const endpoint = `https://nominatim.openstreetmap.org/search?format=json&limit=1&q=${encodeURIComponent(trimmed)}`;

  try {
    const response = await fetch(endpoint, {
      headers: {
        "Accept": "application/json"
      }
    });

    if (!response.ok) {
      throw new Error("Geocoding service not available.");
    }

    const places = await response.json();
    if (!places.length) {
      throw new Error("No places found.");
    }

    return [parseFloat(places[0].lat), parseFloat(places[0].lon)];
  } catch (_) {
    return fallbackCoordinates(trimmed);
  }
}

function fallbackCoordinates(name) {
  const key = name.toLowerCase();
  if (mockLocations[key]) {
    return mockLocations[key];
  }

  const chars = Array.from(key).reduce((sum, ch) => sum + ch.charCodeAt(0), 0);
  const lat = 30.0 + (chars % 90) / 1000;
  const lon = 31.2 + (chars % 140) / 1000;
  return [lat, lon];
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

function drawStationRouteOnMap(stations) {
  if (!stations.length) return;

  clearMapLayers();

  const points = stations.map((station) => [station.latitude, station.longitude]);
  const firstStation = stations[0];
  const lastStation = stations[stations.length - 1];

  fromMarker = L.marker(points[0]).addTo(map).bindPopup(`From: ${firstStation.name}`).openPopup();
  toMarker = L.marker(points[points.length - 1]).addTo(map).bindPopup(`To: ${lastStation.name}`);

  stationMarkers = stations.slice(1, -1).map((station) =>
    L.circleMarker([station.latitude, station.longitude], {
      radius: 6,
      color: "#7ca5ff",
      weight: 2,
      fillColor: "#49d2ff",
      fillOpacity: 0.85
    }).addTo(map).bindPopup(`${station.order}. ${station.name}`)
  );

  routeLine = L.polyline(points, {
    color: "#5ac8ff",
    weight: 5,
    opacity: 0.82,
    lineJoin: "round"
  }).addTo(map);

  map.fitBounds(routeLine.getBounds(), { padding: [40, 40] });
}

async function getBusRoute(from, to) {
  const response = await fetch("/api/bus/routes/search", {
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

  const route = await response.json();

  return {
    busNumber: route.busNumber,
    waitStation: route.waitStation,
    destinationStation: route.destinationStation,
    stations: route.stations || []
  };
}

function renderRouteResult(route) {
  resultCards.innerHTML = "";

  const cards = [
    { title: "Bus Number", value: route.busNumber },
    { title: "Station to Wait At", value: route.waitStation },
    { title: "Destination Station", value: route.destinationStation }
  ];

  cards.forEach((item, index) => {
    const card = document.createElement("article");
    card.className = "result-card";
    card.style.animationDelay = `${0.08 + index * 0.1}s`;
    card.innerHTML = `<h3>${item.title}</h3><p>${item.value}</p>`;
    resultCards.appendChild(card);
  });

  if (route.stations.length) {
    const stationsCard = document.createElement("article");
    stationsCard.className = "result-card stations-card";
    stationsCard.style.animationDelay = "0.42s";
    stationsCard.innerHTML = `
      <h3>Stations On Your Way</h3>
      <ol class="station-list">
        ${route.stations.map((station) => `<li>${station.name}</li>`).join("")}
      </ol>
    `;
    resultCards.appendChild(stationsCard);
  }
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
    const [fromCoords, toCoords] = await Promise.all([
      geocodePlace(fromText),
      geocodePlace(toText)
    ]);

    const routeData = await getBusRoute(fromText.toLowerCase(), toText.toLowerCase());
    if (routeData.stations.length) {
      drawStationRouteOnMap(routeData.stations);
    } else {
      drawRouteOnMap(fromCoords, toCoords, fromText, toText);
    }
    renderRouteResult(routeData);

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

[fromInput, toInput].forEach((input) => {
  input.addEventListener("keydown", (event) => {
    if (event.key === "Enter") {
      handleFindRoute();
    }
  });
});

loadStationOptions();
