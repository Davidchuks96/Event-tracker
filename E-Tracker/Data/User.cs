using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace E_Tracker.Data
{
    public class User: IdentityUser
    {
        public ICollection<IdentityRole> Roles { get; set; }
        public ICollection<IdentityRoleClaim<string>> Claims { get; set; }
        public List<NotificationUser> NotificationUsers { get; set; }
        public Department Department { get; set; }  
        public string DepartmentId { get; set; }
        public string Surname { get; set; }
        public string OtherNames { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDeleted { get; set; }
        public string CreatedById { get; set; }
        public string UpdatedById { get; set; }
        public string DeletedById { get; set; }
        public bool IsActive { get; set; }
        public bool IsFirstLogin { get; set; } = true;
    }
}
