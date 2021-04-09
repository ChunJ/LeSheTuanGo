using System;
using System.Collections.Generic;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class NotifyContent
    {
        public NotifyContent()
        {
            Notifications = new HashSet<Notification>();
        }

        public int ContentId { get; set; }
        public string ContentText { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
