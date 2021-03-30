using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderBuyRecords = new HashSet<OrderBuyRecord>();
        }

        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int HostMemberId { get; set; }
        public short DistrictId { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public byte MaxCount { get; set; }
        public byte AvailableCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
        public string OrderDescription { get; set; }
        public decimal UnitPrice { get; set; }
        public bool CanGo { get; set; }
        public byte GoRangeId { get; set; }

        public virtual DistrictRef District { get; set; }
        public virtual RangeRef GoRange { get; set; }
        public virtual Member HostMember { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderBuyRecord> OrderBuyRecords { get; set; }
    }
}
