using E_Tracker.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class Item: EntityBase
    {
        public string Name { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemGroupId { get; set; }
        public ItemGroup ItemGroup { get; set; }
        public ItemType ItemType { get; set; }
        
        public DateTime ExpiredDate { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public string ApproveOrRejectComments { get; set; }
        public string TagNo { get; set; }

      
        public User ApprovedBy { get; set; }
        public DateTime DateApproved { get; set; }
        public string ApprovedById { get; set; }
        public int ReoccurenceValue { get; set; }
        public ReoccurenceFrequency ReoccurenceFrequency { get; set; }
        public ReoccurenceFrequency NotificationPeriod { get; set; }
        public NotificationFrequency NotificationFrequency { get; set; }
        public DateTime LastDateServiced { get; set; }
       // public object Users { get; internal set; }
    }
}
