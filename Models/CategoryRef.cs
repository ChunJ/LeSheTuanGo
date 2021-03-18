using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class CategoryRef
    {
        public CategoryRef()
        {
            Products = new HashSet<Product>();
        }

        public byte CategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
