using E_Tracker.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(string Message, bool Successful)> ActivateCategoryAsync(Category category)
        {
            if (category is null) return await Task.FromResult(("Please provide the Category to be activate", false));
            category.IsActive = true;
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{category.Name} has been activated successfully", true);
        }

        public async Task<(string Message, bool Successful)> CreateCategoryAsync(Category category)
        {
            if (category is null) return await Task.FromResult(("Please provide the Item to be created", false));
            // item.IsActive = true;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return ($"{category.Name} has been created successfully", true);
        }

        public async Task<(string Message, bool Successful)> DeleteCategoryAsync(Category category)
        {
            if (category is null) return await Task.FromResult(("Please provide the Category to be deleted", false));
            category.IsActive = false;
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{category.Name} has been deleted successfully", true);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.Where(x => x.IsActive == true).ToListAsync();
            categories.OrderBy(x => x.Name);
            return categories;
        }

        public async Task<IEnumerable<Category>> GetAllNotActiveCategoriesAsync()
        {
            var categories = await _context.Categories.Where(x => x.IsActive == false).ToListAsync();
            categories.OrderBy(x => x.Name);
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(string categoryId)
        {
            var category = await _context.Categories.Where(x => x.Id == categoryId).FirstOrDefaultAsync();
            return category;
        }

        public async Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            var category = await _context.Categories.Where(x => x.Name == categoryName && x.IsActive == true).FirstOrDefaultAsync();
            return category;
        }

        public async Task<(string Message, bool Successful)> UpdateCategoryAsync(Category category)
        {
            if (category is null) return await Task.FromResult(("Please provide the Category to be updated", false));
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{category.Name} has been updated successfully", true);
        }
    }
}
