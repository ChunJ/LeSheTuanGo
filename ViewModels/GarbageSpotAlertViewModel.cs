using LeSheTuanGo.Models; //for viewmodel to access model
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.ViewModels
{
    public class GarbageSpotAlertViewModel
    {
        private GarbageSpotAlert iv_GarbageSpotAlert = null;
        public GarbageSpotAlert GarbageSpotAlert { get { return iv_GarbageSpotAlert; } }
        public GarbageSpotAlertViewModel()
        {
            iv_GarbageSpotAlert = new GarbageSpotAlert();
        }
        public GarbageSpotAlertViewModel(GarbageSpotAlert a)
        {
            iv_GarbageSpotAlert = a;
        }

        public int AlertId { get { return iv_GarbageSpotAlert.AlertId; } set { iv_GarbageSpotAlert.AlertId = value; } }
        public int MemberId { get { return iv_GarbageSpotAlert.MemberId; } set { iv_GarbageSpotAlert.MemberId = value; } }
        public int GarbageTruckSpotId { get { return iv_GarbageSpotAlert.GarbageTruckSpotId; } set { iv_GarbageSpotAlert.GarbageTruckSpotId = value; } }
        public byte MinutesBeforeAlert { get { return iv_GarbageSpotAlert.MinutesBeforeAlert; } set { iv_GarbageSpotAlert.MinutesBeforeAlert = value; } }

        public virtual GarbageTruckSpot GarbageTruckSpot { get; set; }
        public virtual Member Member { get; set; }
    }
}
