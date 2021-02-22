using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Models
{
    public class SearchViewModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ItemDepartmentName { get; set; }
        public string ItemDepartmentId { get; set; }

        //So you search for the services for a particular item
        public string ItemTagNo { get; set; }
        public string SerivceDepartmentId { get; set; }
    }
}
