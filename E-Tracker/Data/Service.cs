using E_Tracker.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class Service: EntityBase
    {
        public string ServiceDepartmentId { get; set; }
        public Department ServiceDepartment { get; set; }
        public string ItemId { get; set; }
        public Item Item { get; set; }
        public bool IsServiceApproved { get; set; }
        public string Status { get; set; }
        public string ApproveOrRejectComments { get; set; }
        public User ServiceApprovedBy { get; set; }
        public string ServiceApprovedById { get; set; }
        public DateTime DateApproved { get; set; }
        [Required]
        public DateTime DateServiced { get; set; }
        [Required]
        public DateTime NewExpiryDate { get; set; }
        public string Comments { get; set; }
        public bool IsANewCycle { get; set; }
        public bool IsANewReoccurenceFrequency { get; set; }
        public int NewReoccurenceValue { get; set; }
        public ReoccurenceFrequency NewReoccurenceFrequency { get; set; }

    }
}
