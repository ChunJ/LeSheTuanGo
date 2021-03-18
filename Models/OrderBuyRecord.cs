using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class OrderBuyRecord
    {
        public int OrderBuyRecordId { get; set; }
        public int OrderId { get; set; }
        public int MemberId { get; set; }
        public byte Count { get; set; }
        public bool NeedCome { get; set; }
        public short ComeDistrictId { get; set; }
        public string ComeAddress { get; set; }

        public virtual DistrictRef ComeDistrict { get; set; }
        public virtual Member Member { get; set; }
        public virtual Order Order { get; set; }
    }
}
