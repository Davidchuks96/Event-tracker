using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.CreateDto
{
    public class CreateRoleDto
    {
        public string Name { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDeleted { get; set; }
        public string CreatedById { get; set; }
    }

    public class RoleValidation : AbstractValidator<CreateRoleDto>
    {
        public RoleValidation()
        {
            RuleFor(r => r.Name).NotEmpty().WithMessage("A valid role name must be specified");
        }
    }
}
