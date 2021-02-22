using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.CreateDto
{
    public class CreatePermissionDto
    {
        public string Name { get; set; }
    }

    public class PermissionValidation : AbstractValidator<CreatePermissionDto>
    {
        public PermissionValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The ItemType name must be specified");
        }
    }
}
