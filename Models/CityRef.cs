using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class CityRef
    {
        public CityRef()
        {
            DistrictRefs = new HashSet<DistrictRef>();
        }

        public byte CityId { get; set; }
        public string CityName { get; set; }

        public virtual ICollection<DistrictRef> DistrictRefs { get; set; }
    }
}
