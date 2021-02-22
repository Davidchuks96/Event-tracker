using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Tracker.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Tracker.Repository.ItemTypeRepository
{
    public class ItemTypeRepository : IItemTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<(string Message, bool Successful)> CreateItemTypeAsync(ItemType itemType)
        {
            if (itemType is null) return await Task.FromResult(("Please provide the Item type to be created", false));
            //itemType.IsActive = true;
            await _context.ItemTypes.AddAsync(itemType);
            await _context.SaveChangesAsync();
            return ($"{itemType.Name} has been created successfully", true);
        }
        public async Task<(string Message, bool Successful)> ActivateItemTypeAsync(ItemType itemType)
        {
            if (itemType is null) return await Task.FromResult(("Please provide the Item type to be activated", false));
            itemType.IsActive = true;
            _context.Entry(itemType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{itemType.Name} has been activated successfully", true);
        }
        public async Task<(string Message, bool Successful)> DeleteItemTypeAsync(ItemType itemType)
        {
            if (itemType is null) return await Task.FromResult(("Please provide the Item type to be deleted", false));
            itemType.IsActive = false;
            _context.Entry(itemType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{itemType.Name} has been deleted successfully", true);
        }

        public async Task<IEnumerable<ItemType>> GetAllItemTypesAsync()
        {
           var types = await _context.ItemTypes.Where(x => x.IsActive == true).ToListAsync(); 
            return types;
        }

        public async Task<IEnumerable<ItemType>> GetAllNotActiveItemTypesAsync()
        {
            var types = await _context.ItemTypes.Where(x => x.IsActive == false).ToListAsync();
            return types;
        }

        public async Task<ItemType> GetItemTypeByIdAsync(string itemTypeId)
        {
            var type = await _context.ItemTypes.FindAsync(itemTypeId);
            return type;
        }

        public async Task<ItemType> GetItemTypeByNameAsync(string itemName)
        {
            var type = await _context.ItemTypes.Where(x => x.Name == itemName).FirstOrDefaultAsync();
            return type;
        }

        public async Task<(string Message, bool Successful)> UpdateItemTypeAsync(ItemType itemType)
        {
            if (itemType is null) return await Task.FromResult(("Please provide the Item type to be updated", false));
            _context.Entry(itemType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{itemType.Name} has been updated successfully", true);
        }
    }
}
