using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class Route
    {
        public Route()
        {
            GarbageTruckSpots = new HashSet<GarbageTruckSpot>();
        }

        public short RouteId { get; set; }
        public string RouteName { get; set; }
        public string RouteCode { get; set; }
        public string Schedule { get; set; }

        public virtual ICollection<GarbageTruckSpot> GarbageTruckSpots { get; set; }
    }
}
