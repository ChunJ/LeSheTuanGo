using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class ChatMessageRecord
    {
        public int MessageId { get; set; }
        public byte GroupType { get; set; }
        public int GroupId { get; set; }
        public DateTime SentTime { get; set; }
        public int SentMemberId { get; set; }
        public string Message { get; set; }

        public virtual Member SentMember { get; set; }
    }
}
