using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class Reminder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Message { get; set; }
        public string ReceipentEmail { get; set; }
        //public string ReceipentName { get; set; }
        /// <summary>
        /// Time stamp for the notification
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateOfReminder { get; set; }

        public string ItemId { get; set; }
        public Item Item { get; set; }
        
        //This happens when an item has expired and it has not been serviced
        public bool IsEscalation { get; set; } = false;
        public string EscalatedById { get; set; }
        public bool IsSent { get; set; } = false;   
    }
}
