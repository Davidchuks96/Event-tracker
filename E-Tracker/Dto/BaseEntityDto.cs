using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class BaseEntityDto
    {
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDeleted { get; set; }
        public string CreatedById { get; set; }
        public string UpdatedById { get; set; }
        public string DeletedById { get; set; }
        public UserDto CreatedBy { get; set; }
        public UserDto UpdatedBy { get; set; }
        public UserDto DeletedBy { get; set; }
        public bool IsActive { get; set; }

    }
}
