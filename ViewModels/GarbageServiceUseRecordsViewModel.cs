using LeSheTuanGo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace prj0305.ViewModels
{
    public class GarbageServiceUseRecordsViewModel
    {
        GarbageServiceUseRecord garbageServiceUseRecord = new GarbageServiceUseRecord();
        public GarbageServiceUseRecord gsr { get { return garbageServiceUseRecord; } }
        public GarbageServiceUseRecordsViewModel() { }
        public GarbageServiceUseRecordsViewModel(GarbageServiceUseRecord q)
        {
            garbageServiceUseRecord = q;
        }

        [DisplayName("委託編號")]
        public int ServiceUseRecordId
        {
            get { return garbageServiceUseRecord.ServiceUseRecordId; }
            set { garbageServiceUseRecord.ServiceUseRecordId = value; }
        }
        [DisplayName("使用服務的服務編號")]
        public int GarbageServiceOfferId
        {
            get { return garbageServiceUseRecord.GarbageServiceOfferId; }
            set { garbageServiceUseRecord.GarbageServiceOfferId = value; }
        }
        [DisplayName("委託者編號")]
        public int MemberId
        {
            get { return garbageServiceUseRecord.MemberId; }
            set { garbageServiceUseRecord.MemberId = value; }
        }
        [DisplayName("要給人倒的3L袋數")]
        public byte L3count
        {
            get { return garbageServiceUseRecord.L3count; }
            set { garbageServiceUseRecord.L3count = value; }
        }
        [DisplayName("要給人倒的5L袋數")]
        public byte L5count
        {
            get { return garbageServiceUseRecord.L5count; }
            set { garbageServiceUseRecord.L5count = value; }
        }
        [DisplayName("要給人倒的14L袋數")]
        public byte L14count
        {
            get { return garbageServiceUseRecord.L14count; }
            set { garbageServiceUseRecord.L14count = value; }
        }
        [DisplayName("要給人倒的25L袋數")]
        public byte L25count
        {
            get { return garbageServiceUseRecord.L25count; }
            set { garbageServiceUseRecord.L25count = value; }
        }
        [DisplayName("要給人倒的33L袋數")]
        public byte L33count
        {
            get { return garbageServiceUseRecord.L33count; }
            set { garbageServiceUseRecord.L33count = value; }
        }
        [DisplayName("要給人倒的75L袋數")]
        public byte L75count
        {
            get { return garbageServiceUseRecord.L75count; }
            set { garbageServiceUseRecord.L75count = value; }
        }
        [DisplayName("要給人倒的120L袋數")]
        public byte L120count
        {
            get { return garbageServiceUseRecord.L120count; }
            set { garbageServiceUseRecord.L120count = value; }
        }
        [DisplayName("是否要到府服務")]
        public bool NeedCome
        {
            get { return garbageServiceUseRecord.NeedCome; }
            set { garbageServiceUseRecord.NeedCome = value; }
        }
        [DisplayName("到府位置鄉政市區")]
        public short ComeDistrictId
        {
            get { return garbageServiceUseRecord.ComeDistrictId; }
            set { garbageServiceUseRecord.ComeDistrictId = value; }
        }
        [DisplayName("到府位置道路街名")]
        public string ComeAddress
        {
            get { return garbageServiceUseRecord.ComeAddress; }
            set { garbageServiceUseRecord.ComeAddress = value; }
        }

        public virtual ICollection<DistrictRef>  ComeDistrict { get; set; }
        public virtual ICollection<GarbageServiceOffer>  GarbageServiceOffer { get; set; }
        public virtual ICollection<Member>  Member { get; set; }
    }
}
