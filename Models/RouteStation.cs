namespace Arrive.BusApi.Models;

public class RouteStation
{
    public int Id { get; set; }

    public int BusRouteId { get; set; }

    public BusRoute? BusRoute { get; set; }

    public int StationId { get; set; }

    public Station? Station { get; set; }

    public int StationOrder { get; set; }
}
