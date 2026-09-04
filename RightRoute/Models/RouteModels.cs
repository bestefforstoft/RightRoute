using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightRoute.Models
{
    public class RouteModels
    {
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

    }
}
