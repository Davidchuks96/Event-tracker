using E_Tracker.Data;
using E_Tracker.Data.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class ServiceDto:BaseEntityDto
    {
        public string Id { get; set; }
        public string ServiceDepartmentId { get; set; }
        public DepartmentDto ServiceDepartment { get; set; }
        public string ItemId { get; set; }
        public ItemDto Item { get; set; }
        [Display(Name ="Service Approved")]
        public bool IsServiceApproved { get; set; }
        public string Status { get; set; }
        public string ApproveOrRejectComments { get; set; }
        public UserDto ServiceApprovedBy { get; set; }
        public string ServiceApprovedById { get; set; }
        public DateTime DateServiced { get; set; }
        public DateTime NewExpiryDate { get; set; }
        public string Comments { get; set; }
        public bool IsANewCycle { get; set; }
        public bool IsANewReoccurenceFrequency { get; set; }
        public int NewReoccurenceValue { get; set; }
        public ReoccurenceFrequency NewReoccurenceFrequency { get; set; }
    }
    public class ServiceValidation : AbstractValidator<ServiceDto>
    {
        public ServiceValidation()
        {
            RuleFor(x => x.ServiceDepartmentId).NotEmpty().WithMessage("The Department of Service must be specified");
            RuleFor(x => x.ItemId).NotEmpty().WithMessage("The Item must be specified");
            RuleFor(x => x.ServiceApprovedById).NotEmpty().WithMessage("Who Approved the Service must be specified");
            RuleFor(x => x.DateServiced).NotEmpty().WithMessage("The DateServiced must be specified");
            RuleFor(x => x.NewExpiryDate).NotEmpty().WithMessage("The NewExpiryDate must be specified");
        }
    }
}
