using System.Text.Json;
using static RightRoute.Models.RouteModels;

namespace RightRoute.Services;

public class OsrmService
{
    private readonly HttpClient _httpClient = new();

    public async Task<List<RouteWaypoint>> OptimizeRouteAsync(List<RouteWaypoint> waypoints, bool isRoundTrip)
    {
        if (waypoints == null || waypoints.Count < 2)
        {
            return waypoints;
        }

        try
        {
            // Formulate coordinates format: lng,lat;lng,lat
            var coordStrings = waypoints.Select(w => $"{w.Longitude},{w.Latitude}");
            string coordinates = string.Join(";", coordStrings);

            // Adjust behavior flag settings dynamically based on user toggle options
            string roundTripParam = isRoundTrip ? "true" : "false";
            string destinationParam = isRoundTrip ? "last" : "any";

            string url = $"https://router.project-osrm.org/trip/v1/driving/{coordinates}?source=first&destination={destinationParam}&roundtrip={roundTripParam}&geometries=false";

            System.Diagnostics.Debug.WriteLine($"OSRM Request URL: {url}");

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"OSRM API Error: {response.StatusCode} - {response.ReasonPhrase}");
                return waypoints;
            }

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("code", out var codeProperty) && codeProperty.GetString() == "Ok")
            {
                var waypointsArray = root.GetProperty("waypoints");
                var optimizedList = new List<RouteWaypoint>();

                // Map returned optimized indexes back to original items array
                foreach (var wpElement in waypointsArray.EnumerateArray())
                {
                    int waypointIndex = wpElement.GetProperty("waypoint_index").GetInt32();
                    int tripsIndex = wpElement.GetProperty("trips_index").GetInt32();

                    if (tripsIndex >= 0 && tripsIndex < waypoints.Count)
                    {
                        var originalWaypoint = waypoints[tripsIndex];
                        originalWaypoint.SequenceOrder = waypointIndex;
                        optimizedList.Add(originalWaypoint);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"OSRM Optimization successful: {optimizedList.Count} waypoints optimized");
                return optimizedList.OrderBy(w => w.SequenceOrder).ToList();
            }
            else
            {
                var errorMessage = root.TryGetProperty("message", out var msg) ? msg.GetString() : "Unknown error";
                System.Diagnostics.Debug.WriteLine($"OSRM API returned error: {errorMessage}");
                return waypoints;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OSRM Service Exception: {ex.Message}");
            return waypoints;
        }
    }
}
