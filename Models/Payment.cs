using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public int MemberId { get; set; }
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string TradeDesc { get; set; }
        public string ItemName { get; set; }
        public string CheckMacValue { get; set; }

        public virtual Member Member { get; set; }
    }
}
