using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Data.Enums
{
    public enum NotificationFrequency
    {
        [Display(Name ="Once")]
        once,
        [Display(Name ="Twice")]
        twice,
        [Display(Name ="Thrice")]
        thrice
    }
}
