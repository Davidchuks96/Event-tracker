using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.ItemRepository
{
    public interface IItemRepository
    {
        Task<(string Message, bool Successful)> CreateItemAsync(Item item);
        Task<(string Message, bool Successful)> UpdateItemAsync(Item item);
        Task<(string Message, bool Successful)> DeleteItemAsync(Item item);
        Task<(string Message, bool Successful)> ActivateItemAsync(Item item);
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<IEnumerable<Item>> GetAllNotActiveItemsAsync();
        Task<IEnumerable<Item>> GetAllApprovedItemsAsync();
        Task<IEnumerable<Item>> GetAllNotApprovedItemsAsync();
        Task<IEnumerable<Item>> GetAllItemsByDateAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Item>> GetAllApprovedItemsByDateAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Item>> GetApprovedItemsByDateItemTypeItemDeptAndCategoryAsync(DateTime startDate, DateTime endDate, string itemTypeId, string categoryId, string itemDepartmentId, string userDepartmentId = null);
        Task<IEnumerable<Item>> GetApprovedItemsByDepartmentIdAsync(string departmentId);
        Task<IEnumerable<Item>> GetItemsByCreatedByDepartmentIdAsync(string departmentId, string categoryId = null);
        Task<IEnumerable<Item>> GetItemsByCreatedByUserIdAsync(string userId);
        Task<IEnumerable<Item>> GetAllExpiredItemsAsync();
        Task<Item> GetItemByIdAsync(string itemId);
        Task<Item> GetItemByTagNoAsync(string tagNo);
        Task<IEnumerable<Item>> GetItemsByCategoryIdAsync(string categoryId);
        Task<IEnumerable<Item>> GetItemsByItemGroupIdAsync(string itemGroupId);
        Task<IEnumerable<Item>> GetApprovedItemsByDepartmentIdByCategoryIdAsync(string departmentId, string categoryId);
        Task DeleteItemsByItemGroupIdAsync(string itemGroupId);
        Task ActivateItemsByItemGroupIdAsync(string id);
    }
}
