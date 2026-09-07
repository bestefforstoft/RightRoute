using Microsoft.Maui.ApplicationModel;
using RightRoute.Models;
using static RightRoute.Models.RouteModels;

namespace RightRoute.Services
{
    public class NavigationService
    {
        public async Task LaunchNativeMapsAsync(List<RouteWaypoint> optimizedWaypoints)
        {
            if (optimizedWaypoints == null || optimizedWaypoints.Count == 0)
                return;

            string url = string.Empty;

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // Android Workflow: Uses the official Google Maps URL parameters API
                // Format: https://www.google.com/maps/dir/?api=1&origin=ORIGIN&destination=DEST&waypoints=WAY1|WAY2
                var first = optimizedWaypoints.First();
                var last = optimizedWaypoints.Last();

                string origin = $"{first.Latitude},{first.Longitude}";
                string destination = $"{last.Latitude},{last.Longitude}";

                // Collect any middle points between the first and last stops
                var midPoints = optimizedWaypoints.Skip(1).Take(optimizedWaypoints.Count - 2)
                                                 .Select(w => $"{w.Latitude},{w.Longitude}");

                url = $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}";

                if (midPoints.Any())
                {
                    string waypoints = string.Join("|", midPoints);
                    url += $"&waypoints={waypoints}";
                }
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                // iOS Workflow: Uses Apple Maps Map Links standard
                // Format: http://maps.apple.com/?saddr=START&daddr=END&dirflg=d
                var first = optimizedWaypoints.First();
                var last = optimizedWaypoints.Last();

                string saddr = $"{first.Latitude},{first.Longitude}";
                string daddr = $"{last.Latitude},{last.Longitude}";

                url = $"http://maps.apple.com/?saddr={saddr}&daddr={daddr}&dirflg=d";

                // Note: Apple Maps doesn't support multiple waypoints via URL scheme like Google Maps does.
                // For multiple stops, consider using the Maps app's native functionality or alternative routing services.
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                // Windows Workflow: Uses Bing Maps or Windows Maps
                // Format: bingmaps:?rtp=pos.LATITUDE,LONGITUDE
                var first = optimizedWaypoints.First();
                var last = optimizedWaypoints.Last();

                string origin = $"pos.{first.Latitude},{first.Longitude}";
                string destination = $"pos.{last.Latitude},{last.Longitude}";

                url = $"bingmaps:?rtp={origin}~{destination}";

                // Fallback to web-based Bing Maps if native app doesn't respond
                if (!await TryLaunchAsync(url))
                {
                    url = $"https://www.bing.com/maps/directions?rtp={origin}~{destination}";
                }
            }

            // Launch the appropriate maps application
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    await Launcher.Default.OpenAsync(new Uri(url));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation launch error: {ex.Message}");
                    // Fallback handles situations where browser access or app linking fails on the device
                    await Shell.Current.DisplayAlertAsync(
                        "Navigation Error",
                        "Could not open the native mapping application. Please ensure a maps app is installed.",
                        "OK"
                    );
                }
            }
        }

        /// <summary>
        /// Attempts to launch a URI and returns whether it was successful.
        /// Used as a fallback mechanism for platform-specific URI schemes.
        /// </summary>
        private async Task<bool> TryLaunchAsync(string uri)
        {
            try
            {
                await Launcher.Default.OpenAsync(new Uri(uri));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
