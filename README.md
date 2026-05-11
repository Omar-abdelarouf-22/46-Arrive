# 📍 ARRIVE
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

ARRIVE is a smart mobility web application that helps users find bus routes, detect the nearest metro station, and compare ride-hailing trip costs on an interactive map.

---

## ✨ Features
- 🚌 **Bus route finder** with `FROM`/`TO` station search via `POST /api/bus/routes/search`
- 📍 **Use My Location** for bus: detects your location and suggests the nearest known bus station
- 🔎 **Station autocomplete** for bus inputs from backend-provided station data
- 🗺️ **Interactive map visualization** for bus trips and station paths using Leaflet
- 🚇 **Nearest metro station finder** using typed location or browser geolocation
- 📏 **Distance calculation** to nearest metro station (Haversine-based)
- 🚗 **Smart mobility cost comparison UI** (Uber / DiDi / inDrive, car + moto)
- 🧮 **Ride estimation API** via `POST /api/ride-estimations/calculate`
- 🚉 **Metro route + fare API** via `GET /api/metrostations/route?fromId=&toId=`
- 🧱 **Bus data maintenance endpoints** for reseeding and coordinate synchronization
- 🌐 **Single-host setup** where ASP.NET Core serves both API controllers and static frontend pages

## 📸 Screenshots
- <!-- Add screenshot here -->
- <!-- Add screenshot here -->
- <!-- Add screenshot here -->

## 🏗️ Architecture
ARRIVE is organized into:
- `Arrive.BusApi`: the active ASP.NET Core API + static web frontend (`wwwroot`)
- `ARRIVE`: a legacy static frontend prototype
- `presentation-workspace`: a minimal Node ESM workspace under `Arrive.BusApi`

Real project tree:

```text
.
├── ARRIVE/
│   ├── index.html
│   ├── bus.html
│   ├── metro.html
│   ├── cost.html
│   ├── script.js
│   ├── bus.js
│   ├── metro.js
│   ├── cost.js
│   ├── style.css
│   ├── bus.css
│   ├── metro.css
│   └── cost.css
└── Arrive.BusApi/
    ├── Arrive.BusApi.csproj
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── route_stop_coordinates.csv
    ├── station_coordinates.csv
    ├── Controllers/
    │   ├── BusRoutesController.cs
    │   ├── MetroStationsController.cs
    │   └── RideEstimationsController.cs
    ├── Data/
    │   └── AppDbContext.cs
    ├── Dtos/
    │   ├── BusRouteDtos.cs
    │   └── RideEstimationDtos.cs
    ├── Models/
    │   ├── Bus.cs
    │   ├── BusRoute.cs
    │   ├── MetroStation.cs
    │   ├── RouteStation.cs
    │   └── Station.cs
    ├── Migrations/
    │   ├── 20260506093111_CleanSetup.cs
    │   ├── 20260506093111_CleanSetup.Designer.cs
    │   └── AppDbContextModelSnapshot.cs
    ├── Properties/
    │   └── launchSettings.json
    ├── wwwroot/
    │   ├── index.html
    │   ├── bus.html
    │   ├── metro.html
    │   ├── cost.html
    │   ├── script.js
    │   ├── bus.js
    │   ├── metro.js
    │   ├── cost.js
    │   ├── style.css
    │   ├── bus.css
    │   ├── metro.css
    │   └── cost.css
    └── presentation-workspace/
        ├── package.json
        └── src/
            ├── build-arrive-deck.mjs
            └── smoke.mjs
```

## 📋 Prerequisites
Based on project files:

- **.NET SDK 9.0** (`TargetFramework: net9.0`)
- **SQL Server** instance accessible by the configured connection string
- **Entity Framework Core packages** referenced in `Arrive.BusApi.csproj`:
  - `Microsoft.EntityFrameworkCore.SqlServer` (`9.*`)
  - `Microsoft.EntityFrameworkCore.Design` (`9.*`)
  - `Microsoft.EntityFrameworkCore.Tools` (`10.0.7`)
- **ASP.NET Core OpenAPI package**: `Microsoft.AspNetCore.OpenApi` (`9.0.15`)
- **Node.js** (optional, only for `presentation-workspace`; no dependencies/scripts declared)
- **Modern browser** with geolocation support (for location-based features)

## 🚀 Quick Start
### 1. Clone the Repository
```bash
git clone <your-repo-url>
cd "Graduation Project ( Arrive )/Arrive.BusApi"
```

### 2. Install Dependencies
This project uses .NET NuGet restore:

```bash
dotnet restore
```

Optional (presentation workspace only):

```bash
cd presentation-workspace
npm install
cd ..
```

### 3. Environment Variables Setup
No `.env` or `.env.example` files are present in the repository.  
Configuration is provided through `appsettings*.json` and standard ASP.NET Core environment variables.

