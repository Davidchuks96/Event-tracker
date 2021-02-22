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
    public class ItemDto : BaseEntityDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemGroupId { get; set; }
        public string ReturnUrl { get; set; }
        public ItemGroupDto ItemGroup { get; set; }
        public ItemTypeDto ItemType { get; set; }
        //public string CategoryId { get; set; }
        //public CategoryDto Category { get; set; }
        //public string DepartmentId { get; set; }
        //public DepartmentDto Department { get; set; }
        public DateTime ExpiredDate { get; set; }
        [Display(Name ="Approved")]
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public string ApproveOrRejectComments { get; set; }
        public string TagNo { get; set; }

        [Display(Name = "Approved By")]
        public UserDto ApprovedBy { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedById { get; set; }

        [Display(Name = "Reoccurence Value")]
        public int ReoccurenceValue { get; set; }

        [Display(Name = "Reoccurence Frequency")]
        public ReoccurenceFrequency ReoccurenceFrequency { get; set; }

        [Display(Name = "Notification Period")]
        public ReoccurenceFrequency NotificationPeriod { get; set; }

        [Display(Name = "Notification Frequency")]
        public NotificationFrequency NotificationFrequency { get; set; }
    }

    public class ItemValidation : AbstractValidator<ItemDto>
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
        }
    }
}
