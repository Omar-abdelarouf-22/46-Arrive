using Arrive.BusApi.Data;
using Arrive.BusApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arrive.BusApi.Controllers;

[ApiController]
[Route("api/bus/routes")]
public class BusRoutesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("stations")]
    public async Task<ActionResult<IReadOnlyList<StationDto>>> GetStations()
    {
        var stations = await dbContext.Stations
            .AsNoTracking()
            .OrderBy(station => station.Name)
            .Select(station => new StationDto(
                station.Id,
                station.Name,
                station.Latitude,
                station.Longitude,
                0))
            .ToListAsync();

        return Ok(stations);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusRouteDto>>> GetRoutes()
    {
        var routes = await dbContext.BusRoutes
            .AsNoTracking()
            .Include(route => route.Bus)
            .Include(route => route.RouteStations)
                .ThenInclude(routeStation => routeStation.Station)
            .OrderBy(route => route.Id)
            .ToListAsync();

        return Ok(routes.Select(ToRouteDto).ToList());
    }

    [HttpPost("search")]
    public async Task<ActionResult<BusRouteSearchResult>> SearchRoute(BusRouteSearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.From) || string.IsNullOrWhiteSpace(request.To))
        {
            return BadRequest("Please enter both FROM and TO stations.");
        }

        var fromText = Normalize(request.From);
        var toText = Normalize(request.To);

        var routes = await dbContext.BusRoutes
            .AsNoTracking()
            .Include(route => route.Bus)
            .Include(route => route.RouteStations)
                .ThenInclude(routeStation => routeStation.Station)
            .ToListAsync();

        foreach (var route in routes)
        {
            var orderedStations = route.RouteStations
                .OrderBy(routeStation => routeStation.StationOrder)
                .ToList();

            var fromStation = orderedStations.FirstOrDefault(routeStation =>
                routeStation.Station is not null &&
                Normalize(routeStation.Station.Name).Contains(fromText));

            var toStation = orderedStations.FirstOrDefault(routeStation =>
                routeStation.Station is not null &&
                Normalize(routeStation.Station.Name).Contains(toText));

            if (fromStation is null || toStation is null)
            {
                continue;
            }

            if (fromStation.StationOrder == toStation.StationOrder)
            {
                return BadRequest("FROM and TO cannot be the same station.");
            }

            var tripStations = GetTripStations(orderedStations, fromStation, toStation);

            return Ok(new BusRouteSearchResult(
                route.Id,
                route.RouteName,
                route.Bus?.Number ?? string.Empty,
                fromStation.Station!.Name,
                toStation.Station!.Name,
                tripStations.Select(ToStationDto).ToList()));
        }

        return NotFound("No bus route was found for these stations.");
    }

    private static BusRouteDto ToRouteDto(Models.BusRoute route)
    {
        return new BusRouteDto(
            route.Id,
            route.RouteName,
            route.Bus?.Number ?? string.Empty,
            route.RouteStations
                .OrderBy(routeStation => routeStation.StationOrder)
                .Select(ToStationDto)
                .ToList());
    }

    private static StationDto ToStationDto(Models.RouteStation routeStation)
    {
        var station = routeStation.Station!;

        return new StationDto(
            station.Id,
            station.Name,
            station.Latitude,
            station.Longitude,
            routeStation.StationOrder);
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static List<Models.RouteStation> GetTripStations(
        IReadOnlyList<Models.RouteStation> orderedStations,
        Models.RouteStation fromStation,
        Models.RouteStation toStation)
    {
        var isForwardTrip = fromStation.StationOrder < toStation.StationOrder;
        var startOrder = Math.Min(fromStation.StationOrder, toStation.StationOrder);
        var endOrder = Math.Max(fromStation.StationOrder, toStation.StationOrder);

        var tripStations = orderedStations
            .Where(routeStation =>
                routeStation.StationOrder >= startOrder &&
                routeStation.StationOrder <= endOrder)
            .ToList();

        if (!isForwardTrip)
        {
            tripStations.Reverse();
        }

        return tripStations;
    }

}
