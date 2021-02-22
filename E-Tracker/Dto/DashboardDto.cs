using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class DashboardDto
    {
        public int AllItems { get; set; }
        public int AllActiveItems { get; set; }
        public int AllExpiredItems { get; set; }
        public List<Item> ApproachingExipringDate { get; set; }
    }
}
