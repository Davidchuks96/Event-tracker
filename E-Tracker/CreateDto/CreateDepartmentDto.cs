using E_Tracker.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.CreateDto
{
    public class CreateDepartmentDto
    {
        [Required]
       public string Name { get; set; }
    }
    public class DepartmentValidation : AbstractValidator<CreateDepartmentDto>
    {
        public DepartmentValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The department name must be specified");
        }
    }
}
