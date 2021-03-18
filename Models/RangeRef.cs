using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class RangeRef
    {
        public RangeRef()
        {
            GarbageServiceOffers = new HashSet<GarbageServiceOffer>();
            Orders = new HashSet<Order>();
        }

        public byte RangeId { get; set; }
        public short RangeInMeters { get; set; }

        public virtual ICollection<GarbageServiceOffer> GarbageServiceOffers { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
