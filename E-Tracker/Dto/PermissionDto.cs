using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class PermissionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; } = DateTime.Now;
        public DateTime? DateDeleted { get; set; } = DateTime.Now;
        public string CreatedById { get; set; }
    }

    public class PermissionValidation : AbstractValidator<PermissionDto>
    {
        public PermissionValidation()
        {
            RuleFor(r => r.Name).NotEmpty().WithMessage("A valid role name must be specified");
        }
    }
}

