namespace Arrive.BusApi.Models;

public class Bus
{
    public int Id { get; set; }

    public string Number { get; set; } = string.Empty;

    public ICollection<BusRoute> Routes { get; set; } = new List<BusRoute>();
}
