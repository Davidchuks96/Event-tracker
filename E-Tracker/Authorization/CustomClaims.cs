using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Authorization
{
    public class CustomClaims
    {
        public const string Permission = "permission";
    }
    public class CustomClaimsValues
    {
        public const string View = "View";
        public const string Delete = "Delete";
        public const string Update = "Updated";
        public const string Create = "Create";
        //public const string EditRole = "Edit Role";

        public const string CreateUser = "Create User";
        public const string UpdateUser = "Update User";
        public const string ViewUser = "View User";
        public const string DeleteUser = "Delete User";
        public const string ActivateUser = "Activate User";


        public const string CreateRole = "Create Role";
        public const string ViewRole = "View Role";
        public const string DeleteRole = "Delete Role";
        public const string EditRole = "Edit Role";
        public const string ActivateRole = "Activate Role";


        public const string CreatePermission = "Create Permission";
        public const string ViewPermission = "View Permission";
        public const string DeletePermission = "Delete Permission";
        public const string UpdatePermission = "Update Permission";
        public const string ActivatePermission = "Activate Permission";


        public const string CreateItemType = "Create ItemType";
        public const string ViewItemType = "View ItemType";
        public const string DeleteItemType = "Delete ItemType";
        public const string UpdateItemType = "Update ItemType";
        public const string ActivateItemType = "Activate ItemType";


        public const string CreateItem = "Create Item";
        public const string ViewItem = "View Item";
        public const string ViewAllItems = "View All Items";
        public const string DeleteItem = "Delete Item";
        public const string UpdateItem = "Update Item";
        public const string ActivateItem = "Activate Item";
        public const string ApproveItem = "Approve Item";
        
        public const string CreateItemGroup = "Create Item Group";
        public const string ViewItemGroup = "View Item Group";
        public const string ViewAllItemGroups = "View All Item Groups";
        public const string DeleteItemGroup = "Delete Item Group";
        public const string UpdateItemGroup = "Update Item Group";
        public const string ActivateItemGroup = "Activate Item Group";
        public const string ApproveItemGroup = "Approve Item Group";


        public const string CreateDepartment = "Create Department";
        public const string ViewDepartment = "View Department";
        public const string DeleteDepartment = "Delete Department";
        public const string UpdateDepartment = "Update Department";
        public const string ActivateDepartment = "Activate Department";


        public const string UpdateService = "Update Service";
        public const string ApproveService = "Approve Service";
        public const string ViewApprovedServices = "View Approved Services";
        public const string ViewServices = "View Services";
        public const string ViewAllServices = "View All Services";

        public const string CreateCategory = "Create Category";
        public const string ViewCategory = "View Category";
        public const string DeleteCategory = "Delete Category";
        public const string UpdateCategory = "Update Category";
        public const string ActivateCategory = "Activate Category";
    }
}
