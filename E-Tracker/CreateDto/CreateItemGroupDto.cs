using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.CreateDto
{
    public class CreateItemGroupDto
    {
        public string Name { get; set; }
        public string TagNo { get; set; }
        public string CategoryId { get; set; }
        public string DepartmentId { get; set; }
    }

    public class ItemGroupValidation : AbstractValidator<CreateItemGroupDto>
    {
        public ItemGroupValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The Item Group name must be specified");
            RuleFor(x => x.TagNo).NotEmpty().WithMessage("The Item Group Tag No must be specified");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("The Category must be specified");
            RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("The Department of the item must be specified");
        }
    }
}