Environment/config values used by the app:

- `ASPNETCORE_ENVIRONMENT`  
  Controls environment mode (`Development` in `launchSettings.json`), which enables development-only OpenAPI endpoint mapping.

- `ConnectionStrings__DefaultConnection`  
  Environment-variable override for `ConnectionStrings:DefaultConnection`.  
  Used by EF Core in `Program.cs` to connect to SQL Server.

- `Logging__LogLevel__Default`  
  Optional override for default logging level.

- `Logging__LogLevel__Microsoft__AspNetCore`  
  Optional override for ASP.NET Core framework logging level.

- `AllowedHosts`  
  Host filtering setting (`*` by default in `appsettings.json`).

### 4. Run the Application
From `Arrive.BusApi`:

```bash
dotnet run
```

Default dev URLs (from `launchSettings.json`):
- `http://localhost:5108`
- `https://localhost:7245`

## 🔧 Configuration
- `Arrive.BusApi/appsettings.json`  
  Main runtime config: SQL Server connection string, logging, allowed hosts.
- `Arrive.BusApi/appsettings.Development.json`  
  Development-specific config overrides.
- `Arrive.BusApi/Properties/launchSettings.json`  
  Local launch profiles, app URLs, and environment variables.
- `Arrive.BusApi/Arrive.BusApi.csproj`  
  Build target (`net9.0`) and NuGet package references.

## 📖 Usage
- **Home navigation (`index.html`)**
  1. Open ARRIVE home page.
  2. Choose Bus, Metro, or Cost module.

- **Bus route search**
  1. Open `bus.html`.
  2. Enter `FROM` and `TO` stations (or select from autocomplete).
  3. Click **Find Route**.
  4. Review route options and mapped station path.
  5. Optional: click **Use My Location** to auto-fill nearest station.

- **Nearest metro station**
  1. Open `metro.html`.
  2. Enter a location or click **Use My Location**.
  3. Click **Find Nearest Station**.
  4. View nearest station, distance, and map highlight.

- **Smart mobility cost comparison**
  1. Open `cost.html`.
  2. Enter `FROM` and `TO`.
  3. Click **Calculate Cost**.
  4. View estimated distance and pricing cards (Uber/DiDi/inDrive).

- **API endpoints (for integration/testing)**
  - `GET /api/busroutes/stations` is **not** used; actual path is `GET /api/bus/routes/stations`
  - `POST /api/bus/routes/search`
  - `GET /api/metrostations`
  - `GET /api/metrostations/route?fromId={id}&toId={id}`
  - `POST /api/ride-estimations/calculate`
  - `POST /api/bus/routes/sync-coordinates`
  - `POST /api/bus/routes/seed-new-data`

## 🛠️ Development
- **Backend**
  - C# + ASP.NET Core Web API (`net9.0`)
  - Entity Framework Core with SQL Server
- **Frontend**
  - Vanilla HTML/CSS/JavaScript
  - Leaflet for interactive maps
  - External geocoding through OpenStreetMap Nominatim API
- **Project structure notes**
  - Active app lives in `Arrive.BusApi` (`wwwroot` + API controllers).
  - `ARRIVE` directory is a standalone/legacy static variant.
  - `presentation-workspace` is separate and minimal (Node ESM, no scripts declared).

## 🐛 Troubleshooting
- **Database connection fails**
  - Verify SQL Server is running and reachable.
  - Update `ConnectionStrings:DefaultConnection` (or `ConnectionStrings__DefaultConnection`) as needed.

- **Frontend cannot call API**
  - Run the app on the configured dev port (`5108`) or update frontend API base assumptions.
  - Confirm `dotnet run` is started from `Arrive.BusApi`.

- **Geolocation does not work**
  - Allow browser location permissions.
  - Use HTTPS/localhost contexts where browser geolocation is permitted.

- **Geocoding returns limited/no results**
  - Nominatim can rate-limit or fail temporarily; retry and use more specific place names.

- **Route map incomplete for bus stations**
  - Some stations may not have validated coordinates; use coordinate sync/seed endpoints and CSV data.

## 📦 Deployment
The active deployment unit is `Arrive.BusApi` (API + static frontend served by ASP.NET Core).

Build and publish:

```bash
cd Arrive.BusApi
dotnet restore
dotnet publish -c Release -o ./publish
```

Deployment checklist based on current code:
- Configure production connection string (`ConnectionStrings__DefaultConnection` recommended).
- Set `ASPNETCORE_ENVIRONMENT=Production`.
- Host published output on a .NET-capable server/runtime.
- Ensure outbound internet access for Nominatim geocoding calls used by frontend modules.
- Restrict or protect admin-like endpoints (`seed-new-data`, `sync-coordinates`) before public exposure.

## 📄 License
MIT License

