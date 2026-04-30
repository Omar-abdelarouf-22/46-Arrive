namespace Arrive.BusApi.Dtos;

public record StationDto(
    int Id,
    string Name,
    double Latitude,
    double Longitude,
    int Order);

public record BusRouteDto(
    int RouteId,
    string RouteName,
    string BusNumber,
    IReadOnlyList<StationDto> Stations);

public record BusRouteSearchRequest(
    string From,
    string To);

public record BusRouteSearchResult(
    int RouteId,
    string RouteName,
    string BusNumber,
    string WaitStation,
    string DestinationStation,
    IReadOnlyList<StationDto> Stations);
