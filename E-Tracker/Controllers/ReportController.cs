using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using E_Tracker.Repository.ItemRepository;
using E_Tracker.Dto;
using E_Tracker.Data;
using Microsoft.AspNetCore.Identity;
using E_Tracker.Repository.ItemTypeRepository;
using E_Tracker.Repository.CategoryRepository;
using E_Tracker.Models;
using E_Tracker.Repository.ServiceRepo;
using E_Tracker.Repository.DepartmentRepo;
using E_Tracker.Authorization;
using E_Tracker.Extensions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    public class ReportController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly UserManager<User> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IMapper _mapper;
        private readonly IDepartmentRepository _departmentRepository;

        public ReportController(IItemRepository itemRepository,UserManager<User> userManager, ICategoryRepository categoryRepository,
                 IServiceRepository serviceRepository, IItemTypeRepository itemTypeRepository, IMapper mapper,
            IDepartmentRepository departmentRepository)
        {
            _itemRepository = itemRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _serviceRepository = serviceRepository;
            _itemTypeRepository = itemTypeRepository;
            _mapper = mapper;
            _departmentRepository = departmentRepository;
        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> ItemReport()
        {
            var dropdownLists = await DropDownLists();
            ViewBag.Categories = dropdownLists.categoriesList;
            ViewBag.ItemTypes = dropdownLists.itemTypesList;
            ViewBag.Departments = dropdownLists.departmentsList;
            var currentUser = await GetCurrentUser();
            var items = await _itemRepository.GetItemsByCreatedByDepartmentIdAsync(currentUser.DepartmentId);
            //Return only approved items
            items = items.Where(x => x.IsApproved);
            var itemDto = new List<ItemDto>();
            foreach (var item in items)
            {
                var map = _mapper.Map<ItemDto>(item);
                var itemCreator = await _userManager.FindByIdAsync(item.CreatedById);
                var itemCreatorDto = _mapper.Map<UserDto>(itemCreator);
                map.CreatedBy = itemCreatorDto;

                var itemApprovedBy = await _userManager.FindByIdAsync(item.ApprovedById);
                var itemApprovedByDto = _mapper.Map<UserDto>(itemApprovedBy);
                map.ApprovedBy = itemApprovedByDto;
                itemDto.Add(map);
            }
            return View(itemDto);
        }
        

        private async Task<(List<SelectListItem> categoriesList, List<SelectListItem> itemTypesList,  List<SelectListItem> departmentsList)> DropDownLists()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var itemTypes = await _itemTypeRepository.GetAllItemTypesAsync();
            var departments = await _departmentRepository.GetAllDepartmentAsync();
            var categoriesList = categories.Select(a => new SelectListItem()
            {
                Value = a.Id,
                Text = a.Name
            }).Distinct()
            .OrderBy(a => a.Value)
            .ToList();
            categoriesList.Add(new SelectListItem()
            {
                //Giving it a value of zero because ALL Categories is not recognized on Microsoft edge if Value is ""
                Value = "0",
                Text = "All Categories"
            });
            
            var itemTypesList = itemTypes.Select(a => new SelectListItem()
            {
                Value = a.Id,
                Text = a.Name
            }).Distinct()
            .OrderBy(a => a.Value)
            .ToList();

            itemTypesList.Add(new SelectListItem()
            {
                //Giving it a value of zero because ALL Item Types is not recognized on Microsoft edge if Value is ""
                Value = "0",
                Text = "All Item Types"
            });
            
            var departmentsList = departments.Select(a => new SelectListItem()
            {
                Value = a.Id,
                Text = a.Name
            }).Distinct()
            .OrderBy(a => a.Value)
            .ToList();

            departmentsList.Add(new SelectListItem()
            {
                //Giving it a value of zero because ALL Item Departments is not recognized on Microsoft edge if Value is ""
                Value = "0",
                Text = "All Item Departments"
            });

            return (categoriesList,itemTypesList, departmentsList);
        }

        //[HttpGet]
        //public async Task<IActionResult> ItemReport(SearchViewModel searchViewModel)
        //{
        //    IEnumerable<Item> items = new List<Item>();
        //    var dropdownLists = await DropDownLists();
        //    if (searchViewModel.CategoryId == null && searchViewModel.ItemTypeId == null)
        //    {
        //        items = await _itemRepository.GetAllApprovedItemsAsync();
        //    }
        //    else
        //    {
        //        items = await _itemRepository.GetAllApprovedItemsByDateItemTypeAndCategoryAsync(searchViewModel.StartDate, searchViewModel.EndDate, searchViewModel.ItemTypeId, searchViewModel.CategoryId);
                
        //        var selectedItemType = dropdownLists.Item2.SingleOrDefault(x => x.Value == searchViewModel.ItemTypeId);
        //        if (selectedItemType != null)
        //        {
        //            selectedItemType.Selected = true;
        //        }
        //        var selectedCategory = dropdownLists.Item2.SingleOrDefault(x => x.Value == searchViewModel.CategoryId);
        //        if (selectedCategory != null)
        //        {
        //            selectedCategory.Selected = true;
        //        }
        //    }
        //    ViewBag.SearchParameters = searchViewModel;
        //    ViewBag.Categories = dropdownLists.Item1;
        //    ViewBag.ItemTypes = dropdownLists.Item2;
            
        //    var itemDto = new List<ItemDto>();
        //    foreach (var item in items)
        //    {
        //        var map = _mapper.Map<ItemDto>(item);
        //        var itemCreator = await _userManager.FindByIdAsync(item.CreatedById);
        //        var itemCreatorDto = _mapper.Map<UserDto>(itemCreator);
        //        map.CreatedBy = itemCreatorDto;

        //        var itemApprovedBy = await _userManager.FindByIdAsync(item.ApprovedById);
        //        var itemApprovedByDto = _mapper.Map<UserDto>(itemApprovedBy);
        //        map.ApprovedBy = itemApprovedByDto;
        //        itemDto.Add(map);
        //    }
        //    return View(itemDto);
        //}
        
        [HttpPost]
        public async Task<IActionResult> ItemReport(SearchViewModel searchViewModel)
        {
            IEnumerable<Item> items = new List<Item>();
            var dropdownLists = await DropDownLists();

            var currentUser = await GetCurrentUser();
            var startDate = searchViewModel.StartDate.ToInvariantDateTime("dd/MM/yyyy");
            var endDate = searchViewModel.EndDate.ToInvariantDateTime("dd/MM/yyyy");
            items = await _itemRepository.GetApprovedItemsByDateItemTypeItemDeptAndCategoryAsync(startDate, endDate,
                searchViewModel.ItemTypeId, searchViewModel.CategoryId, searchViewModel.ItemDepartmentId,currentUser.DepartmentId);

            HighlightSelectedItem(dropdownLists,searchViewModel);
            
            ViewBag.SearchParameters = searchViewModel;
            ViewBag.Categories = dropdownLists.categoriesList;
            ViewBag.ItemTypes = dropdownLists.itemTypesList;
            ViewBag.Departments = dropdownLists.departmentsList;

            var itemDto = new List<ItemDto>();
            foreach (var item in items)
            {
                var map = _mapper.Map<ItemDto>(item);
                var itemCreator = await _userManager.FindByIdAsync(item.CreatedById);
                var itemCreatorDto = _mapper.Map<UserDto>(itemCreator);
                map.CreatedBy = itemCreatorDto;

                var itemApprovedBy = await _userManager.FindByIdAsync(item.ApprovedById);
                var itemApprovedByDto = _mapper.Map<UserDto>(itemApprovedBy);
                map.ApprovedBy = itemApprovedByDto;
                itemDto.Add(map);
            }
            return View(itemDto);
        }

        private void HighlightSelectedItem((List<SelectListItem> categoriesList, List<SelectListItem> itemTypesList, List<SelectListItem> departmentsList) dropdownLists, SearchViewModel searchViewModel)
        {
            //Item type list
            var selectedItemType = dropdownLists.itemTypesList.SingleOrDefault(x => x.Value == searchViewModel.ItemTypeId);
            if (selectedItemType != null)
            {
                selectedItemType.Selected = true;
                searchViewModel.ItemTypeName = selectedItemType.Text;
            }
            else
            {
                selectedItemType = dropdownLists.itemTypesList.SingleOrDefault(x => x.Text == "All Item Types");
                if (selectedItemType != null)
                    selectedItemType.Selected = true;
            }
            //Categories list
            var selectedCategory = dropdownLists.categoriesList.SingleOrDefault(x => x.Value == searchViewModel.CategoryId);
            if (selectedCategory != null)
            {
                selectedCategory.Selected = true;
                searchViewModel.CategoryName = selectedCategory.Text;
            }
            else
            {
                selectedCategory = dropdownLists.categoriesList.SingleOrDefault(x => x.Text == "All Categories");
                if (selectedCategory != null)
                    selectedCategory.Selected = true;
            }
            //Item department list
            var selectedItemDepartment = dropdownLists.departmentsList.SingleOrDefault(x => x.Value == searchViewModel.ItemDepartmentId);
            if (selectedItemDepartment != null)
            {
                selectedItemDepartment.Selected = true;
                searchViewModel.ItemDepartmentName = selectedItemDepartment.Text;
            }
            else
            {
                selectedItemDepartment = dropdownLists.departmentsList.SingleOrDefault(x => x.Text == "All Item Departments");
                if (selectedItemDepartment != null)
                    selectedItemDepartment.Selected = true;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ServiceReport()
        {
            var dropdownLists = await DropDownLists();
            ViewBag.Categories = dropdownLists.categoriesList;
            ViewBag.ItemTypes = dropdownLists.itemTypesList;
            ViewBag.Departments = dropdownLists.departmentsList;
            //If user is WITH ALL SERVICES PERMISION, DONT PASS IN A USER DEPARTMENT ID
            var currentUser = await GetCurrentUser();
            IEnumerable<Service> services = new List<Service>();
            if (User.HasClaim(CustomClaims.Permission, CustomClaimsValues.ViewAllServices))
            {
                 services = await _serviceRepository.GetAllServicesApprovedAsync();
            }
            else
            {
                services = await _serviceRepository.GetApprovedServicesByServiceDepartmentAsync(currentUser.DepartmentId);
            }

            var serviceDtos = new List<ServiceDto>();
            foreach (var service in services)
            {
                var map = _mapper.Map<ServiceDto>(service);
                var serviceCreator = await _userManager.FindByIdAsync(service.CreatedById);
                var serviceCreatorDto = _mapper.Map<UserDto>(serviceCreator);
                map.CreatedBy = serviceCreatorDto;

                var serviceApprovedBy = await _userManager.FindByIdAsync(service.ServiceApprovedById);
                var serviceApprovedByDto = _mapper.Map<UserDto>(serviceApprovedBy);
                map.ServiceApprovedBy = serviceApprovedByDto;
                serviceDtos.Add(map);
            }
            return View(serviceDtos);
        }

        [HttpPost]
        public async Task<IActionResult> ServiceReport(SearchViewModel searchViewModel)
        {
            IEnumerable<Service> services = new List<Service>();
            var dropdownLists = await DropDownLists();

            //If user is WITH ALL SERVICES PERMISION, DONT PASS IN A USER DEPARTMENT ID
            var currentUser = await GetCurrentUser();
            if (User.HasClaim(CustomClaims.Permission, CustomClaimsValues.ViewAllServices))
            {
                var startDate = searchViewModel.StartDate.ToInvariantDateTime("dd/MM/yyyy");
                var endDate = searchViewModel.EndDate.ToInvariantDateTime("dd/MM/yyyy");
                services = await _serviceRepository.GetAllApprovedServicesByDateItemTypeItemDepartmentAndCategoryAsync(startDate, endDate,
                searchViewModel.ItemTypeId, searchViewModel.CategoryId, searchViewModel.ItemDepartmentId);
            }
            else
            {
                var startDate = searchViewModel.StartDate.ToInvariantDateTime("dd/MM/yyyy");
                var endDate = searchViewModel.EndDate.ToInvariantDateTime("dd/MM/yyyy");
                services = await _serviceRepository.GetAllApprovedServicesByDateItemTypeItemDepartmentAndCategoryAsync(startDate, endDate,
                searchViewModel.ItemTypeId, searchViewModel.CategoryId, searchViewModel.ItemDepartmentId, currentUser.DepartmentId);
            }

            HighlightSelectedItem(dropdownLists, searchViewModel);

            ViewBag.SearchParameters = searchViewModel;
            ViewBag.Categories = dropdownLists.categoriesList;
            ViewBag.ItemTypes = dropdownLists.itemTypesList;
            ViewBag.Departments = dropdownLists.departmentsList;

            var serviceDtos = new List<ServiceDto>();
            foreach (var service in services)
            {
                var map = _mapper.Map<ServiceDto>(service);
                var itemCreator = await _userManager.FindByIdAsync(service.CreatedById);
                var itemCreatorDto = _mapper.Map<UserDto>(itemCreator);
                map.CreatedBy = itemCreatorDto;

                var serviceApprovedBy = await _userManager.FindByIdAsync(service.ServiceApprovedById);
                var serviceApprovedByDto = _mapper.Map<UserDto>(serviceApprovedBy);
                map.ServiceApprovedBy = serviceApprovedByDto;
                serviceDtos.Add(map);
            }
            return View(serviceDtos);
        }

        public async Task<User> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }
    }
}
    