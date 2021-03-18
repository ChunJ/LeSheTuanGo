using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class DistrictRef
    {
        public DistrictRef()
        {
            GarbageServiceOffers = new HashSet<GarbageServiceOffer>();
            GarbageServiceUseRecords = new HashSet<GarbageServiceUseRecord>();
            GarbageTruckSpots = new HashSet<GarbageTruckSpot>();
            Members = new HashSet<Member>();
            OrderBuyRecords = new HashSet<OrderBuyRecord>();
            Orders = new HashSet<Order>();
        }

        public short DistrictId { get; set; }
        public string DistrictName { get; set; }
        public byte CityId { get; set; }

        public virtual CityRef City { get; set; }
        public virtual ICollection<GarbageServiceOffer> GarbageServiceOffers { get; set; }
        public virtual ICollection<GarbageServiceUseRecord> GarbageServiceUseRecords { get; set; }
        public virtual ICollection<GarbageTruckSpot> GarbageTruckSpots { get; set; }
        public virtual ICollection<Member> Members { get; set; }
        public virtual ICollection<OrderBuyRecord> OrderBuyRecords { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
