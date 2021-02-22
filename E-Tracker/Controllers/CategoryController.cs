using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Tracker.Authorization;
using E_Tracker.CreateDto;
using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Dto;
using E_Tracker.Repository.CategoryRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<User> _userManager;

        public CategoryController(ILogger<ItemController> logger, IMapper mapper, ICategoryRepository categoryRepository, UserManager<User> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }
        private List<CategoryDto> CategoryListDto(IEnumerable<Category> categories)
        {
            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                var map = _mapper.Map<CategoryDto>(category);
                categoriesDto.Add(map);
            }

            return categoriesDto;
        }

        // GET: /<controller>/
        public IActionResult Categories()
        {
            return View(new List<CategoryDto>());
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewCategory)]
        public async Task<IActionResult> AllCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var categoriesDto =  CategoryListDto(categories);
            
            return View("Categories", categoriesDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateCategory)]
        public async Task<IActionResult> ActivateCategory(string categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category is null)
            {
                var msg = $"Category with ID:{categoryId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllNotActiveCatehories");
            }

            var result = await _categoryRepository.ActivateCategoryAsync(category);
            if (result.Successful)
            {
                Alert(result.Message, Notifications.success);

            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllNotActiveCategories");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateCategory)]
        public async Task<IActionResult> AllNotActiveCategories()
        {
            var categories = await _categoryRepository.GetAllNotActiveCategoriesAsync();
            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                var map = _mapper.Map<CategoryDto>(category);
                categoriesDto.Add(map);
            }
           
            return View("Categories", categoriesDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.CreateCategory)]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.CreateCategory)]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(categoryDto);
                }
                var categorycheck = await _categoryRepository.GetCategoryByNameAsync(categoryDto.Name);
                if (categorycheck == null)
                {
                    var category = _mapper.Map<Category>(categoryDto);
                    var user = await GetCurrentUser();
                    category.CreatedById = user.Id;
                    var result = await _categoryRepository.CreateCategoryAsync(category);
                    if (result.Successful)
                    {
                        Alert(result.Message, Notifications.success);
                    }
                    else
                    {
                        Alert(result.Message, Notifications.error);
                    }
                }
                else
                {
                    Alert($"Category: {categoryDto.Name} already exists", Notifications.error);
                    return View(categoryDto);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
              
            return RedirectToAction("AllCategories");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.UpdateCategory)]
        public async Task<IActionResult> UpdateCategory(string categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category is null)
            {
                var msg = $"Category with ID:{categoryId} does not exist";
                Alert(msg, Notifications.error);
                //TempData["msg"] = $"<script>Swal.fire('Failed!','Item with ID:{itemId} does not exist','error');</script>";
                return RedirectToAction("AllCategories");
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            
            return View(categoryDto);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.UpdateCategory)]
        public async Task<IActionResult> UpdateCategory(Category categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            var user = await GetCurrentUser();
            category.UpdatedById = user.Id;
            var result = await _categoryRepository.UpdateCategoryAsync(category);
            if (result.Successful)
            {
                Alert(result.Message, Notifications.success);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllCategories");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.DeleteCategory)]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category is null)
            {
                var msg = $"Category with ID:{categoryId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllCategories");
            }

            var user = await GetCurrentUser();
            category.DeletedById = user.Id;
            var result = await _categoryRepository.DeleteCategoryAsync(category);


            if (result.Successful)
            {
                Alert(result.Message, Notifications.success);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllCategories");

        }
        private async Task<User> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }
    }
}
