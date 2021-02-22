using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.CreateDto
{
    public class CreateItemTypeDto
    {
        [Required]
        public string Name { get; set; }
    }

    public class ItemTypeValidation : AbstractValidator<CreateItemTypeDto>
    {
        public ItemTypeValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The ItemType name must be specified");
        }
    }
}
