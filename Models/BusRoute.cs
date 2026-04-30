namespace Arrive.BusApi.Models;

public class BusRoute
{
    public int Id { get; set; }

    public string RouteName { get; set; } = string.Empty;

    public int BusId { get; set; }

    public Bus? Bus { get; set; }

    public ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
}
