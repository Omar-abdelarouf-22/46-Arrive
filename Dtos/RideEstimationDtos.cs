namespace Arrive.BusApi.Dtos;

public class RideEstimationRequest
{
    public double FromLat { get; set; }
    public double FromLng { get; set; }
    public double ToLat { get; set; }
    public double ToLng { get; set; }
}

public class RideEstimationResponse
{
    public double DistanceKm { get; set; }
    public RidePricing Pricing { get; set; } = new();
}

public class RidePricing
{
    public CompanyPrice Uber { get; set; } = new();
    public CompanyPrice DiDi { get; set; } = new();
    public CompanyPrice InDrive { get; set; } = new();
}

public class CompanyPrice
{
    public double Car { get; set; }
    public double Moto { get; set; }
}
