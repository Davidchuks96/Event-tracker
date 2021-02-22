using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data
{
    public class ItemGroup: EntityBase
    {
        public string Name { get; set; }
        public string TagNo { get; set; }
        public string CategoryId { get; set; }
        public Category Category { get; set; }
        public string DepartmentId { get; set; }
        public Department Department { get; set; }
        public bool IsApproved { get; set; }
        public User ApprovedBy { get; set; }
        public DateTime DateApproved { get; set; }
        public string ApprovedById { get; set; }
    }
}
