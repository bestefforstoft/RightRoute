# RightRoute - Quick Reference Card

## 🎯 What is RightRoute?

RightRoute helps you **create**, **optimize**, **save**, and **navigate** delivery routes on Windows, Android, and iOS.

---

## 🚀 Quick Workflow

```
1. Name Route  →  2. Add Stops  →  3. Optimize  →  4. Save  →  5. Navigate
```

---

## 📱 UI Layout

```
┌─────────────────────────────────────────┐
│  📝 Loadout Name: "My Route"            │  ← Give your route a name
├─────────────────────────────────────────┤
│  📍 Where do you want to go?  [+]      │  ← Add stop (via address)
│  📐 Manual Lat: [ ]  Manual Lng: [ ]   │  ← Or manual coordinates
├─────────────────────────────────────────┤
│  ↩️  Return to start? [Toggle: ON]     │  ← Round-trip mode
├─────────────────────────────────────────┤
│  Stops in Current Loadout:              │
│  ┌─────────────────────────────────────┐│
│  │ [1] Pike Place Market               ││
│  │     Lat: 47.609205  Lng: -122.34154 ││ [🗑]
│  │ [2] Space Needle                    ││
│  │     Lat: 47.620506  Lng: -122.34949 ││ [🗑]
│  │ [3] Seattle Center                  ││
│  │     Lat: 47.621201  Lng: -122.34956 ││ [🗑]
│  └─────────────────────────────────────┘│
├─────────────────────────────────────────┤
│ [Optimize Path] [Save Loadout] [Launch] │  ← Action buttons
└─────────────────────────────────────────┘
```

---

## 🎮 Button Reference

| Button | Color | Action | Requirement |
|--------|-------|--------|-------------|
| **+** | DarkSlateBlue | Add waypoint | Address or coordinates |
| **Optimize Path** | DarkSlateBlue | Re-order waypoints | 2+ stops |
| **Save Loadout** | Green | Save to database | Route name + 1+ stops |
| **Launch Maps** | OrangeRed | Open navigation | 1+ stops |
| **🗑** (Trash) | Red | Delete stop | Click in list |

---

## 🌍 Platform Differences

| Feature | Windows | Android | iOS |
|---------|---------|---------|-----|
| Geocoding | ❌ Manual only | ✅ Auto + Permission | ✅ Auto + Permission |
| Maps App | Bing/Web | Google Maps | Apple Maps |
| Multiple Waypoints | ✅ | ✅ | ⚠️ Start/End |
| Permission Flow | N/A | Grant per request | One-time prompt* |

*iOS: Settings > Privacy > Location if denied

---

## 📍 How to Add a Stop

### Option A: Address Geocoding (Android/iOS)
```
1. Type: "123 Main Street, Seattle, WA"
2. Press Enter or click [+]
3. Grant permission if prompted
4. ✅ Stop added with auto-detected coordinates
```

### Option B: Manual Coordinates (All Platforms)
```
1. Type: "My Office" (or any name)
2. Fill in "Manual Lat": 47.6062
3. Fill in "Manual Lng": -122.3321
4. Click [+]
5. ✅ Stop added
```

---

## 🚗 Route Optimization

**What it does**: Reorders stops for shortest travel distance

**How to use**:
1. Add 2+ stops
2. Click **[Optimize Path]**
3. Stops are re-ordered by optimal route
4. Sequence numbers (badges) update

**How it works**:
- Uses OSRM (Open Source Routing Machine) API
- Calculates real road distances (not straight-line)
- Works globally with live map data

---

## 💾 Saving Routes

**Why save?**
- Persist routes to device
- Reload saved routes anytime
- Build route library

**How to save**:
1. Enter route name
2. Add 1+ stops
3. Click **[Save Loadout]** (Green)
4. ✅ Route saved to SQLite database

**Location**: `AppDataDirectory/RouteLoadouts.db3`

---

## 🗺️ Launching Navigation

**What happens**:
- Native maps app opens
- Shows all stops in optimized order
- Start → Waypoints → End
- Turn-by-turn navigation enabled

**Step-by-step**:
1. Add stops + optimize + save
2. Click **[Launch Maps]** (OrangeRed)
3. Maps app opens automatically
4. Follow turn-by-turn directions

**Platform behavior**:
- **Android** (Google Maps): Full waypoint support
- **iOS** (Apple Maps): Start & end only
- **Windows** (Bing Maps): Full waypoint support

---

## 🔒 Permissions

