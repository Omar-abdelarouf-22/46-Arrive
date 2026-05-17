const fromInput = document.getElementById("fromInput");
const toInput = document.getElementById("toInput");
const calculateBtn = document.getElementById("calculateBtn");
const statusText = document.getElementById("statusText");
const distanceText = document.getElementById("distanceText");
const loadingState = document.getElementById("loadingState");
const resultCards = document.getElementById("resultCards");

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

// Backend API wrapper
async function calculateTrip(from, to) {
  const [fromCoords, toCoords] = await Promise.all([geocodePlace(from), geocodePlace(to)]);
  
  // Call the new C# Backend Pricing Engine
  const response = await fetch("http://localhost:5000/api/ride-estimations/calculate", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      FromLat: fromCoords[0],
      FromLng: fromCoords[1],
      ToLat: toCoords[0],
      ToLng: toCoords[1]
    })
  });

  if (!response.ok) {
    throw new Error("Could not calculate pricing. Is backend server running?");
  }

  const result = await response.json();

  // The backend returns keys matching the models: uber, diDi, inDrive
  // We map them to lowercase for the frontend renderCards function
  const pricing = {
    uber: {
      car: result.pricing.uber.car,
      moto: result.pricing.uber.moto
    },
    didi: {
      car: result.pricing.diDi.car,
      moto: result.pricing.diDi.moto
    },
    indrive: {
      car: result.pricing.inDrive.car,
      moto: result.pricing.inDrive.moto
    }
  };

  return { fromCoords, toCoords, distanceKm: result.distanceKm, pricing };
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

// === New Features: My Location & Autocomplete ===

const useLocationBtn = document.getElementById("useLocationBtn");
const suggestionsList = document.getElementById("suggestionsList");

// 1. My Location Feature
useLocationBtn.addEventListener("click", () => {
  if (!navigator.geolocation) {
    setStatus("Geolocation is not supported by your browser.");
    return;
  }
  
  setStatus("Locating you...");
  useLocationBtn.disabled = true;

  navigator.geolocation.getCurrentPosition(
    async (position) => {
      const lat = position.coords.latitude;
      const lon = position.coords.longitude;
      
      try {
        // Reverse geocoding to get a readable name (in Arabic)
        const res = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lon}&accept-language=ar`);
        const data = await res.json();
        
        // Put the readable address in the input, or fallback to coordinates
        fromInput.value = data.display_name ? data.display_name.split(',')[0] + ', ' + data.display_name.split(',')[1] : `${lat.toFixed(5)}, ${lon.toFixed(5)}`;
        setStatus("Location found!");
      } catch (err) {
        fromInput.value = `${lat.toFixed(5)}, ${lon.toFixed(5)}`;
        setStatus("Location found!");
      } finally {
        useLocationBtn.disabled = false;
      }
    },
    (error) => {
      setStatus("Unable to retrieve your location.");
      useLocationBtn.disabled = false;
    }
  );
});

// 2. Autocomplete Feature for "TO WHERE?"
let debounceTimeout;

toInput.addEventListener("input", (e) => {
  const query = e.target.value.trim();
  
  clearTimeout(debounceTimeout);
  
  if (query.length < 3) {
    suggestionsList.hidden = true;
    return;
  }

  // Delay the API call so we don't spam the server while typing
  debounceTimeout = setTimeout(async () => {
    try {
      // Add 'viewbox' and 'bounded' parameters to prioritize places in/around Cairo if needed,
      // but a general search works fine. We restrict to Egypt (countrycodes=eg) and Arabic (accept-language=ar).
      const endpoint = `https://nominatim.openstreetmap.org/search?format=json&limit=5&countrycodes=eg&accept-language=ar&q=${encodeURIComponent(query)}`;
      const response = await fetch(endpoint, { headers: { Accept: "application/json" } });
      const places = await response.json();

      suggestionsList.innerHTML = "";
      
      if (places.length > 0) {
        places.forEach(place => {
          const li = document.createElement("li");
          li.textContent = place.display_name;
          li.addEventListener("click", () => {
            toInput.value = place.display_name; // Set full or partial name
            suggestionsList.hidden = true;
          });
          suggestionsList.appendChild(li);
        });
        suggestionsList.hidden = false;
      } else {
        suggestionsList.hidden = true;
      }
    } catch (error) {
      console.error("Autocomplete error:", error);
      suggestionsList.hidden = true;
    }
  }, 500); // 500ms delay
});

// Hide autocomplete when clicking outside
document.addEventListener("click", (e) => {
  if (e.target !== toInput && e.target !== suggestionsList) {
    suggestionsList.hidden = true;
  }
});
