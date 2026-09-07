# RightRoute Quick Start Guide

Get up and running with RightRoute in 5 minutes!

## 30-Second Overview

RightRoute helps you create optimized delivery routes. You simply:
1. **Add** multiple stop locations
2. **Optimize** the route sequence for efficiency
3. **Save** your route for later
4. **Launch** your phone's maps to navigate

## Installation & Running

### Windows
```bash
dotnet run -f net10.0-windows10.0.19041.0
```

### Android (Emulator)
```bash
dotnet run -f net10.0-android
```

### iOS (macOS required)
```bash
dotnet run -f net10.0-ios
```

## Your First Route (5 Minutes)

### 1. Launch RightRoute

The Route Editor page loads automatically with an empty form.

### 2. Name Your Route

At the top, enter a name:
- Example: "My First Route"

### 3. Add Three Stops

**Add Stop 1:**
1. Type in "Where do you want to go?": `Pike Place Market, Seattle`
2. Click the **"+"** button
3. The app geocodes the address (Android/iOS) or asks you to enter coordinates
4. Stop 1 appears in the list with a blue badge "1"

**Add Stop 2:**
1. Type: `Space Needle, Seattle`
2. Click **"+""**
3. Stop 2 appears with badge "2"

**Add Stop 3:**
1. Type: `Seattle Center`
2. Click **"+""**
3. Stop 3 appears with badge "3"

### 4. Optimize the Route

1. Click the **"Optimize Path"** button (DarkSlateBlue)
2. You'll see:
   - "Route optimized successfully!" message
   - Waypoints might be reordered
   - Badges update to new sequence

### 5. Save Your Route

1. Click **"Save Loadout"** (Green button)
2. You'll see: "Loadout saved successfully!"
3. Route is now stored in the local database

### 6. Launch Navigation

1. Click **"Launch Maps"** (OrangeRed button)
2. Your native maps app opens:
   - **Android**: Google Maps with navigation
   - **iOS**: Apple Maps with navigation
   - **Windows**: Bing Maps in browser
3. You can now navigate using optimized sequence

**Done!** 🎉

## Common Tasks

### Add a Stop Using Coordinates (No Geocoding)

Required on Windows; optional on Android/iOS:

1. Type location name: `My Office`
2. Enter "Manual Lat": `47.6062`
3. Enter "Manual Lng": `-122.3321`
4. Click **"+"**

### Delete a Stop

1. Find the stop in the list
2. Click the trash icon (🗑️) on the right
3. Stop is removed immediately

### Create Another Route

1. Modify the loadout name at the top
2. Click **"Save Loadout"** to save as new route
3. Or refresh the page to start fresh

### Toggle Round-Trip Mode

- **Enabled** (default): Route returns to the starting point
- **Disabled**: Route ends at the last waypoint

## Troubleshooting Quick Fixes

| Problem | Solution |
|---------|----------|
| "Could not find coordinates" | Add city name to address (e.g., "Main St, Seattle") |
| "Permission denied" (Android) | Tap OK, grant location permission in system dialog |
| "Permission denied" (iOS) | Go to Settings > Privacy > Location > RightRoute and enable |
| "Could not open maps" (Windows) | Make sure Bing Maps or a browser is installed |
| Maps app won't launch | Check that Google Maps (Android) or Apple Maps (iOS) is installed |

## Platform Differences

| Feature | Windows | Android | iOS |
|---------|---------|---------|-----|
| Geocoding | ❌ Manual only | ✅ Auto | ✅ Auto |
| Maps App | Bing/Browser | Google Maps | Apple Maps |
| Multiple Waypoints | ✅ Yes | ✅ Yes | ⚠️ Start/End only |
| Permission Requests | N/A | ✅ Dynamic | ✅ Once only* |

*iOS: If user denies, they must enable in Settings app

## Tips & Tricks

### For Best Results

1. **Be specific with addresses**
   - ✅ "123 Main Street, Seattle, WA 98101"
   - ❌ "Main Street"

2. **Add 2+ stops before optimizing**
   - Optimization requires at least 2 waypoints

3. **Check sequence after optimization**
   - Make sure reordered waypoints make sense geographically

4. **Use manual coordinates for known locations**
   - Faster than geocoding if you know lat/lng

5. **Enable round-trip toggle for delivery routes**
   - Automatically returns to starting point

### Keyboard Shortcuts

- **Enter/Return** in address field: Add waypoint (same as clicking "+")
- **Ctrl+S** (Windows): May not work; use "Save Loadout" button instead

## Feature Overview

### Route Optimization Algorithm

RightRoute uses the **Open Source Routing Machine (OSRM)**:
- Calculates actual driving distances/times (not straight-line)
- Optimizes for shortest total distance
- Takes into account real road networks
- Works globally (not just local)

### Data Storage

Your routes are stored locally in SQLite:
- File location: `{AppDataDirectory}/RouteLoadouts.db3`
- Private to your app (encrypted on mobile)
- No cloud sync (future enhancement)

### Permissions Required

**Android**:
- `ACCESS_FINE_LOCATION`: Geocoding
- `ACCESS_COARSE_LOCATION`: Geocoding fallback
- `INTERNET`: API calls

**iOS**:
- `NSLocationWhenInUseUsageDescription`: Geocoding when app is active

**Windows**:
- None (manual coordinates only)

## Next Steps

1. ✅ Created your first route
2. ✅ Optimized waypoint sequence
3. ✅ Launched navigation

**What to do now?**
- Create additional routes for different areas
- Test with real addresses in your delivery area
- Export or share saved routes (future version)
- Provide feedback on UI/UX

## Need Help?

- 📖 Full documentation: See [README.md](README.md)
- 🔧 Technical details: Check [GEOCODING_AND_PERMISSIONS_FIX.md](GEOCODING_AND_PERMISSIONS_FIX.md)
- 🐛 Debug logging: View in Visual Studio Output window

## Feature Requests & Bugs

Found an issue? Have an idea? Open an issue on GitHub:
- 🐛 Bug report
- 💡 Feature request
- 📝 Documentation improvement

---

**Welcome to RightRoute!** Happy routing! 🚗📍
