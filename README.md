# RightRoute

**RightRoute** is a cross-platform route planning and optimization application built with .NET MAUI. It allows users to create delivery routes, optimize the waypoint sequence using advanced algorithms, and launch native mapping applications to execute the optimized route.

## Features

### ✨ Core Functionality

- **📍 Waypoint Management**
  - Add multiple stops via address name or manual coordinates (latitude/longitude)
  - Automatic address geocoding on Android and iOS
  - Manual coordinate entry on all platforms (including Windows and as fallback)
  - Delete individual stops easily
  - Visual sequence indicator for each waypoint

- **🚗 Route Optimization**
  - Optimize waypoint order using the Open Source Routing Machine (OSRM) API
  - Choose between round-trip mode (return to start) or one-way route
  - Real-time re-ordering of waypoints based on optimal travel sequence

- **💾 Persistent Storage**
  - Save route loadouts to SQLite database
  - Name your routes for easy identification
  - Load previously saved routes
  - Edit and re-save existing routes

- **🗺️ Native Maps Integration**
  - Launch native mapping applications with optimized waypoints
  - **Android**: Google Maps with turn-by-turn navigation
  - **iOS**: Apple Maps with turn-by-turn navigation
  - **Windows**: Bing Maps or web-based alternative

- **🔐 Cross-Platform Permission Handling**
  - Automatic location permission requests
  - Geo-fencing awareness for each platform
  - Clear user prompts and error messages
  - Platform-specific fallbacks

## How to Use RightRoute

### Step 1: Create a Route Loadout

1. Launch the app and give your route a name (e.g., "East Side Deliveries")
2. Toggle "Return to start at end of route?" if you want a round-trip route

### Step 2: Add Waypoints

#### Option A: Using Address Geocoding (Android/iOS)
1. Type an address in the "Where do you want to go?" field
   - Example: "123 Main Street, Seattle, WA"
   - Example: "Pike Place Market"
2. Click the **"+"** button or press Enter
3. The app will automatically geocode the address to coordinates
4. The waypoint appears in the list with coordinates displayed

#### Option B: Using Manual Coordinates (All Platforms)
1. Enter an address or location name in the text field
2. Fill in the "Manual Lat" field with latitude (e.g., 47.6062)
3. Fill in the "Manual Lng" field with longitude (e.g., -122.3321)
4. Click the **"+"** button
5. The waypoint is added with your exact coordinates

**Repeat for each stop you want to add.**

### Step 3: Review Your Route

- See all added stops in the collection below
- Each waypoint shows:
  - Sequential order badge (blue box with number)
  - Location description
  - Latitude and longitude coordinates
- Delete any stop by clicking the trash icon (🗑️)

### Step 4: Optimize the Route

1. Ensure you have at least 2 stops added
2. Click the **"Optimize Path"** button
3. The app will:
   - Call the OSRM (Open Source Routing Machine) API
   - Calculate the most efficient travel sequence
   - Reorder waypoints based on optimal routing
   - Update the sequence badges automatically

### Step 5: Save Your Route

1. Review the optimized sequence
2. Click the **"Save Loadout"** button
3. Your route is saved to the local SQLite database
4. Success message confirms the save

### Step 6: Launch Navigation

1. Click the **"Launch Maps"** button (OrangeRed)
2. Your default native maps app will open with:
   - Starting waypoint as origin
   - Final waypoint as destination
   - All intermediate waypoints as route waypoints
   - Turn-by-turn navigation enabled

3. Follow the route in your navigation app
4. Execute deliveries in the optimized sequence

## Platform-Specific Details

### Windows

**Geocoding**: Manual coordinates only
- Automatic address geocoding is not available on Windows
- You must enter latitude and longitude manually
- Use any map reference tool to find coordinates

**Maps Integration**: Bing Maps
- Routes will open in Bing Maps via `bingmaps://` URI scheme
- Falls back to web-based Bing Maps if native app isn't installed
- Supports full waypoint sequences

**no Runtime Permissions**: Not applicable
- No location permission requests required

### Android

**Geocoding**: Automatic with permission request
- Permissions: `ACCESS_FINE_LOCATION`, `ACCESS_COARSE_LOCATION`
- First geocoding attempt will prompt user for location permission
- Permission can be granted/denied per request

**Maps Integration**: Google Maps
- Requires Google Maps app to be installed
- Uses official Google Maps URL scheme (`https://www.google.com/maps/dir/`)
- Supports unlimited waypoints
- Provides optimal route rendering and navigation

**Permissions Declared**: `AndroidManifest.xml`
```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.INTERNET" />
```

### iOS

