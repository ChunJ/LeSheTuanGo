using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class GarbageServiceUseRecord
    {
        public int ServiceUseRecordId { get; set; }
        public int GarbageServiceOfferId { get; set; }
        public int MemberId { get; set; }
        public byte L3count { get; set; }
        public byte L5count { get; set; }
        public byte L14count { get; set; }
        public byte L25count { get; set; }
        public byte L33count { get; set; }
        public byte L75count { get; set; }
        public byte L120count { get; set; }
        public bool NeedCome { get; set; }
        public short ComeDistrictId { get; set; }
        public string ComeAddress { get; set; }

        public virtual DistrictRef ComeDistrict { get; set; }
        public virtual GarbageServiceOffer GarbageServiceOffer { get; set; }
        public virtual Member Member { get; set; }
    }
}
