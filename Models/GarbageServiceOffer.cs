using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class GarbageServiceOffer
    {
        public GarbageServiceOffer()
        {
            GarbageServiceUseRecords = new HashSet<GarbageServiceUseRecord>();
        }

        public int GarbageServiceId { get; set; }
        public byte ServiceTypeId { get; set; }
        public int HostMemberId { get; set; }
        public short DistrictId { get; set; }
        public string Address { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool CanGo { get; set; }
        public byte GoRangeId { get; set; }
        public byte L3maxCount { get; set; }
        public byte L5maxCount { get; set; }
        public byte L14maxCount { get; set; }
        public byte L25maxCount { get; set; }
        public byte L33maxCount { get; set; }
        public byte L75maxCount { get; set; }
        public byte L120maxCount { get; set; }

        public virtual DistrictRef District { get; set; }
        public virtual RangeRef GoRange { get; set; }
        public virtual Member HostMember { get; set; }
        public virtual ServiceTypeRef ServiceType { get; set; }
        public virtual ICollection<GarbageServiceUseRecord> GarbageServiceUseRecords { get; set; }
    }
}
