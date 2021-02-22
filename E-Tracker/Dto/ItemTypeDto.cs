using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class ItemTypeDto : BaseEntityDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ItemTypeValidation : AbstractValidator<ItemTypeDto>
    {
        public ItemTypeValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The ItemType name must be specified");
        }
    }
}
