using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class TrashBag
    {
        public byte TrashBagId { get; set; }
        public byte Volumn { get; set; }
        public byte BagsPerPackage { get; set; }
        public short Price { get; set; }
    }
}
