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

        public RouteEditorViewModel()
        {
            // For production, inject these via your AppShell MauiProgram dependency containers
            _dbService = new DatabaseService();
            _osrmService = new OsrmService();

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

        // Command: Add a waypoint manually to the current active array sandbox
        [RelayCommand]
        private async Task AddWaypointAsync()
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
                    // Call the built-in phone system to translate text into map points
                    var locations = await Geocoding.Default.GetLocationsAsync(NewWaypointDescription);
                    var firstLocation = locations?.FirstOrDefault();

                    if (firstLocation != null)
                    {
                        lat = firstLocation.Latitude;
                        lng = firstLocation.Longitude;
                        locationFound = true;
                    }
                }
                catch (Exception ex)
                {
                    // Handles connection dropouts or system lookup errors gracefully
                    await Shell.Current.DisplayAlertAsync($"ex Search Failed", "Could not find coordinates for that address. Check your connection.", "OK");
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
                await Shell.Current.DisplayAlertAsync("Not Found", "Address not found. Try adding a city or zip code to the text.", "OK");
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
        private async Task OptimizeRouteSequenceAsync()
        {
            if (Waypoints.Count < 2) return;

            var inputList = Waypoints.ToList();
            var sortedList = await _osrmService.OptimizeRouteAsync(inputList, IsRoundTrip);

            Waypoints.Clear();
            foreach (var wp in sortedList)
            {
                Waypoints.Add(wp);
            }
        }

        // Command: Save Current State configurations directly to SQLite tables
        [RelayCommand]
        private async Task SaveLoadoutAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentRoute.Name)) return;

            await _dbService.SaveRouteLoadoutAsync(CurrentRoute, Waypoints.ToList());
            await Shell.Current.DisplayAlertAsync("Success", "Loadout saved successfully!", "OK");
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
