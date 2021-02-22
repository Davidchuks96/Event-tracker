using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class MailLog:EntityBase
    {
        public string To { get; set; }
        public string RecipientName { get; set; }
        public string  From { get; set; }
        public string Receiver { get; set; }
        public string MessageBody { get; set; }
        public string Subject { get; set; }
        public DateTime DateSent { get; set; } = DateTime.Now;
        public bool IsSent { get; set; }
    }
}
