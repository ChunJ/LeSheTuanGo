using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class GarbageTruckSpot
    {
        public GarbageTruckSpot()
        {
            GarbageSpotLikes = new HashSet<GarbageSpotLike>();
        }

        public int GarbageTruckSpotId { get; set; }
        public short DistrictId { get; set; }
        public string Address { get; set; }
        public short RouteId { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan LeaveTime { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public virtual DistrictRef District { get; set; }
        public virtual Route Route { get; set; }
        public virtual ICollection<GarbageSpotLike> GarbageSpotLikes { get; set; }
    }
}
