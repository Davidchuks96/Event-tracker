using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.CreateDto
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CategoryValidation : AbstractValidator<CreateCategoryDto>
    {
        public CategoryValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The Category name must be specified");
            
        }
    }
}
