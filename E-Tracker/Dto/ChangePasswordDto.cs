using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class ChangePasswordDto
    {
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage =
            "Password mismatch.")]
        public string ConfirmPassword { get; set; }
    }
}
