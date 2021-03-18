using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class ServiceTypeRef
    {
        public ServiceTypeRef()
        {
            GarbageServiceOffers = new HashSet<GarbageServiceOffer>();
        }

        public byte ServiceTypeId { get; set; }
        public string ServiceName { get; set; }

        public virtual ICollection<GarbageServiceOffer> GarbageServiceOffers { get; set; }
    }
}
