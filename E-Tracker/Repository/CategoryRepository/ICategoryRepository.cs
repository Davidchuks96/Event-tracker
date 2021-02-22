using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<(string Message, bool Successful)> CreateCategoryAsync(Category category);
        Task<(string Message, bool Successful)> UpdateCategoryAsync(Category category);
        Task<(string Message, bool Successful)> DeleteCategoryAsync(Category category);
        Task<(string Message, bool Successful)> ActivateCategoryAsync(Category category);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<Category>> GetAllNotActiveCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(string categoryId);
        Task<Category> GetCategoryByNameAsync(string categoryName);

    }
}
