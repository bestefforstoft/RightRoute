using System.Text.Json;
using static RightRoute.Models.RouteModels;

namespace RightRoute.Services;

public class OsrmService
{
    private readonly HttpClient _httpClient = new();

    public async Task<List<RouteWaypoint>> OptimizeRouteAsync(List<RouteWaypoint> waypoints, bool isRoundTrip)
    {
        if (waypoints == null || waypoints.Count < 2) return waypoints;

        // Formulate coordinates format: lng,lat;lng,lat
        var coordStrings = waypoints.Select(w => $"{w.Longitude},{w.Latitude}");
        string coordinates = string.Join(";", coordStrings);

        // Adjust behavior flag settings dynamically based on user toggle options
        string roundTripParam = isRoundTrip ? "true" : "false";
        string destinationParam = isRoundTrip ? "last" : "any";

        string url = $"https://project-osrm.org{coordinates}?source=first&destination={destinationParam}&roundtrip={roundTripParam}&geometries=false";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return waypoints;

        string json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.GetProperty("code").GetString() == "Ok")
        {
            var waypointsArray = root.GetProperty("waypoints");
            var optimizedList = new List<RouteWaypoint>();

            // Map returned optimized indexes back to original items array
            foreach (var wpElement in waypointsArray.EnumerateArray())
            {
                int waypointIndex = wpElement.GetProperty("waypoint_index").GetInt32();
                int originalIndex = wpElement.GetProperty("trips_index").GetInt32();

                var originalWaypoint = waypoints[originalIndex];
                originalWaypoint.SequenceOrder = waypointIndex;
                optimizedList.Add(originalWaypoint);
            }

            return optimizedList.OrderBy(w => w.SequenceOrder).ToList();
        }

        return waypoints;
    }
}
