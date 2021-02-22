using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
