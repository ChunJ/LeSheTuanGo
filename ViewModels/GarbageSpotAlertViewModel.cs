using LeSheTuanGo.Models; //for viewmodel to access model
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.ViewModels
{
    public class GarbageSpotAlertViewModel
    {
        private GarbageSpotLike iv_GarbageSpotAlert = null;
        public GarbageSpotLike GarbageSpotAlert { get { return iv_GarbageSpotAlert; } }
        public GarbageSpotAlertViewModel()
        {
            iv_GarbageSpotAlert = new GarbageSpotLike();
        }
        public GarbageSpotAlertViewModel(GarbageSpotLike a)
        {
            iv_GarbageSpotAlert = a;
        }

        public int LikeId { get { return iv_GarbageSpotAlert.LikeId; } set { iv_GarbageSpotAlert.LikeId = value; } }
        public int MemberId { get { return iv_GarbageSpotAlert.MemberId; } set { iv_GarbageSpotAlert.MemberId = value; } }
        public int GarbageTruckSpotId { get { return iv_GarbageSpotAlert.GarbageTruckSpotId; } set { iv_GarbageSpotAlert.GarbageTruckSpotId = value; } }
        public byte MinutesBeforeNotify { get { return iv_GarbageSpotAlert.MinutesBeforeNotify; } set { iv_GarbageSpotAlert.MinutesBeforeNotify = value; } }

        public bool NotifyMe { get { return iv_GarbageSpotAlert.NotifyMe; } set { iv_GarbageSpotAlert.NotifyMe = value; } }

        public virtual GarbageTruckSpot GarbageTruckSpot { get; set; }
        public virtual Member Member { get; set; }
    }
}
