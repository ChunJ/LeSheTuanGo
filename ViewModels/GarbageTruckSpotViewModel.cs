using LeSheTuanGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.ViewModels
{
    public class GarbageTruckSpotViewModel
    {
        private GarbageTruckSpot iv_GarbageTruckSpot = null;
        public GarbageTruckSpot GarbageTruckSpot { get { return iv_GarbageTruckSpot; } }
        public GarbageTruckSpotViewModel()
        {
            iv_GarbageTruckSpot = new GarbageTruckSpot();
        }
        public GarbageTruckSpotViewModel(GarbageTruckSpot g)
        {
            iv_GarbageTruckSpot = g;
        }

        public int GarbageTruckSpotId { get { return iv_GarbageTruckSpot.GarbageTruckSpotId; } set { iv_GarbageTruckSpot.GarbageTruckSpotId = value; } }
        public short DistrictId { get { return iv_GarbageTruckSpot.DistrictId; } set { iv_GarbageTruckSpot.DistrictId = value; } }
        public string Address { get { return iv_GarbageTruckSpot.Address; } set { iv_GarbageTruckSpot.Address = value; } }
        public short RouteId { get { return iv_GarbageTruckSpot.RouteId; } set { iv_GarbageTruckSpot.RouteId = value; } }
        public TimeSpan ArrivalTime { get { return iv_GarbageTruckSpot.ArrivalTime; } set { iv_GarbageTruckSpot.ArrivalTime = value; } }
        public TimeSpan LeaveTime { get { return iv_GarbageTruckSpot.LeaveTime; } set { iv_GarbageTruckSpot.LeaveTime = value; } }
        public decimal Latitude { get { return iv_GarbageTruckSpot.Latitude; } set { iv_GarbageTruckSpot.Latitude = value; } }
        public decimal Longitude { get { return iv_GarbageTruckSpot.Longitude; } set { iv_GarbageTruckSpot.Longitude = value; } }

        public virtual DistrictRef District { get; set; }
        public virtual Route Route { get; set; }
        public virtual ICollection<GarbageSpotAlert> GarbageSpotAlerts { get; set; }

    }
}