### Android
**Need**: `ACCESS_FINE_LOCATION` + `ACCESS_COARSE_LOCATION`
**When**: First attempt to geocode
**How**: App requests via system dialog
**Re-request**: Yes, if denied

### iOS
**Need**: Location permission
**When**: First attempt to geocode
**How**: System shows one-time prompt
**After deny**: Must enable in Settings app

### Windows
**Need**: None
**Workaround**: Enter coordinates manually

---

## 🐛 Troubleshooting Cheat Sheet

| Problem | Solution |
|---------|----------|
| "Address not found" | Add city/state: "Main St → Main St, Seattle, WA" |
| "Permission denied" (Android) | Tap OK, grant when prompted |
| "Permission denied" (iOS) | Settings > Privacy > Location > RightRoute > On |
| "Not supported on Windows" | Use Manual Lat/Lng fields instead |
| "Could not open maps" | Install Google Maps (Android) or check browser (Windows) |
| "Could not optimize route" | Check internet connection; try again |
| "No stops added" message | Add at least 1 waypoint before saving |

---

## 📊 Data Stored

### Per Route
- Route name
- Creation timestamp
- List of waypoints

### Per Waypoint
- Address/description
- Latitude coordinate
- Longitude coordinate
- Sequence order (after optimization)

### Storage
- **Location**: Device local storage (SQLite)
- **Privacy**: Not synced cloud; local-only
- **Backup**: Manual user backup needed

---

## ⌨️ Keyboard Shortcuts

| Key | Action |
|-----|--------|
| **Enter** (in address field) | Add waypoint (same as [+]) |
| **Esc** (if supported) | Cancel dialog (depends on platform) |

---

## 🔧 Settings/Toggles

### Round-Trip Mode
- **ON** (default): Route returns to starting point
- **OFF**: Route ends at final waypoint

### Manual Coordinates
- **When to use**: 
  - Windows (only option for geocoding)
  - Fallback if geocoding fails
  - Exact locations you know latitude/longitude for

---

## 💡 Pro Tips

1. **Be specific with addresses**
   - ❌ "Main Street"
   - ✅ "123 Main Street, Seattle, WA 98101"

2. **Test with small routes first**
   - 2-3 stops before complex routes
   - Verify optimization is working

3. **Check sequence after optimize**
   - Review badge numbers
   - Ensure geographic sense

4. **Use manual coordinates for known locations**
   - Hotels, offices, warehouses
   - Faster than geocoding

5. **Save frequently**
   - Save after each optimization
   - Build library of common routes

---

## 📞 Getting Help

| Need | Location |
|------|----------|
| Quick start | See **QUICK_START.md** |
| Full guide | See **README.md** |
| Features list | See **FEATURES.md** |
| Technical details | See **GEOCODING_AND_PERMISSIONS_FIX.md** |
| Error messages | See **README.md** Troubleshooting |
| Debug logs | Visual Studio Debug Output window |

---

## 🚀 Typical Session

```
📱 Open App
	↓
📝 Name Route: "Downtown Deliveries"
	↓
📍 Add Stop 1: "Pike Place Market"
	→ Grant permission (Android) or confirm (iOS)
	→ Geocoded to coordinates
	↓
📍 Add Stop 2: "Space Needle"
	→ Geocoded to coordinates
	↓
📍 Add Stop 3: "Museum"
	→ Geocoded to coordinates
	↓
🔄 Click "Optimize Path"
	→ Stops reorder to [1→3→2→1]
	↓
💾 Click "Save Loadout"
	→ Route saved to database
	↓
🗺️  Click "Launch Maps"
	→ Google Maps (Android) | Apple Maps (iOS) | Bing Maps (Windows)
	→ Navigation started!
	↓
✅ Complete!
```

---

## 📈 Limitations

- ❌ Geocoding not available on Windows (use manual coordinates)
- ⚠️ Apple Maps: Maximum 2 waypoints (start/end; iOS limitation)
- ❌ Requires internet connection (geocoding + optimization)
- ❌ Routes are local-only (no cloud sync in v1.0)
- ❌ Real-time traffic not included (uses general routing model)

---

## 🎓 Next Steps

1. Read **QUICK_START.md** for hands-on guide
2. Create your first route
3. Test with real addresses
4. Save common routes for reuse
5. Check README.md for advanced tips

---

**RightRoute v1.0 Reference Card**
*Print this page for quick reference!*

✨ Happy routing! 🚗
