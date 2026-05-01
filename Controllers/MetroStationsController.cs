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
            id = s.Id,
            name = s.Name,
            lat = s.Latitude,
            lng = s.Longitude
        }));
    }

    [HttpGet("route")]
    public async Task<IActionResult> GetRoute([FromQuery] int fromId, [FromQuery] int toId)
    {
        var stations = await context.MetroStations.ToListAsync();
        
        // Build adjacency list for Cairo Metro Graph
        var graph = BuildMetroGraph();

        if (!graph.ContainsKey(fromId) || !graph.ContainsKey(toId))
            return BadRequest("Invalid station IDs.");

        // BFS to find the shortest path in terms of number of stops
        var pathIds = FindShortestPath(graph, fromId, toId);
        if (pathIds == null)
            return NotFound("No route found between these stations.");

        var pathStations = pathIds.Select(id => stations.First(s => s.Id == id)).ToList();
        
        int stationCount = pathStations.Count;
        int price = CalculateTicketPrice(stationCount);

        return Ok(new
        {
            stationCount = stationCount,
            priceEgp = price,
            path = pathStations.Select(s => new
            {
                id = s.Id,
                name = s.Name,
                lat = s.Latitude,
                lng = s.Longitude
            })
        });
    }

    private int CalculateTicketPrice(int count)
    {
        if (count <= 9) return 10;
        if (count <= 16) return 12;
        if (count <= 23) return 15;
        return 20;
    }

    private Dictionary<int, List<int>> BuildMetroGraph()
    {
        var graph = new Dictionary<int, List<int>>();
        for (int i = 1; i <= 74; i++) graph[i] = new List<int>();

        // Line 1: 1 to 35
        for (int i = 1; i < 35; i++) AddEdge(graph, i, i + 1);

        // Line 2: 36 to 53. Wait, we need to insert the transfer stations (19 and 22) into Line 2 correctly.
        // Line 2 path: 36,37,38,39,40,41,42,43,44 -> 19 (Sadat) -> 45 -> 46 (Attaba) -> 22 (Al-Shohadaa) -> 47,48,49,50,51,52,53
        int[] line2 = { 36, 37, 38, 39, 40, 41, 42, 43, 44, 19, 45, 46, 22, 47, 48, 49, 50, 51, 52, 53 };
        for (int i = 0; i < line2.Length - 1; i++) AddEdge(graph, line2[i], line2[i + 1]);

        // Line 3: 54 to 74. Transfer at 46 (Attaba) and 20 (Nasser).
        // Line 3 path: 54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71 -> 46 (Attaba) -> 20 (Nasser) -> 72,73,74
        int[] line3 = { 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 46, 20, 72, 73, 74 };
        for (int i = 0; i < line3.Length - 1; i++) AddEdge(graph, line3[i], line3[i + 1]);

        return graph;
    }

    private void AddEdge(Dictionary<int, List<int>> graph, int u, int v)
    {
        if (!graph[u].Contains(v)) graph[u].Add(v);
        if (!graph[v].Contains(u)) graph[v].Add(u);
    }

    private List<int> FindShortestPath(Dictionary<int, List<int>> graph, int start, int target)
    {
        var queue = new Queue<int>();
        var parent = new Dictionary<int, int>();
        var visited = new HashSet<int>();

        queue.Enqueue(start);
        visited.Add(start);
        parent[start] = -1;

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            if (current == target)
            {
                var path = new List<int>();
                int curr = target;
                while (curr != -1)
                {
                    path.Add(curr);
                    curr = parent[curr];
                }
                path.Reverse();
                return path;
            }

            foreach (var neighbor in graph[current])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    parent[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return null;
    }
}
