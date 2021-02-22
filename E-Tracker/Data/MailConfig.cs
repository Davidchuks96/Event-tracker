using E_Tracker.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class MailConfig:EntityBase
    {
        public string MailProvider { get; set; }
        public string ApiKey { get; set; }
        public string Password { get; set; }
        public ProviderType ProviderType { get; set; }
        public bool IsDefault { get; set; }
        public string Url { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
    }
}
