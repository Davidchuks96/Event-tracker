using E_Tracker.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.ItemRepository
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ItemRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<(string Message, bool Successful)> CreateItemAsync(Item item)
        {
            if (item is null) return await Task.FromResult(("Please provide the Item to be created", false));
           // item.IsActive = true;
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
            return ($"{item.Name} has been created successfully", true);
        }
        public async Task<(string Message, bool Successful)> ActivateItemAsync(Item item)
        {
            if (item is null) return await Task.FromResult(("Please provide the Item to be activate", false));
            item.IsActive = true;
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{item.Name} has been activated successfully", true);
        }
        public async Task<(string Message, bool Successful)> DeleteItemAsync(Item item)
        {
            if (item is null) return await Task.FromResult(("Please provide the Item to be deleted", false));
            item.IsActive = false;
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{item.Name} has been deleted successfully", true);
        }

        public async Task<IEnumerable<Item>> GetAllApprovedItemsAsync()
        {
            var items = await _context.Items.Where(x => x.IsApproved == true && x.IsActive == true)
                .Include(x => x.ItemType).Include(x => x.ItemGroup).ThenInclude(x => x.Department).Include(x => x.ApprovedBy).ToListAsync();
            items.OrderByDescending(x => x.ExpiredDate);
            return items;
        }

        public async Task<IEnumerable<Item>> GetAllApprovedItemsByDateAsync(DateTime startDate, DateTime endDate)
        {
            var items = await _context.Items.Where(x => x.ExpiredDate >= startDate && x.ExpiredDate <= endDate && x.IsApproved == true && x.IsActive == true).Include(x => x.ItemType).Include(x => x.ApprovedBy).Include(x => x.ItemGroup).ThenInclude(x => x.Department).ToListAsync();
            items.OrderByDescending(x => x.ExpiredDate);
            return items;
        }

        public async Task<IEnumerable<Item>> GetApprovedItemsByDateItemTypeItemDeptAndCategoryAsync(DateTime startDate, DateTime endDate, string itemTypeId, string categoryId, string itemDepartmentId, string userDepartmentId = null)
        {
            IEnumerable<Item> items = new List<Item>(); //await GetApprovedItemsByDepartmentIdByCategoryIdAsync(itemDepartmentId, categoryId);
            if (userDepartmentId != null && userDepartmentId != "0")
            {

                items = await GetItemsByCreatedByDepartmentIdAsync(userDepartmentId);

                /// The above query already filtered out the non-active Items
                items = items.Where(x => x.ExpiredDate.Date >= startDate.Date 
                                    && x.ExpiredDate.Date <= endDate.Date 
                                    && x.IsApproved);
            }
            else
            {
                items = _context.Items.Where(x => x.ExpiredDate.Date >= startDate.Date && x.ExpiredDate.Date <= endDate.Date
                                                       && x.IsApproved && x.IsActive)
                                                     //.Include(x => x.Category)
                                                     .Include(x => x.ItemType)
                                                     .Include(x => x.ItemGroup)
                                                     .ThenInclude(x => x.Department)
                                                     .Include(x => x.ApprovedBy).ToList();
                //.Include(x => x.Department);
            }

            if (categoryId != null && categoryId != "0")
            {
                items = items.Where(x => x.ItemGroup.CategoryId == categoryId);
            }
            if (itemTypeId != null && itemTypeId != "0")
            {
                items = items.Where(x => x.ItemTypeId == itemTypeId);
            }
            if (itemDepartmentId != null && itemDepartmentId != "0")
            {
                items = items.Where(x => x.ItemGroup.DepartmentId == itemDepartmentId);
            }
            return items.OrderByDescending(x => x.ExpiredDate);
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            var items = await _context.Items.Where(x => x.IsActive == true).Include(x => x.ItemType).Include(x => x.ItemGroup).ThenInclude(x => x.Department).ToListAsync();
            items.OrderBy(x => x.Name);
            return items;
        }

        public async Task<IEnumerable<Item>> GetAllExpiredItemsAsync()
        {
            var items = await _context.Items.Where(x => x.IsActive == true && x.ExpiredDate.Date <= DateTime.Now.Date).Include(x => x.ItemType).Include(x => x.ItemGroup).ThenInclude(x => x.Department).ToListAsync();
            items.OrderBy(x => x.Name);
            return items;
        }

        public async Task<IEnumerable<Item>> GetAllNotActiveItemsAsync()
        {
            var items = await _context.Items.Where(x => x.IsActive == false).Include(x => x.ItemType).ToListAsync();
            items.OrderBy(x => x.Name);
            return items;
        }

        public async Task<IEnumerable<Item>> GetAllItemsByDateAsync(DateTime startDate, DateTime endDate)
        {
            var items = await _context.Items.Where(x=> x.ExpiredDate >= startDate && x.ExpiredDate <= endDate && x.IsActive == true).Include(x => x.ItemType).Include(x => x.ItemGroup).ThenInclude(x => x.Department).ToListAsync();
            items.OrderByDescending(x => x.ExpiredDate);
            return items;

        }

        public async Task<IEnumerable<Item>> GetAllNotApprovedItemsAsync()
        {
            var items = await _context.Items.Where(x => x.IsApproved == false && x.IsActive == true).Include(x => x.ItemType).Include(x => x.ItemGroup).ThenInclude(x => x.Department).ToListAsync();
            items.OrderByDescending(x => x.ExpiredDate).ThenBy(x => x.Name);
            return items;
        }

        public async Task<Item> GetItemByIdAsync(string itemId)
        {
            var item = await _context.Items.Where(x => x.Id == itemId)
                .Include(x => x.ItemType)
                .Include(x => x.ItemGroup)
                .ThenInclude(x => x.Department).FirstOrDefaultAsync();
            return item;
        }
        
        public async Task<IEnumerable<Item>> GetItemsByItemGroupIdAsync(string itemGroupId)
        {
            var items = await _context.Items.Where(x => x.ItemGroupId == itemGroupId)
                .Include(x => x.ItemGroup).ThenInclude(x => x.Department).Include(x => x.ItemType)
                .ToListAsync();
            return items;
        }

        public async Task<Item> GetItemByTagNoAsync(string tagNo)
        {
            var item =   await _context.Items.Where(x => x.TagNo == tagNo && x.IsActive == true).Include(x => x.ItemType).Include(x => x.ItemGroup).ThenInclude(x => x.Department).FirstOrDefaultAsync();
            return item;
        }

        public async Task<IEnumerable<Item>> GetApprovedItemsByDepartmentIdAsync(string departmentId)
        {
            if (departmentId == null) return new List<Item>();
            var dept = await _context.Departments.Where(x => x.Id == departmentId).Include(x => x.ItemGroups).FirstOrDefaultAsync();
            var itemGroups = dept.ItemGroups.Where(x => x.IsActive == true && x.IsApproved == true);
            var items = new List<Item>();
            foreach (var itemGroup in itemGroups)
            {

                //Adding list of items created by user to items
                var smallItems = await _context.Items.Where(x => x.ItemGroup.Id == itemGroup.Id && x.IsActive && x.IsApproved)
                                                     .Include(x => x.ItemGroup)
                                                     .ThenInclude(x => x.Department)
                                                     .Include(x => x.ItemType)
                                                     .Include(x => x.ApprovedBy)
                                                     .ToListAsync();
                items.AddRange(smallItems);
            }


            items.OrderBy(x => x.Name);
            return items;
        }
        
        public async Task<IEnumerable<Item>> GetApprovedItemsByDepartmentIdByCategoryIdAsync(string departmentId, string categoryId)
        {
            //if (departmentId = null) return new List<Item>();
            IQueryable<ItemGroup> itemGroups = _context.ItemGroups.Where(x => x.IsActive == true
                                                                    && x.IsApproved == true);
            //Check if it is not no selection and it is not ALL Item Departments selection
            if (departmentId != null && departmentId != "0")
            {
                itemGroups = itemGroups.Where(x => x.DepartmentId == departmentId);
            }

            //Check if it is not no selection and it is not ALL Categories selection
            if (categoryId != null && categoryId != "0")
            {
                itemGroups = itemGroups.Where(x => x.CategoryId == categoryId);
            }
            var items = new List<Item>(); 
            foreach (var itemGroup in itemGroups)
            {

                //Adding list of items created by user to items
                var smallItems = await _context.Items.Where(x => x.ItemGroup.Id == itemGroup.Id)
                                                     .Include(x => x.ItemGroup)
                                                     .ThenInclude(x => x.Department)
                                                     .Include(x => x.ItemType)
                                                     .Include(x => x.ApprovedBy)
                                                     .ToListAsync();
                items.AddRange(smallItems);
            }
            items.OrderBy(x => x.Name);
            return items;
        }

        public async Task<(string Message, bool Successful)> UpdateItemAsync(Item item)
        {
            if (item is null) return await Task.FromResult(("Please provide the Item to be updated", false));
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{item.Name} has been updated successfully", true);
        }

        
        public async Task<IEnumerable<Item>> GetItemsByCreatedByUserIdAsync(string userId)
        {
            var items = await _context.Items.Where(x => x.IsActive == true && x.CreatedById == userId).Include(x => x.ItemGroup).ThenInclude(x => x.Department)/*.Include(x => x.Category)*/.Include(x => x.ItemType)/*.Include(x => x.Department)*/.ToListAsync();
            items.OrderBy/*(x => x.Department.Name).ThenBy*/(x => x.Name);
            return items;
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryIdAsync(string categoryId)
        {
            var items = new List<Item>();
            var itemGroups = _context.ItemGroups.Where(x => x.CategoryId == categoryId && x.IsActive == true && x.IsApproved == true);
            foreach (var itemGroup in itemGroups)
            {

                //Adding list of items created by user to items
                var smallItems = await _context.Items.Where(x => x.ItemGroup.Id == itemGroup.Id)
                                                     .Include(x => x.ItemGroup)
                                                     .ThenInclude(x => x.Department)
                                                     .Include(x => x.ItemType)
                                                     .Include(x => x.ApprovedBy)
                                                     .ToListAsync();
                items.AddRange(smallItems);
            }
            return items;
        }

        /// <summary>
        /// Get all approved items created by a given departmentId
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="categoryId">Optional CategoryId parameter</param>
        /// <returns></returns>
        public async Task<IEnumerable<Item>> GetItemsByCreatedByDepartmentIdAsync(string departmentId, string categoryId = null)
        {
            if (departmentId == null) return new List<Item>();
            
            //CreatedByMyDepartment
            var dept = await _context.Departments.Where(x => x.Id == departmentId).Include(x => x.Users).FirstOrDefaultAsync();
            var items = new List<Item>();
            foreach (var user in dept.Users)
            {
                var itemGroups = new List<ItemGroup>();
                if (categoryId == null)
                {
                    itemGroups = _context.ItemGroups?.Where(x => x.CreatedById == user.Id && x.IsActive == true /*&& x.IsApproved == true*/).ToList();
                }
                //ByCategoryId
                else
                {
                    itemGroups = _context.ItemGroups.Where(x => x.CreatedById == user.Id && x.CategoryId == categoryId && x.IsActive == true /*&& x.IsApproved == true*/).ToList();

                }
                foreach (var itemGroup in itemGroups)
                {

                    //Adding list of items created by user to items
                    var smallItems = await _context.Items.Where(x => x.ItemGroup.Id == itemGroup.Id && x.IsActive)
                                                         .Include(x => x.ItemGroup)
                                                         .ThenInclude(x => x.Department)
                                                         .Include(x => x.ItemType)
                                                         .Include(x => x.ApprovedBy)
                                                         .ToListAsync();
                    items.AddRange(smallItems);
                }
            }

            items.OrderByDescending(x => x.DateCreated);
            return items;
        }

        public async Task DeleteItemsByItemGroupIdAsync(string itemGroupId)
        {
            var items = await GetItemsByItemGroupIdAsync(itemGroupId);

            foreach (var item in items)
            {
                item.IsActive = false;
                _context.Entry(item).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }

        public async Task ActivateItemsByItemGroupIdAsync(string itemGroupId)
        {
            var items = await GetItemsByItemGroupIdAsync(itemGroupId);

            foreach (var item in items)
            {
                item.IsActive = true;
                _context.Entry(item).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }
    }
}
