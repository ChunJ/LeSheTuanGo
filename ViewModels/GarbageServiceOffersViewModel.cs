using LeSheTuanGo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.ViewModels
{
    public class GarbageServiceOffersViewModel
    {
        private GarbageServiceOffer garbageServiceOffer = new GarbageServiceOffer();
        public GarbageServiceOffer gso { get { return garbageServiceOffer; } }
        public GarbageServiceOffersViewModel() { }
        public GarbageServiceOffersViewModel(GarbageServiceOffer p)
        {
            garbageServiceOffer = p;
        }
        public int GarbageServiceId
        {
            get { return garbageServiceOffer.GarbageServiceId; }
            set { garbageServiceOffer.GarbageServiceId = value; }
        }
        [DisplayName("服務項目")]
        public byte ServiceTypeId
        {
            get { return garbageServiceOffer.ServiceTypeId; }
            set { garbageServiceOffer.ServiceTypeId = value; }
        }
        [DisplayName("用戶ID")]
        public int HostMemberId
        {
            //get { return garbageServiceOffer.HostMemberId; }
            get { return garbageServiceOffer.HostMemberId; }
            set { garbageServiceOffer.HostMemberId = value; }
        }
        [DisplayName("鄉鎮市區")]
        public short DistrictId
        {
            get { return garbageServiceOffer.DistrictId; }
            set { garbageServiceOffer.DistrictId = value; }
        }
        [DisplayName("地址")]
        public string Address
        {
            get { return garbageServiceOffer.Address; }
            set { garbageServiceOffer.Address = value; }
        }
        [DisplayName("開始時間")]
        public DateTime StartTime
        {
            get { return garbageServiceOffer.StartTime; }
            set { garbageServiceOffer.StartTime = value; }
        }
        [DisplayName("結束時間")]
        public DateTime EndTime
        {
            get { return garbageServiceOffer.EndTime; }
            set { garbageServiceOffer.EndTime = value; }
        }
        [DisplayName("委託是否有效")]
        public bool IsActive
        {
            get { return garbageServiceOffer.IsActive; }
            set { garbageServiceOffer.IsActive = value; }
        }
        public decimal Latitude
        {
            get { return garbageServiceOffer.Latitude; }
            set { garbageServiceOffer.Latitude = value; }
        }
        
        public decimal Longitude
        {
            get { return garbageServiceOffer.Longitude; }
            set { garbageServiceOffer.Longitude = value; }
        }
        [DisplayName("是否可到府服務")]
        public bool CanGo
        {
            get { return garbageServiceOffer.CanGo; }
            set { garbageServiceOffer.CanGo = value; }
        }
        [DisplayName("可服務距離")]
        public byte GoRangeId
        {
            get { return garbageServiceOffer.GoRangeId; }
            set { garbageServiceOffer.GoRangeId = value; }
        }
        [DisplayName("3公升")]
        public byte L3maxCount
        {
            get { return garbageServiceOffer.L3maxCount; }
            set { garbageServiceOffer.L3maxCount = value; }
        }
        [DisplayName("5公升")]
        public byte L5maxCount
        {
            get { return garbageServiceOffer.L5maxCount; }
            set { garbageServiceOffer.L5maxCount = value; }
        }
        [DisplayName("14公升")]
        public byte L14maxCount
        {
            get { return garbageServiceOffer.L14maxCount; }
            set { garbageServiceOffer.L14maxCount = value; }
        }
        [DisplayName("25公升")]
        public byte L25maxCount
        {
            get { return garbageServiceOffer.L25maxCount; }
            set { garbageServiceOffer.L25maxCount = value; }
        }
        [DisplayName("33公升")]
        public byte L33maxCount
        {
            get { return garbageServiceOffer.L33maxCount; }
            set { garbageServiceOffer.L33maxCount = value; }
        }
        [DisplayName("75公升")]
        public byte L75maxCount
        {
            get { return garbageServiceOffer.L75maxCount; }
            set { garbageServiceOffer.L75maxCount = value; }
        }
        [DisplayName("120公升")]
        public byte L120maxCount
        {
            get { return garbageServiceOffer.L120maxCount; }
            set { garbageServiceOffer.L120maxCount = value; }
        }
        
        public virtual DistrictRef District { get; set; }
        [DisplayName("鄉鎮市區")]
        public string DistrictName
        {
            get { return garbageServiceOffer.District.DistrictName; }
        }
        
        public virtual RangeRef GoRange { get; set; }
        [DisplayName("可接受距離")]
        public int RangeInMeters
        {
            get { return garbageServiceOffer.GoRange.RangeInMeters; }
        }
        public virtual Member HostMember { get; set; }
        public virtual ServiceTypeRef ServiceType { get; set; }
        [DisplayName("服務項目")]
        public string ServiceName
        {
            get { return garbageServiceOffer.ServiceType.ServiceName; }
        }
        public virtual ICollection<GarbageServiceUseRecord> GarbageServiceUseRecords { get; set; }
    }
}
