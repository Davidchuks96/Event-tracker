
using FluentValidation;
using System.Collections.Generic;

namespace E_Tracker.Dto
{
    public class RoleDto
    {
        public RoleDto()
        {
            Claims = new List<string>();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public List<string> Claims { get; set; }
    }

    public class RoleValidation: AbstractValidator<RoleDto>
    {
        public RoleValidation()
        {
            RuleFor(r => r.Name).NotEmpty().WithMessage("A valid role name must be specified");
        }
    }
}
