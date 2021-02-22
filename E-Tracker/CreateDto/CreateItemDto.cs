using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.CreateDto
{
    public class CreateItemDto
    {

        public string Name { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemGroupId { get; set; }
        //For returnUrl stuff for create view
        public string ReturnUrl { get; set; }
        public ItemTypeDto ItemType { get; set; }
        //public string CategoryId { get; set; }
        //public CategoryDto Category { get; set; }
        //public string DepartmentId { get; set; }
        //public DepartmentDto Department { get; set; }
        public DateTime ExpiredDate { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; } = "Pending";
        public string TagNo { get; set; }
        public User ApprovedBy { get; set; }
        public string ApprovedById { get; set; }
        public int ReoccurenceValue { get; set; }
        public ReoccurenceFrequency ReoccurenceFrequency { get; set; }
        public ReoccurenceFrequency NotificationPeriod { get; set; }
        public NotificationFrequency NotificationFrequency { get; set; }
    }

    public class ItemValidation : AbstractValidator<CreateItemDto>
    {
        public ItemValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The Item name must be specified");
            RuleFor(x => x.TagNo).NotEmpty().WithMessage("The Item Tag No must be specified");
            RuleFor(x => x.ItemTypeId).NotEmpty().WithMessage("The Item Type must be specified");
            RuleFor(x => x.ItemGroupId).NotEmpty().WithMessage("The Item Group must be specified");
            //RuleFor(x => x.CategoryId).NotEmpty().WithMessage("The Category must be specified");
            //RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("The Department of the item must be specified");
            RuleFor(x => x.ExpiredDate).NotEmpty().WithMessage("The Expiry date of the item must be specified");
            RuleFor(x => x.ReoccurenceValue).GreaterThan(0).WithMessage("Reoccurence value cannot be zero");
        }
    }
}
