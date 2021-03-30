using prj0305.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace prj0305.ViewModels
{
    public class ProductViewModels
    {
        public ProductViewModels()
        {
            Orders = new HashSet<Order>();
        }

        [DisplayName("商品ID")]
        public int ProductId { get; set; }
        [DisplayName("類別ID")]
        public byte CategoryId { get; set; }
        [DisplayName("商品名稱")]
        public string ProductName { get; set; }
        [DisplayName("商品圖片")]
        public byte[] ProductImage { get; set; }
        [DisplayName("商品描述")]
        public string ProductDescription { get; set; }

        public virtual CategoryRef Category { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