**Geocoding**: Automatic with permission request
- Permission: `NSLocationWhenInUseUsageDescription`
- First geocoding attempt will prompt for location permission
- **Important**: If user denies, app cannot request again (iOS limitation)
  - User must enable in Settings > Privacy > Location

**Maps Integration**: Apple Maps
- Built-in system app (always available)
- Uses Apple Maps URL scheme (`http://maps.apple.com/`)
- Limited to start and end waypoints (Apple Maps doesn't support multiple waypoints via URL)

**Permissions Required in Info.plist**:
```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>RightRoute needs your location to geocode addresses and optimize your delivery route.</string>
<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>RightRoute needs your location to geocode addresses and optimize your delivery route.</string>
```

## Technical Architecture

### Technology Stack

- **Framework**: .NET MAUI 8.0+ (net10.0+)
- **UI Framework**: MAUI Controls & XAML
- **MVVM**: MVVM Community Toolkit 8.4.2
- **Database**: SQLite (sqlite-net-pcl)
- **Routing**: Open Source Routing Machine (OSRM) API
- **Navigation**: Platform-native maps APIs

### Project Structure

```
RightRoute/
├── Views/
│   ├── RouteEditorPage.xaml          # Main route editing UI
│   └── RouteEditorPage.xaml.cs       # Code-behind
├── ViewModels/
│   └── RouteEditorViewModel.cs       # Route editing logic & commands
├── Services/
│   ├── DatabaseService.cs            # SQLite operations
│   ├── OsrmService.cs                # Route optimization via OSRM API
│   └── NavigationService.cs          # Native maps launching
├── Models/
│   └── RouteModels.cs                # RouteLoadout & RouteWaypoint entities
├── Platforms/
│   ├── Android/AndroidManifest.xml   # Android permissions
│   ├── iOS/Info.plist                # iOS permissions & descriptions
│   └── Windows/...                   # Windows platform files
├── App.xaml                          # App-level resources & shell
├── App.xaml.cs                       # App startup code
├── MauiProgram.cs                    # Dependency injection setup
└── README.md                         # This file
```

### Data Models

#### RouteLoadout
```csharp
public class RouteLoadout
{
	public int Id { get; set; }                    // Primary key
	public string Name { get; set; }               // Route name (e.g., "East Side")
	public DateTime CreatedAt { get; set; }        // Creation timestamp
}
```

#### RouteWaypoint
```csharp
public class RouteWaypoint
{
	public int Id { get; set; }                    // Primary key
	public int RouteId { get; set; }               // Foreign key to RouteLoadout
	public double Latitude { get; set; }           // Waypoint latitude
	public double Longitude { get; set; }          // Waypoint longitude
	public string Description { get; set; }        // User-friendly location name
	public int SequenceOrder { get; set; }         // Order after optimization
}
```

### API Integrations

#### Open Source Routing Machine (OSRM)
- **Endpoint**: `https://router.project-osrm.org/trip/v1/driving/{coordinates}`
- **Purpose**: Calculates optimal waypoint sequence
- **Authentication**: None (public API)
- **Parameters**:
  - `source=first`: Start from first waypoint
  - `destination=last` or `any`: End point behavior
  - `roundtrip=true/false`: Round-trip mode
  - `geometries=false`: Don't return route geometry (save bandwidth)

#### Platform Maps APIs
- **Android**: Google Maps URI Scheme (`https://www.google.com/maps/dir/`)
- **iOS**: Apple Maps URI Scheme (`http://maps.apple.com/`)
- **Windows**: Bing Maps URI Scheme (`bingmaps://`)

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- Visual Studio 2022 (recommended) or Visual Studio Code
- MAUI workload installed (`dotnet workload install maui`)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/RightRoute.git
   cd RightRoute
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run on Windows:
   ```bash
   dotnet run -f net10.0-windows10.0.19041.0
   ```

4. Run on Android (requires Android SDK/emulator):
   ```bash
   dotnet run -f net10.0-android
   ```

5. Run on iOS (requires macOS & Xcode):
   ```bash
   dotnet run -f net10.0-ios
   ```

## Workflow Example

### Scenario: Planning a Delivery Route

**Goal**: Plan a delivery route for 4 stops in Seattle, optimized for fuel efficiency.

**Steps**:

1. **Create Loadout**
   - Name: "Downtown Seattle Deliveries"
   - Toggle: ✓ "Return to start at end of route?"

2. **Add Stops**
   - Stop 1: "Pike Place Market, Seattle"
   - Stop 2: "Space Needle, Seattle"
   - Stop 3: "Seattle Art Museum"
   - Stop 4: "Washington State Convention Center"

3. **Review Initial Order**
   - Stops are shown in the order entered
   - Currently not optimized for travel

4. **Optimize**
   - Click "Optimize Path"
   - OSRM calculates best sequence
   - Stops are reordered (e.g., 1→3→2→4→1)

5. **Save**
   - Click "Save Loadout"
   - Route is saved to local database

6. **Execute**
   - Click "Launch Maps"
   - Google Maps (Android) or Apple Maps (iOS) opens
   - Show optimized route with navigation
   - Follow turn-by-turn directions

7. **Complete**
   - Driver executes deliveries in optimized order
   - Returns to start point automatically

## Error Handling & Troubleshooting

### "Could not find coordinates for that address"

**Causes**:
- Address doesn't exist or is misspelled
- Internet connection unavailable
- Geocoding not supported on this platform

**Solutions**:
- Double-check address spelling
- Add city/state/zip code for disambiguation
- Use manual coordinate entry as fallback
- On Windows: Must use manual coordinates

### "Location Permission Denied"

**Android**:
- App will re-request permission on next geocoding attempt
- User can grant in app permission dialog

**iOS**:
- User must enable in: Settings > Privacy > Location > RightRoute
- Cannot request again from app after denial

### "Could not open the native mapping application"

**Causes**:
- Required maps app not installed
- Device doesn't support URI scheme handling

**Solutions**:
- **Android**: Install Google Maps from Play Store
- **iOS**: Apple Maps is built-in (should always work)
- **Windows**: Ensure Bing Maps or web browser is available

### "Optimization failed" or OSRM API errors

**Causes**:
- Internet connection unavailable
- OSRM API service is down
- Waypoint coordinates are invalid

**Solutions**:
- Check internet connection
- Verify waypoint coordinates are reasonable (lat: -90 to 90, lng: -180 to 180)
- Wait and retry
- Check Open Source Routing Machine status at https://project-osrm.org

## Debug Logging

Enable detailed logging in Visual Studio's Debug Output window:

**To view logs**:
1. In Visual Studio: Debug > Windows > Output
2. Look for messages containing:
   - `Permission request error`
   - `Geocoding error`
   - `OSRM Request URL`
   - `OSRM API Error`
   - `Navigation launch error`

**Example log output**:
```
Permission request error: LocationWhenInUse permission granted
OSRM Request URL: https://router.project-osrm.org/trip/v1/driving/...
OSRM Optimization successful: 4 waypoints optimized
Navigation launch error: None, launching maps successfully
```

## Known Limitations

1. **iOS Apple Maps**
   - Cannot specify more than start/end waypoints via URL scheme
   - Users may need to manually reorder waypoints in Apple Maps for full control

2. **Windows Geocoding**
   - Automatic address geocoding not supported
   - Manual coordinate entry required

3. **Permission Re-requests (iOS)**
   - Once user denies location permission, app cannot request it again
   - User must manually enable in device settings

4. **OSRM API**
   - Requires active internet connection
   - Public API (rate-limited by default)
   - Not real-time traffic-aware (uses general routing model)

5. **Database**
   - SQLite storage is local-only (not synced to cloud)
   - No automatic backup mechanism

## Future Enhancements

- [ ] Cloud sync of saved routes
- [ ] Real-time traffic integration
- [ ] Route editing in map view
- [ ] Delivery confirmation (photos, signatures)
- [ ] Multiple vehicle support
- [ ] Route sharing between team members
- [ ] Estimated delivery time calculations
- [ ] Alternative routing service support
- [ ] Route history and analytics

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, questions, or suggestions:

1. Check the Troubleshooting section above
2. Review the `GEOCODING_AND_PERMISSIONS_FIX.md` documentation
3. Check debug logs in Visual Studio Output window
4. Open an issue on GitHub

## Acknowledgments

- [Open Source Routing Machine (OSRM)](https://project-osrm.org) - Route optimization
- [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/) - Cross-platform framework
- [MVVM Community Toolkit](https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/) - MVVM architecture
- [SQLite-net PCL](https://github.com/praeclarum/sqlite-net) - Database access

## Roadmap

### Version 1.0 (Current)
- ✅ Route creation and management
- ✅ Waypoint geocoding & manual entry
- ✅ Route optimization via OSRM
- ✅ Native maps integration
- ✅ SQLite persistence
- ✅ Cross-platform support (Windows, Android, iOS)

### Version 1.1 (Planned)
- [ ] Route templates
- [ ] Favorite locations
- [ ] Estimated time & distance calculations
- [ ] Enhanced error messages

### Version 2.0 (Future)
- [ ] Team collaboration
- [ ] Cloud synchronization
- [ ] Real-time tracking
- [ ] Advanced analytics

---

**RightRoute v1.0** - Plan. Optimize. Navigate. 🚗
