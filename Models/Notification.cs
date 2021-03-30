using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class Notification
    {
        public int NotifyId { get; set; }
        public int MemberId { get; set; }
        public string NotifyMessage { get; set; }
        public DateTime SentTime { get; set; }

        public virtual Member Member { get; set; }
    }
}
