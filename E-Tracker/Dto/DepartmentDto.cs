using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace E_Tracker.Dto
{
    public class DepartmentDto: BaseEntityDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }

    public class DepartmentValidation : AbstractValidator<DepartmentDto>
    {
        public DepartmentValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The department name must be specified");
        }
    }

}
