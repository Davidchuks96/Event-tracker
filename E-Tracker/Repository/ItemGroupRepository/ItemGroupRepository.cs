using E_Tracker.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.ItemGroupRepository
{
    public class ItemGroupRepository : IItemGroupRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ItemGroupRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<(string Message, bool Successful)> CreateItemGroupAsync(ItemGroup itemGroup)
        {
            if (itemGroup is null) return await Task.FromResult(("Please provide the Item Group to be created", false));
            await _context.ItemGroups.AddAsync(itemGroup);
            await _context.SaveChangesAsync();
            return ($"{itemGroup.Name} has been created successfully", true);
        }
        public async Task<(string Message, bool Successful)> ActivateItemGroupAsync(ItemGroup itemGroup)
        {
            if (itemGroup is null) return await Task.FromResult(("Please provide the Item Group to be activate", false));
            itemGroup.IsActive = true;
            _context.Entry(itemGroup).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{itemGroup.Name} has been activated successfully", true);
        }
        public async Task<(string Message, bool Successful)> DeleteItemGroupAsync(ItemGroup itemGroup)
        {
            if (itemGroup is null) return await Task.FromResult(("Please provide the Item Group to be deleted", false));
            itemGroup.IsActive = false;
            _context.Entry(itemGroup).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{itemGroup.Name} has been deleted successfully", true);
        }

        public async Task<IEnumerable<ItemGroup>> GetAllApprovedItemGroupsAsync()
        {
            var itemgroups = await _context.ItemGroups.Where(x => x.IsApproved == true && x.IsActive == true).Include(x => x.Category).Include(x => x.Department).Include(x => x.ApprovedBy).ToListAsync();
            itemgroups.OrderBy(x => x.Department.Name).ThenBy(x => x.Name);
            return itemgroups;
        }

        public async Task<IEnumerable<ItemGroup>> GetAllApprovedItemGroupsByItemGroupDeptAndCategoryAsync(string categoryId, string itemDepartmentId, string userDepartmentId = null)
        {
            IEnumerable<ItemGroup> itemGroups;
            if (userDepartmentId != null)
            {

                itemGroups = await GetItemGroupsByCreatedByDepartmentIdAsync(userDepartmentId);

                /// The above query already filtered out the non-active Items
                itemGroups = itemGroups.Where(x => x.IsApproved);
            }
            else
            {
                itemGroups = _context.ItemGroups.Where(x => x.IsApproved && x.IsActive)
                                                     .Include(x => x.Category)
                                                     .Include(x => x.ApprovedBy)
                                                     .Include(x => x.Department);
            }

            if (categoryId != null)
            {
                itemGroups = itemGroups.Where(x => x.CategoryId == categoryId);
            }
            if (itemDepartmentId != null)
            {
                itemGroups = itemGroups.Where(x => x.DepartmentId == itemDepartmentId);
            }
            return itemGroups;
        }

        public async Task<IEnumerable<ItemGroup>> GetAllItemGroupsAsync()
        {
            var itemGroups = await _context.ItemGroups.Where(x => x.IsActive == true).Include(x => x.Category).Include(x => x.Department).ToListAsync();
            itemGroups.OrderBy(x => x.Department.Name).ThenBy(x => x.Name);
            return itemGroups;
        }

        public async Task<IEnumerable<ItemGroup>> GetAllNotActiveItemGroupsAsync()
        {
            var itemGroups = await _context.ItemGroups.Where(x => x.IsActive == false).Include(x => x.Category).Include(x => x.Department).ToListAsync();
            itemGroups.OrderBy(x => x.Department.Name).ThenBy(x => x.Name);
            return itemGroups;
        }

        public async Task<IEnumerable<ItemGroup>> GetAllNotApprovedItemGroupsAsync()
        {
            var itemGroups = await _context.ItemGroups.Where(x => x.IsApproved == false && x.IsActive == true).Include(x => x.Category).Include(x => x.Department).ToListAsync();
            itemGroups.OrderBy(x => x.Department.Name).ThenBy(x => x.Name);
            return itemGroups;
        }

        public async Task<ItemGroup> GetItemGroupByIdAsync(string itemGroupId)
        {
            var itemGroup = await _context.ItemGroups.Where(x => x.Id == itemGroupId)
                .Include(x => x.Category)
                .Include(x => x.Department).FirstOrDefaultAsync();
            return itemGroup;
        }

        public async Task<ItemGroup> GetItemGroupByTagNoAsync(string tagNo)
        {
            var itemGroup = await _context.ItemGroups.Where(x => x.TagNo == tagNo && x.IsActive == true).Include(x => x.Category).Include(x => x.Department).FirstOrDefaultAsync();
            return itemGroup;
        }

        public async Task<IEnumerable<ItemGroup>> GetApprovedItemGroupsByDepartmentIdAsync(string departmentId)
        {
            var itemGroups = await _context.ItemGroups.Where(x => x.DepartmentId == departmentId && x.IsActive == true && x.IsApproved == true).Include(x => x.Category).Include(x => x.Department).ToListAsync();
            itemGroups.OrderBy(x => x.Name);
            return itemGroups;
        }

        public async Task<(string Message, bool Successful)> UpdateItemGroupAsync(ItemGroup itemGroup)
        {
            if (itemGroup is null) return await Task.FromResult(("Please provide the Item Group to be updated", false));
            _context.Entry(itemGroup).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{itemGroup.Name} has been updated successfully", true);
        }

        public async Task<IEnumerable<ItemGroup>> GetItemGroupsByCreatedByDepartmentIdAsync(string departmentId/*,string userId*/)
        {
            if (departmentId == null) return new List<ItemGroup>();
            var dept = await _context.Departments.Where(x => x.Id == departmentId).Include(x => x.Users).FirstOrDefaultAsync();
            var itemGroups = new List<ItemGroup>();
            foreach (var user in dept.Users)
            {

                //Adding list of items created by user to items
                var smallItems = await _context.ItemGroups.Where(x => x.CreatedById == user.Id && x.IsActive == true)
                                                     .Include(x => x.Category)
                                                     .Include(x => x.ApprovedBy)
                                                     .Include(x => x.Department).ToListAsync();
                itemGroups.AddRange(smallItems);
            }

            itemGroups.OrderByDescending(x => x.DateCreated);
            return itemGroups;
        }

        public async Task<IEnumerable<ItemGroup>> GetItemGroupsByCreatedByUserIdAsync(string userId)
        {
            var itemGroups = await _context.ItemGroups.Where(x => x.IsActive == true && x.CreatedById == userId).Include(x => x.Category).Include(x => x.Department).ToListAsync();
            itemGroups.OrderBy(x => x.Department.Name).ThenBy(x => x.Name);
            return itemGroups;
        }

        public async Task<IEnumerable<ItemGroup>> GetAllActiveItemGroupsByCategoryIdAsync(string categoryId)
        {
            var itemGroups = await _context.ItemGroups.Where(x => x.CategoryId == categoryId && x.IsActive == true).Include(x => x.Category).Include(x => x.Department).ToListAsync();
            itemGroups.OrderBy(x => x.Name);
            return itemGroups;
        }
        public async Task<IEnumerable<ItemGroup>> GetApprovedItemGroupsByCategoryIdAsync(string categoryId)
        {
            var itemGroups = await _context.ItemGroups.Where(x => x.CategoryId == categoryId && x.IsActive == true && x.IsApproved == true).Include(x => x.Category).Include(x => x.Department).ToListAsync();
            itemGroups.OrderBy(x => x.Name);
            return itemGroups;
        }

        public async Task<IEnumerable<ItemGroup>> GetItemGroupsByMyDepartmentCategoryIdAsync(string departmentId, string categoryId)
        {
            if (departmentId == null) return new List<ItemGroup>();
            var dept = await _context.Departments.Where(x => x.Id == departmentId).Include(x => x.Users).FirstOrDefaultAsync();
            var itemGroups = new List<ItemGroup>();
            foreach (var user in dept.Users)
            {

                //Adding list of items created by user to items
                var smallItems = await _context.ItemGroups.Where(x => x.CreatedById == user.Id && x.CategoryId == categoryId && x.IsActive == true)
                                                     .Include(x => x.Category)
                                                     .Include(x => x.Department).ToListAsync();
                itemGroups.AddRange(smallItems);
            }

            itemGroups.OrderByDescending(x => x.DateCreated);
            return itemGroups;
        }
    }
}
