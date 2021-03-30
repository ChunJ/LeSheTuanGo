using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class Notification
    {
        public int MemberId { get; set; }
        public int GarbageTruckSpotId { get; set; }
        public byte MinutesBeforeAlert { get; set; }

        public virtual GarbageTruckSpot GarbageTruckSpot { get; set; }
        public virtual Member Member { get; set; }
    }
}
