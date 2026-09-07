using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RightRoute.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static RightRoute.Models.RouteModels;

namespace RightRoute.ViewModels
{
    public partial class RouteEditorViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;
        private readonly OsrmService _osrmService;
        private readonly NavigationService _navigationService;

        public RouteEditorViewModel(DatabaseService dbService, OsrmService osrmService, NavigationService navigationService)
        {
            _dbService = dbService;
            _osrmService = osrmService;
            _navigationService = navigationService;

            CurrentRoute = new RouteLoadout { Name = "New Loadout Route" };
            Waypoints = new ObservableCollection<RouteWaypoint>();
        }

        [ObservableProperty]
        private RouteLoadout _currentRoute;

        [ObservableProperty]
        private bool _isRoundTrip = true; // Default behavior: Return back home

        [ObservableProperty]
        private string _newWaypointDescription = string.Empty;

        [ObservableProperty]
        private string _newLatitude = string.Empty;

        [ObservableProperty]
        private string _newLongitude = string.Empty;

        // Direct UI binding data source for editing list view structures
        public ObservableCollection<RouteWaypoint> Waypoints { get; }

        /// <summary>
        /// Requests location permission from the user if not already granted.
        /// </summary>
        private async Task<bool> RequestLocationPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Granted)
                {
                    return true; // Permission already granted
                }

                if (status == PermissionStatus.Denied && DeviceInfo.Current.Platform == DevicePlatform.iOS)
                {
                    // On iOS, denied permissions cannot be re-requested; user must go to Settings
                    await Shell.Current.DisplayAlertAsync(
                        "Location Permission Denied",
                        "Location access is required for geocoding addresses. Please enable it in Settings > Privacy > Location.",
                        "OK"
                    );
                    return false;
                }

                // Request permission from user
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Permission request error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error", $"Could not request location permission: {ex.Message}", "OK");
                return false;
            }
        }

        // Command: Add a waypoint manually to the current active array sandbox
        [RelayCommand]
        private async Task AddWaypoint()
        {
            // Safety check: ensure they actually typed something in the description box
            if (string.IsNullOrWhiteSpace(NewWaypointDescription))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter an address or location name.", "OK");
                return;
            }

            double lat = 0;
            double lng = 0;
            bool locationFound = false;

            // SCENARIO A: The user typed numeric numbers manually into the Lat/Lng boxes
            if (double.TryParse(NewLatitude, out lat) && double.TryParse(NewLongitude, out lng))
            {
                locationFound = true;
            }
            // SCENARIO B: The Lat/Lng boxes are blank, so we look up the street text
            else
            {
                try
                {
                    // Check if platform supports geocoding
                    if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
                    {
                        // Windows doesn't have built-in geocoding support in MAUI
                        await Shell.Current.DisplayAlertAsync(
                            "Not Supported on Windows",
                            "Automatic geocoding is not supported on Windows. Please enter coordinates manually in the Lat/Lng fields.",
                            "OK"
                        );
                        return;
                    }

                    // Request permission before attempting geocoding
                    if (!await RequestLocationPermissionAsync())
                    {
                        await Shell.Current.DisplayAlertAsync(
                            "Permission Required",
                            "Location permission is required for geocoding addresses.",
                            "OK"
                        );
                        return;
                    }

                    // Call the built-in phone system to translate text into map points
                    var locations = await Geocoding.Default.GetLocationsAsync(NewWaypointDescription);
                    var firstLocation = locations?.FirstOrDefault();

                    if (firstLocation != null)
                    {
                        lat = firstLocation.Latitude;
                        lng = firstLocation.Longitude;
                        locationFound = true;

                        // Pre-fill the coordinate boxes so user can verify
                        NewLatitude = lat.ToString("F6");
                        NewLongitude = lng.ToString("F6");
                    }
                }
                catch (FeatureNotSupportedException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Geocoding not supported: {ex.Message}");
                    await Shell.Current.DisplayAlertAsync(
                        "Geocoding Not Available",
                        "Geocoding is not supported on this platform. Please enter coordinates manually.",
                        "OK"
                    );
                    return;
                }
                catch (Exception ex)
                {
                    // Handles connection dropouts or system lookup errors gracefully
                    System.Diagnostics.Debug.WriteLine($"Geocoding error: {ex.Message}");
                    await Shell.Current.DisplayAlertAsync(
                        "Search Failed",
                        $"Could not find coordinates for that address. Check your internet connection and try again. Error: {ex.Message}",
                        "OK"
                    );
                    return;
                }
            }

            // If we successfully found a location, add it to our weapon loadout array
            if (locationFound)
            {
                Waypoints.Add(new RouteWaypoint
                {
                    Description = NewWaypointDescription,
                    Latitude = lat,
                    Longitude = lng,
                    SequenceOrder = Waypoints.Count
                });

                // Wipe the input boxes clean so the user can type the next stop instantly
                NewWaypointDescription = string.Empty;
                NewLatitude = string.Empty;
                NewLongitude = string.Empty;
            }
            else
            {
                await Shell.Current.DisplayAlertAsync(
                    "Not Found",
                    "Address not found. Try adding a city or zip code to the text.",
                    "OK"
                );
            }
        }

        // Command: Delete an single stop instantly out of the workspace editor
        [RelayCommand]
        private void RemoveWaypoint(RouteWaypoint waypoint)
        {
            if (Waypoints.Contains(waypoint))
            {
                Waypoints.Remove(waypoint);
            }
        }

        // Command: Process OSRM API trip algorithm optimization over internal items
        [RelayCommand]
        private async Task OptimizeRouteSequence()
        {
            if (Waypoints.Count < 2)
            {
                await Shell.Current.DisplayAlertAsync("Not Enough Waypoints", "Add at least 2 stops to optimize the route.", "OK");
                return;
            }

            try
            {
                var inputList = Waypoints.ToList();
                var sortedList = await _osrmService.OptimizeRouteAsync(inputList, IsRoundTrip);

                Waypoints.Clear();
                foreach (var wp in sortedList)
                {
                    Waypoints.Add(wp);
                }

                await Shell.Current.DisplayAlertAsync("Success", "Route optimized successfully!", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Route optimization error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Optimization Failed", $"Could not optimize route: {ex.Message}", "OK");
            }
        }

        // Command: Save Current State configurations directly to SQLite tables
        [RelayCommand]
        private async Task SaveLoadout()
        {
            if (string.IsNullOrWhiteSpace(CurrentRoute.Name))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a loadout name.", "OK");
                return;
            }

            if (Waypoints.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Add at least one stop before saving.", "OK");
                return;
            }

            try
            {
                await _dbService.SaveRouteLoadoutAsync(CurrentRoute, Waypoints.ToList());
                await Shell.Current.DisplayAlertAsync("Success", "Loadout saved successfully!", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database save error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Save Failed", $"Could not save loadout: {ex.Message}", "OK");
            }
        }

        // Command: Launch native maps with the optimized route
        [RelayCommand]
        private async Task LaunchNativeMaps()
        {
            if (Waypoints.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("No Route", "Add stops and optimize the route before launching maps.", "OK");
                return;
            }

            try
            {
                var waypointsList = Waypoints.OrderBy(w => w.SequenceOrder).ToList();
                await _navigationService.LaunchNativeMapsAsync(waypointsList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Launch maps error: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Launch Error", $"Could not open native maps: {ex.Message}", "OK");
            }
        }

        // Method to execute when navigating here to load an existing saved profile loadout
        public async Task LoadSavedRouteAsync(int routeId)
        {
            var routes = await _dbService.GetRoutesAsync();
            var route = routes.FirstOrDefault(r => r.Id == routeId);

            if (route != null)
            {
                CurrentRoute = route;
                var savedWaypoints = await _dbService.GetWaypointsForRouteAsync(routeId);

                Waypoints.Clear();
                foreach (var wp in savedWaypoints)
                {
                    Waypoints.Add(wp);
                }
            }
        }
    }

}
