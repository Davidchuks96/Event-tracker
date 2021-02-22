using E_Tracker.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    /// <summary>
    /// User notifications on the platform
    /// </summary>
    public class Notification
    {
        public int Id { get; set; }
        public string Text { get; set; }
        /// <summary>
        /// The id of the person that triggered the notification
        /// </summary>
        public string NotificationTriggerId { get; set; }
        public User NotificationTrigger { get; set; }
        /// <summary>
        /// Time stamp for the notification
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.Now;

        /// <summary>
        /// To Approve Item or show That item has been approved
        /// </summary>
        public NotificationType NotificationType { get; set; }
        public string ItemId { get; set; }
        public Item Item { get; set; }
        public string ItemGroupId { get; set; }
        public ItemGroup ItemGroup { get; set; }
        public string ServiceId { get; set; }
        public Service Service { get; set; }
        public List<NotificationUser> NotificationUsers { get; set; }
    }
}
