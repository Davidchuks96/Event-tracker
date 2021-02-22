using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.ItemTypeRepository
{
    public interface IItemTypeRepository
    {
        Task<(string Message, bool Successful)> CreateItemTypeAsync(ItemType itemType);
        Task<(string Message, bool Successful)> UpdateItemTypeAsync(ItemType itemType);
        Task<(string Message, bool Successful)> DeleteItemTypeAsync(ItemType itemType);
        Task<(string Message, bool Successful)> ActivateItemTypeAsync(ItemType itemType);
        Task<IEnumerable<ItemType>> GetAllItemTypesAsync();
        Task<IEnumerable<ItemType>> GetAllNotActiveItemTypesAsync();
        Task<ItemType> GetItemTypeByIdAsync(string itemTypeId);
        Task<ItemType> GetItemTypeByNameAsync(string itemName);

    }
}
