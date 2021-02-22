using System.Collections.Generic;
using System.Security.Claims;

namespace E_Tracker.Authorization
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim(CustomClaims.Permission, CustomClaimsValues.Create),

            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateUser),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ActivateUser),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateUser),
            new Claim(CustomClaims.Permission, CustomClaimsValues.DeleteUser),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewUser),
            
            new Claim(CustomClaims.Permission, CustomClaimsValues.DeleteRole),
            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateRole),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewRole),
            new Claim(CustomClaims.Permission, CustomClaimsValues.EditRole),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ActivateRole),

            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.DeleteItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewAllItems),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ActivateItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ApproveItem),
            
            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewAllItemGroups),
            new Claim(CustomClaims.Permission, CustomClaimsValues.DeleteItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ActivateItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ApproveItemGroup),

            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateItemType),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewItemType),
            new Claim(CustomClaims.Permission, CustomClaimsValues.DeleteItemType),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateItemType),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ActivateItemType),

            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateDepartment),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateDepartment),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewDepartment),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ActivateDepartment),
            new Claim(CustomClaims.Permission, CustomClaimsValues.DeleteDepartment),

            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateCategory),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateCategory),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewCategory),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ActivateCategory),
            new Claim(CustomClaims.Permission, CustomClaimsValues.DeleteCategory),

            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateService),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ApproveService),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewApprovedServices),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewServices),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewAllServices)
        };

        public static List<Claim> UserClaims = new List<Claim>()
        {
            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewItem),

            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewItemGroup),

            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewCategory),

            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateService),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewApprovedServices),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewServices)
        };

        public static List<Claim> AdminClaims = new List<Claim>()
        {
            //Can only view the list of users in his/her department
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewUser),

            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.DeleteItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ActivateItem),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ApproveItem),

            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.DeleteItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ActivateItemGroup),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ApproveItemGroup),

            new Claim(CustomClaims.Permission, CustomClaimsValues.CreateCategory),
            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateCategory),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewCategory),

            new Claim(CustomClaims.Permission, CustomClaimsValues.UpdateService),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ApproveService),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewApprovedServices),
            new Claim(CustomClaims.Permission, CustomClaimsValues.ViewServices)
        };
    }

}
