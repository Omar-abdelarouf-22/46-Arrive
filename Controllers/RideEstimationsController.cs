using Arrive.BusApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Arrive.BusApi.Controllers;

[ApiController]
[Route("api/ride-estimations")]
public class RideEstimationsController : ControllerBase
{
    [HttpPost("calculate")]
    public ActionResult<RideEstimationResponse> Calculate([FromBody] RideEstimationRequest request)
    {
        // 1. Calculate Haversine Distance
        double distanceKm = HaversineDistanceKm(request.FromLat, request.FromLng, request.ToLat, request.ToLng);
        
        // Add a 20% multiplier to account for actual road routing vs straight line
        distanceKm = distanceKm * 1.2;
        
        // Ensure a minimum distance
        if (distanceKm < 1.0) distanceKm = 1.0;

        // Estimated minutes (assume average speed of 25 km/h in Cairo traffic)
        double estimatedMinutes = (distanceKm / 25.0) * 60;

        // 2. Pricing Logic
        // Uber Model: Base: 15, per km: 5.5, per min: 1.2
        double uberCar = 15 + (distanceKm * 5.5) + (estimatedMinutes * 1.2);
        double uberMoto = 10 + (distanceKm * 3.5) + (estimatedMinutes * 0.8);

        // DiDi Model: Base: 12, per km: 4.8, per min: 1.0
        double didiCar = 12 + (distanceKm * 4.8) + (estimatedMinutes * 1.0);
        double didiMoto = 8 + (distanceKm * 3.0) + (estimatedMinutes * 0.6);

        // InDrive Model: Base: 10, per km: 4.5, no per min cost but higher flat rate
        double inDriveCar = 10 + (distanceKm * 4.5);
        double inDriveMoto = 7 + (distanceKm * 2.8);

        // Add a random small surge multiplier between 1.0 and 1.15 for realism
        Random random = new Random();
        double surge = 1.0 + (random.NextDouble() * 0.15);

        var response = new RideEstimationResponse
        {
            DistanceKm = distanceKm,
            Pricing = new RidePricing
            {
                Uber = new CompanyPrice { Car = Math.Round(uberCar * surge), Moto = Math.Round(uberMoto * surge) },
                DiDi = new CompanyPrice { Car = Math.Round(didiCar * surge), Moto = Math.Round(didiMoto * surge) },
                InDrive = new CompanyPrice { Car = Math.Round(inDriveCar * surge), Moto = Math.Round(inDriveMoto * surge) }
            }
        };

        return Ok(response);
    }

    private static double HaversineDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        double r = 6371; // Earth radius in km
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return r * c;
    }

    private static double ToRadians(double angle)
    {
        return Math.PI * angle / 180.0;
    }
}
