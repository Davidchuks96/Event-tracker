using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class EmailSettings
    {
        public bool SSl { get; set; } = false;
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
    }
}
