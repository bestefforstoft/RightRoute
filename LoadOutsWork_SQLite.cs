using SQLite;

namespace RouteLoadoutApp.Services;

// Data model for a Saved Route Loadout
public class RouteLoadout
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Data model for points inside a Loadout
public class RouteWaypoint
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int RouteId { get; set; } // Links to RouteLoadout.Id
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SequenceOrder { get; set; } // Order after optimization
}

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task InitAsync()
    {
        if (_database is not null) return;

        // Path: Local device application sandboxed storage
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "RouteLoadouts.db3");
        _database = new SQLiteAsyncConnection(dbPath);

        // Automatically create tables if they do not exist yet
        await _database.CreateTableAsync<RouteLoadout>();
        await _database.CreateTableAsync<RouteWaypoint>();
    }

    public async Task<List<RouteLoadout>> GetRoutesAsync()
    {
        await InitAsync();
        return await _database!.Table<RouteLoadout>().OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<List<RouteWaypoint>> GetWaypointsForRouteAsync(int routeId)
    {
        await InitAsync();
        return await _database!.Table<RouteWaypoint>()
                               .Where(w => w.RouteId == routeId)
                               .OrderBy(w => w.SequenceOrder)
                               .ToListAsync();
    }

    public async Task SaveRouteLoadoutAsync(RouteLoadout route, List<RouteWaypoint> waypoints)
    {
        await InitAsync();

        if (route.Id == 0)
        {
            await _database!.InsertAsync(route); // Generates auto-increment ID
        }
        else
        {
            await _database!.UpdateAsync(route);
            // Wipe out old structural waypoints to overwrite with current edits
            await _database.ExecuteAsync("DELETE FROM RouteWaypoint WHERE RouteId = ?", route.Id);
        }

        // Apply updated parent route IDs to child waypoints and save
        foreach (var wp in waypoints)
        {
            wp.RouteId = route.Id;
        }
        await _database!.InsertAllAsync(waypoints);
    }

    public async Task DeleteRouteAsync(int routeId)
    {
        await InitAsync();
        await _database!.RunInTransactionAsync(tran =>
        {
            tran.Execute("DELETE FROM RouteLoadout WHERE Id = ?", routeId);
            tran.Execute("DELETE FROM RouteWaypoint WHERE RouteId = ?", routeId);
        });
    }
}
