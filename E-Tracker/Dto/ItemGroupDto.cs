using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class ItemGroupDto : BaseEntityDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TagNo { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedByOtherNames { get; set; }
        public string ApprovedByFullNames { get; set; }
        
        public DateTime DateApproved { get; set; }
        public string ApprovedById { get; set; }
        public UserDto ApprovedBy { get; set; }
    }
   
}
