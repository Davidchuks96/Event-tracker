using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Tracker.Data
{
    public class Department : EntityBase
    {
        
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<ItemGroup> ItemGroups { get; set; }

    }
}
