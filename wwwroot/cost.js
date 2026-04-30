const fromInput = document.getElementById("fromInput");
const toInput = document.getElementById("toInput");
const calculateBtn = document.getElementById("calculateBtn");
const statusText = document.getElementById("statusText");
const distanceText = document.getElementById("distanceText");
const loadingState = document.getElementById("loadingState");
const resultCards = document.getElementById("resultCards");

const map = L.map("map", { zoomControl: true }).setView([30.0444, 31.2357], 11);
L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
  maxZoom: 19,
  attribution: "&copy; OpenStreetMap contributors"
}).addTo(map);

let fromMarker = null;
let toMarker = null;
let routeLine = null;

const knownLocations = {
  "tahrir square": [30.0459, 31.2356],
  "zamalek": [30.0638, 31.2196],
  "dokki": [30.0384, 31.2123],
  "maadi": [29.9602, 31.2569],
  "heliopolis": [30.0916, 31.3175],
  "nasr city": [30.0626, 31.3302],
  "giza": [30.0131, 31.2089]
};

const priceModels = {
  uber: { car: 10, moto: 7 },
  didi: { car: 8, moto: 6 },
  indrive: { car: 9, moto: 6.5 }
};

function setStatus(message) {
  statusText.textContent = message;
}

function setLoading(isLoading) {
  calculateBtn.disabled = isLoading;
  loadingState.hidden = !isLoading;
}

function haversineKm(lat1, lon1, lat2, lon2) {
  const toRad = (v) => v * (Math.PI / 180);
  const earthRadius = 6371;
  const dLat = toRad(lat2 - lat1);
  const dLon = toRad(lon2 - lon1);
  const a =
    Math.sin(dLat / 2) ** 2 +
    Math.cos(toRad(lat1)) * Math.cos(toRad(lat2)) * Math.sin(dLon / 2) ** 2;
  return 2 * earthRadius * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
}

async function geocodePlace(place) {
  const key = place.trim().toLowerCase();
  if (!key) throw new Error("Please enter both FROM and TO locations.");
  if (knownLocations[key]) return knownLocations[key];

  const endpoint = `https://nominatim.openstreetmap.org/search?format=json&limit=1&q=${encodeURIComponent(place)}`;
  try {
    const response = await fetch(endpoint, { headers: { Accept: "application/json" } });
    if (!response.ok) throw new Error("Geocoding unavailable.");
    const list = await response.json();
    if (!list.length) throw new Error("Location not found.");
    return [parseFloat(list[0].lat), parseFloat(list[0].lon)];
  } catch (_) {
    // Stable fallback for demo behavior.
    const seed = Array.from(key).reduce((sum, ch) => sum + ch.charCodeAt(0), 0);
    return [30.0 + (seed % 90) / 1000, 31.2 + (seed % 130) / 1000];
  }
}

function drawRoute(fromCoords, toCoords, fromLabel, toLabel) {
  if (fromMarker) map.removeLayer(fromMarker);
  if (toMarker) map.removeLayer(toMarker);
  if (routeLine) map.removeLayer(routeLine);

  fromMarker = L.marker(fromCoords).addTo(map).bindPopup(`From: ${fromLabel}`);
  toMarker = L.marker(toCoords).addTo(map).bindPopup(`To: ${toLabel}`);
  routeLine = L.polyline([fromCoords, toCoords], {
    color: "#72d8ff",
    weight: 5,
    opacity: 0.84
  }).addTo(map);

  map.fitBounds(routeLine.getBounds(), { padding: [45, 45] });
}

// Backend-ready wrapper: replace with API call later.
async function calculateTrip(from, to) {
  const [fromCoords, toCoords] = await Promise.all([geocodePlace(from), geocodePlace(to)]);
  const distanceKm = haversineKm(fromCoords[0], fromCoords[1], toCoords[0], toCoords[1]);

  const pricing = {
    uber: {
      car: distanceKm * priceModels.uber.car,
      moto: distanceKm * priceModels.uber.moto
    },
    didi: {
      car: distanceKm * priceModels.didi.car,
      moto: distanceKm * priceModels.didi.moto
    },
    indrive: {
      car: distanceKm * priceModels.indrive.car,
      moto: distanceKm * priceModels.indrive.moto
    }
  };

  return { fromCoords, toCoords, distanceKm, pricing };
}

function formatEgp(value) {
  return `${value.toFixed(2)} EGP`;
}

function renderCards(pricing) {
  resultCards.innerHTML = "";

  const cards = [
    {
      key: "uber",
      title: "Uber",
      themeClass: "uber-card",
      delay: "0.06s",
      iconCar: "🚘",
      iconMoto: "🏍️"
    },
    {
      key: "didi",
      title: "Didi",
      themeClass: "didi-card",
      delay: "0.2s",
      iconCar: "🚗",
      iconMoto: "🛵"
    },
    {
      key: "indrive",
      title: "inDrive",
      themeClass: "indrive-card",
      delay: "0.34s",
      iconCar: "🚙",
      iconMoto: "🏍️"
    }
  ];

  cards.forEach((cardData) => {
    const companyPrices = pricing[cardData.key];
    const card = document.createElement("article");
    card.className = `company-card ${cardData.themeClass}`;
    card.style.animationDelay = cardData.delay;

    card.innerHTML = `
      <div class="company-head">
        <h3>${cardData.title}</h3>
        <span>Ride Options</span>
      </div>
      <div class="price-line">
        <span>${cardData.iconCar} Car</span>
        <strong>${formatEgp(companyPrices.car)}</strong>
      </div>
      <div class="price-line">
        <span>${cardData.iconMoto} Moto</span>
        <strong>${formatEgp(companyPrices.moto)}</strong>
      </div>
    `;

    resultCards.appendChild(card);

    setTimeout(() => {
      card.classList.add("show");
    }, 40);
  });
}

async function onCalculateCost() {
  const from = fromInput.value.trim();
  const to = toInput.value.trim();

  if (!from || !to) {
    setStatus("Please enter both FROM and TO locations.");
    return;
  }

  setLoading(true);
  setStatus("Calculating trip distance and mobility prices...");
  resultCards.innerHTML = "";

  try {
    const trip = await calculateTrip(from, to);
    drawRoute(trip.fromCoords, trip.toCoords, from, to);
    distanceText.textContent = `Distance: ${trip.distanceKm.toFixed(2)} km`;

    // Give loading state enough time for polished UX.
    await new Promise((resolve) => setTimeout(resolve, 650));
    renderCards(trip.pricing);
    setStatus("Comparison ready. Choose your best option.");
  } catch (error) {
    setStatus(error.message || "Could not calculate trip.");
  } finally {
    setLoading(false);
  }
}

calculateBtn.addEventListener("click", onCalculateCost);
[fromInput, toInput].forEach((input) => {
  input.addEventListener("keydown", (event) => {
    if (event.key === "Enter") onCalculateCost();
  });
});
