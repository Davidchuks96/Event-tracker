using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data.Enums
{
    public enum ReoccurenceFrequency
    {
        [Display(Name ="Day")]
        day,
        [Display(Name ="Week")]
        week,
        [Display(Name ="Month")]
        month,
        [Display(Name = "Year")]
        year
    }
}
