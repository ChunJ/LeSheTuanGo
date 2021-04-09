using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class GarbageSpotLike
    {
        public int LikeId { get; set; }
        public int MemberId { get; set; }
        public int GarbageTruckSpotId { get; set; }
        public byte MinutesBeforeNotify { get; set; }
        public bool NotifyMe { get; set; }

        public virtual GarbageTruckSpot GarbageTruckSpot { get; set; }
        public virtual Member Member { get; set; }
    }
}
