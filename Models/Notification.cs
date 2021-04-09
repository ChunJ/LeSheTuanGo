using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class Notification
    {
        public int NotifyId { get; set; }
        public int MemberId { get; set; }
        public int ContentId { get; set; }
        public DateTime SentTime { get; set; }
        public byte SourceType { get; set; }
        public int SourceId { get; set; }
        public bool Checked { get; set; }

        public virtual NotifyContent Content { get; set; }
        public virtual Member Member { get; set; }
    }
}
