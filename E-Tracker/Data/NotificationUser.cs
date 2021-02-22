using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class NotificationUser
    {
        public int NotificationId { get; set; }
        public Notification Notification { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
