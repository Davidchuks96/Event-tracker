using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Tracker.Dto
{
    public class UserDto
    {
        public UserDto()
        {
            Roles = new List<string>();
        }

        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        [Display(Name ="User Name")]
        public string UserName { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string OtherNames { get; set; }
        public string DepartmentName { get; set; }
        public DepartmentDto Department { get; set; }
        [Required]
        public string DepartmentId { get; set; }
        public DateTime DateCreated { get; set; } 
        public DateTime DateUpdated { get; set; }
        public DateTime DateDeleted { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByFullName { get; set; }

        public string DeletedById { get; set; }
        public string DeletedByFullName { get; set; }

        public string UpdatedById { get; set; }
        public string UpdatedByFullName { get; set; }

        public bool IsActive { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class UserValidation: AbstractValidator<UserDto>
    {
        public UserValidation()
        {
            RuleFor(u => u.Email).NotEmpty().WithMessage("A valid email must be used");
            RuleFor(u => u.Surname).NotEmpty().WithMessage("This field must be specified");
            RuleFor(u => u.OtherNames).NotEmpty().WithMessage("This field must be specified");
            RuleFor(u => u.DepartmentId).NotEmpty().WithMessage("A department must be specified");
        }
    }
}
    