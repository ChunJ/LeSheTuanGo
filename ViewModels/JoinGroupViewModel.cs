using prj0305.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace prj0305.ViewModels
{
    public class JoinGroupViewModel
    {
        private Order iv_order = null;
        private ProductViewModels iv_productView=null;
        public JoinGroupViewModel(Order o)
        {
            iv_order = o;
        }
        public JoinGroupViewModel()
        {
            iv_order = new Order();
            iv_productView = new ProductViewModels();
        }
        public Order JOrder { get { return iv_order; } }
        public ProductViewModels VProduct { get { return iv_productView; } }
        [DisplayName("訂單ID")]
        public int OrderId { get; set; }
        [DisplayName("商品ID")]
        public int ProductId { get; set; }
        [DisplayName("團主ID")]
        public int HostMemberId { get; set; }
        [DisplayName("使用者ID")]
        public bool UseMemberAddress { get; set; }
        [DisplayName("地區ID")]
        public short? DistrictId { get; set; }
        [DisplayName("地址")]
        public string Address { get; set; }
        [DisplayName("經度")]
        public decimal? Latitude { get; set; }
        [DisplayName("緯度")]
        public decimal? Longitude { get; set; }
        [DisplayName("最大數量")]
        public byte? MaxCount { get; set; }
        [DisplayName("開團時間")]
        public DateTime? StartTime { get; set; }
        [DisplayName("結束時間")]
        public DateTime? EndTime { get; set; }
        [DisplayName("訂單描述")]
        public string OrderDescription { get; set; }
        [DisplayName("訂單狀態")]
        public bool IsActive { get; set; }
        [DisplayName("團購是否滿員")]
        public bool? IsFull { get; set; }
        [DisplayName("單位價格")]
        public decimal? UnitPrice { get; set; }
        [DisplayName("是否接受外送")]
        public bool? CanGo { get; set; }
        [DisplayName("距離")]
        public byte? GoRangeId { get; set; }
        [DisplayName("可用數量")]
        public byte AvailableCount { get; set; }
        public virtual DistrictRef District { get; set; }
        public virtual RangeRef GoRange { get; set; }
        public virtual Member HostMember { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderBuyRecord> OrderBuyRecords { get; set; }
    }
}
