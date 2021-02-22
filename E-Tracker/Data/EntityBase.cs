using System;

namespace E_Tracker.Data
{
    public class EntityBase
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDeleted { get; set; }
        public string CreatedById { get; set; }
        public string UpdatedById { get; set; }
        public string DeletedById { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
