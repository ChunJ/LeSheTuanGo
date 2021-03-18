using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class Member
    {
        public Member()
        {
            GarbageServiceOffers = new HashSet<GarbageServiceOffer>();
            GarbageServiceUseRecords = new HashSet<GarbageServiceUseRecord>();
            OrderBuyRecords = new HashSet<OrderBuyRecord>();
            Orders = new HashSet<Order>();
        }

        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public short DistrictId { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Balance { get; set; }

        public virtual DistrictRef District { get; set; }
        public virtual ICollection<GarbageServiceOffer> GarbageServiceOffers { get; set; }
        public virtual ICollection<GarbageServiceUseRecord> GarbageServiceUseRecords { get; set; }
        public virtual ICollection<OrderBuyRecord> OrderBuyRecords { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
