using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class CategoryDto: BaseEntityDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CategoryValidation : AbstractValidator<CategoryDto>
    {
        public CategoryValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The Category name must be specified");

        }
    }
}
