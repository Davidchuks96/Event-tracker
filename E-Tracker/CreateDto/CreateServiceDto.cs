using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.CreateDto
{
    public class CreateServiceDto: BaseEntityDto
    {      
            public string ServiceDepartmentId { get; set; }
            public Department ServiceDepartment { get; set; }
            public string ItemId { get; set; }
            public Item Item { get; set; }
            public bool IsServiceApproved { get; set; }
            public string Status { get; set; } = "Pending";
            public User ServiceApprovedBy { get; set; }
            public string ServiceApprovedById { get; set; }
            public DateTime DateServiced { get; set; }
            public DateTime NewExpiryDate { get; set; }
            public string Comments { get; set; }
            public bool IsANewCycle { get; set; }
            public bool IsANewReoccurenceFrequency { get; set; }
            public int NewReoccurenceValue { get; set; }
            public ReoccurenceFrequency NewReoccurenceFrequency { get; set; }

        public class ServiceValidation : AbstractValidator<CreateServiceDto>
        {
            public ServiceValidation()
            {
                RuleFor(x => x.ServiceDepartmentId).NotEmpty().WithMessage("The Department of Service must be specified");
                RuleFor(x => x.ItemId).NotEmpty().WithMessage("The Item must be specified");
                RuleFor(x => x.DateServiced).NotEmpty().WithMessage("The Date serviced must be specified");
                RuleFor(x => x.DateServiced).Must(x => { return x.Date <= DateTime.Now.Date; }).WithMessage("The date serviced should be on or before today's date.");
                RuleFor(x => x.Comments).NotEmpty().WithMessage("Comments on Service must be specified");
                //RuleFor(x => x.NewExpiryDate).NotEmpty().WithMessage("The New expiry date must be specified");
                //RuleFor(x => x.NewExpiryDate).Must(x => { return x.Date > DateTime.Now.Date; }).WithMessage("The new date of expiry should be after today's date.");
            }
        }
    }
}
