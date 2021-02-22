using E_Tracker.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class AutoGenServicePeriod
    {
        public string Id { get; set; }
        public string ItemId { get; set; }

        public DateTime PresentExpiryDate { get; set; }
        //As at when this Log was made, 
        //ReoccurenceValue and ReoccurenceFrequency can change
        //so we need to keep track of what it was as at the time of creation/logging
        public int ReoccurenceValue { get; set; }
        public ReoccurenceFrequency ReoccurenceFrequency { get; set; }
        public DateTime NextExpiryDate { get; set; }

        public Item Item { get; set; }

        //Boolean to identify if that particular service date was actually met
        public bool IsServiceDateMet { get; set; }

    }
}
