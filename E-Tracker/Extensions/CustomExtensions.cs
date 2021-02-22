using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Extensions
{
    public static class CustomExtensions
    {
        public static int TotalMonths(this TimeSpan timeSpan)
        {
            var totalDays = timeSpan.TotalDays;
            int months = Convert.ToInt32(totalDays/28);
            return months;
        }
    }
}
