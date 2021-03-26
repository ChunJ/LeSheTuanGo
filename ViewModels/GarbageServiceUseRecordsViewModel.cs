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

        [DisplayName("使用者ID")]
        public int ServiceUseRecordId
        {
            get { return garbageServiceUseRecord.ServiceUseRecordId; }
            set { garbageServiceUseRecord.ServiceUseRecordId = value; }
        }
        [DisplayName("創造活動者ID")]
        public int GarbageServiceOfferId
        {
            get { return garbageServiceUseRecord.GarbageServiceOfferId; }
            set { garbageServiceUseRecord.GarbageServiceOfferId = value; }
        }
        [DisplayName("會員ID")]
        public int MemberId
        {
            get { return garbageServiceUseRecord.MemberId; }
            set { garbageServiceUseRecord.MemberId = value; }
        }
        [DisplayName("L3數量")]
        public byte L3count
        {
            get { return garbageServiceUseRecord.L3count; }
            set { garbageServiceUseRecord.L3count = value; }
        }
        [DisplayName("L5數量")]
        public byte L5count
        {
            get { return garbageServiceUseRecord.L5count; }
            set { garbageServiceUseRecord.L5count = value; }
        }
        [DisplayName("L14數量")]
        public byte L14count
        {
            get { return garbageServiceUseRecord.L14count; }
            set { garbageServiceUseRecord.L14count = value; }
        }
        [DisplayName("L25數量")]
        public byte L25count
        {
            get { return garbageServiceUseRecord.L25count; }
            set { garbageServiceUseRecord.L25count = value; }
        }
        [DisplayName("L33數量")]
        public byte L33count
        {
            get { return garbageServiceUseRecord.L33count; }
            set { garbageServiceUseRecord.L33count = value; }
        }
        [DisplayName("L75數量")]
        public byte L75count
        {
            get { return garbageServiceUseRecord.L75count; }
            set { garbageServiceUseRecord.L75count = value; }
        }
        [DisplayName("L120數量")]
        public byte L120count
        {
            get { return garbageServiceUseRecord.L120count; }
            set { garbageServiceUseRecord.L120count = value; }
        }
        [DisplayName("是否前往")]
        public bool NeedCome
        {
            get { return garbageServiceUseRecord.NeedCome; }
            set { garbageServiceUseRecord.NeedCome = value; }
        }
        [DisplayName("區域")]
        public short ComeDistrictId
        {
            get { return garbageServiceUseRecord.ComeDistrictId; }
            set { garbageServiceUseRecord.ComeDistrictId = value; }
        }
        [DisplayName("地址")]
        public string ComeAddress
        {
            get { return garbageServiceUseRecord.ComeAddress; }
            set { garbageServiceUseRecord.ComeAddress = value; }
        }
    }
}
