using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class SendGridSettings
    {
        public string Url { get; set; }
        public string ApiKey { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
    }
}
