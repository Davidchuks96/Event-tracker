using E_Tracker.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.CreateDto
{
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Surname { get; set; }
        public string OtherNames { get; set; }
        public DepartmentDto Department { get; set; }
        public string DepartmentId { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [StringLength(14, ErrorMessage = "The {0} be at least {2} and at most {1}")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Does not match the password entered.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserValidation : AbstractValidator<CreateUserDto>
    {
        public UserValidation()
        {
            RuleFor(u => u.Email).NotEmpty().WithMessage("A valid email must be used");
            RuleFor(u => u.Surname).NotEmpty().WithMessage("This field must be specified");
            RuleFor(u => u.OtherNames).NotEmpty().WithMessage("This field must be specified");
            RuleFor(u => u.DepartmentId).NotEmpty().WithMessage("A department must be specified");
            RuleFor(u => u.Password).NotEmpty().WithMessage("A password must be entered");
            //RuleFor(u => u.Password).Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{6,15}$").WithMessage("Passwords must have at least one non alphanumeric character and at least one digit('0' - '9') and at least one uppercase('A' - 'Z').");
            RuleFor(u => u.ConfirmPassword).NotEmpty().WithMessage("Please enter the same password as the one above");
        }
    }
}
