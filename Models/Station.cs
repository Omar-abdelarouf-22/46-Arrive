namespace Arrive.BusApi.Models;

public class Station
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
}
