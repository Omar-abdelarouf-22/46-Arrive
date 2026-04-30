using Arrive.BusApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arrive.BusApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetroStationsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMetroStations()
    {
        var stations = await context.MetroStations.ToListAsync();
        return Ok(stations.Select(s => new
        {
            name = s.Name,
            lat = s.Latitude,
            lng = s.Longitude
        }));
    }
}
