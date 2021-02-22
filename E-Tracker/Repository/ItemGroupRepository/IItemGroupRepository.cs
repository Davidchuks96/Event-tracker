using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Tracker.Repository.ItemGroupRepository
{
    public interface IItemGroupRepository
    {
        Task<(string Message, bool Successful)> CreateItemGroupAsync(ItemGroup itemGroup);
        Task<(string Message, bool Successful)> UpdateItemGroupAsync(ItemGroup itemGroup);
        Task<(string Message, bool Successful)> DeleteItemGroupAsync(ItemGroup itemGroup);
        Task<(string Message, bool Successful)> ActivateItemGroupAsync(ItemGroup itemGroup);
        Task<IEnumerable<ItemGroup>> GetAllItemGroupsAsync();
        Task<IEnumerable<ItemGroup>> GetAllApprovedItemGroupsAsync();
        Task<IEnumerable<ItemGroup>> GetAllNotApprovedItemGroupsAsync();
        Task<IEnumerable<ItemGroup>> GetAllNotActiveItemGroupsAsync();
        Task<IEnumerable<ItemGroup>> GetAllApprovedItemGroupsByItemGroupDeptAndCategoryAsync(string categoryId, string itemDepartmentId, string userDepartmentId = null);
        Task<IEnumerable<ItemGroup>> GetApprovedItemGroupsByCategoryIdAsync(string categoryId);
        Task<IEnumerable<ItemGroup>> GetApprovedItemGroupsByDepartmentIdAsync(string departmentId);
        Task<ItemGroup> GetItemGroupByIdAsync(string itemId);
        Task<ItemGroup> GetItemGroupByTagNoAsync(string tagNo);
        Task<IEnumerable<ItemGroup>> GetItemGroupsByCreatedByDepartmentIdAsync(string departmentId);
        Task<IEnumerable<ItemGroup>> GetItemGroupsByCreatedByUserIdAsync(string userId);
        Task<IEnumerable<ItemGroup>> GetItemGroupsByMyDepartmentCategoryIdAsync(string departmentId, string categoryId);
        Task<IEnumerable<ItemGroup>> GetAllActiveItemGroupsByCategoryIdAsync(string categoryId);
    }
}