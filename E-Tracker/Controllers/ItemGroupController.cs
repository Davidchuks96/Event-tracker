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
using E_Tracker.Repository.AutoGenServicePeriodRepository;
using E_Tracker.Repository.CategoryRepository;
using E_Tracker.Repository.DepartmentRepo;
using E_Tracker.Repository.EmailRepo;
using E_Tracker.Repository.ItemGroupRepository;
using E_Tracker.Repository.ItemRepository;
using E_Tracker.Repository.ReminderRepo;
using E_Tracker.Repository.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    public class ItemGroupController : BaseController
    {
        private readonly ILogger<ItemGroupController> _logger;
        private readonly IMapper _mapper;
        private readonly IItemGroupRepository _itemGroupRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IReminderService _reminderService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAutoGenServicePeriodService _autoGenServicePeriodService;

        public ItemGroupController(ILogger<ItemGroupController> logger, IMapper mapper, IItemGroupRepository itemGroupRepository, IDepartmentRepository departmentRepository, UserManager<User> userManager, IUserRepository userRepository,
            IItemRepository itemRepository, INotificationService notificationService, IReminderService reminderService, ICategoryRepository categoryRepository, IAutoGenServicePeriodService autoGenServicePeriodService)
        {
            _logger = logger;
            _mapper = mapper;
            _itemRepository = itemRepository;
            _itemGroupRepository = itemGroupRepository;
            _departmentRepository = departmentRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _reminderService = reminderService;
            _categoryRepository = categoryRepository;
            _autoGenServicePeriodService = autoGenServicePeriodService;
        }
        // GET: /<controller>/
        private async Task<List<ItemGroupDto>> ItemGroupListDto(IEnumerable<ItemGroup> ItemGroups)
        {
            var itemGroupsDto = new List<ItemGroupDto>();
            foreach (var ItemGroup in ItemGroups)
            {
                //var ItemGroupType = await _ItemGroupTypeRepository.GetItemGroupTypeByIdAsync(ItemGroup.ItemGroupTypeId);
                //var dept = await _departmentRepository.GetDepartmentByIdAsync(ItemGroup.DepartmentId);
                //ItemGroup.ItemGroupType = ItemGroupType;
                //ItemGroup.Department = dept;
                var map = _mapper.Map<ItemGroupDto>(ItemGroup);
                var createdUser = await _userRepository.GetUserByIdAsync(ItemGroup.CreatedById);
                var approvedUser = await _userRepository.GetUserByIdAsync(ItemGroup.ApprovedById);
                var deletedUser = await _userRepository.GetUserByIdAsync(ItemGroup.DeletedById);
                var updatedUser = await _userRepository.GetUserByIdAsync(ItemGroup.UpdatedById);

                map.CreatedBy = _mapper.Map<UserDto>(createdUser);
                //map.ApprovedBy = _mapper.Map<UserDto>(approvedUser);
                map.DeletedBy = _mapper.Map<UserDto>(deletedUser);
                map.UpdatedBy = _mapper.Map<UserDto>(updatedUser);
                itemGroupsDto.Add(map);
            }

            return itemGroupsDto;
        }

        private async Task<(IEnumerable<Department> departments,IEnumerable<Category> categories)> DropDownListDto()
        {
            var departments = await _departmentRepository.GetAllDepartmentAsync();
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return (departments, categories);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewAllItemGroups)]
        public async Task<IActionResult> AllItemGroupsInEveryDepartment()
        {///test
            var itemGroups = await _itemGroupRepository.GetAllItemGroupsAsync();
            var itemGroupsDto = await ItemGroupListDto(itemGroups);
            ViewBag.IsAllItemGroups = true;
            return View("AllItemGroups", itemGroupsDto);
        }
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItemGroup)]
        public async Task<IActionResult> AllItemGroupsCreatedByMyDepartment()
        {
            var user = await GetCurrentUser();
            var itemGroups = await _itemGroupRepository.GetItemGroupsByCreatedByDepartmentIdAsync(user.DepartmentId);
            var itemGroupsDto = await ItemGroupListDto(itemGroups);
            return View("AllItemGroups", itemGroupsDto);

        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItemGroup)]
        public async Task<IActionResult> AllItemGroupsLocatedInMyDepartment()
        {
            var user = await GetCurrentUser();
            var itemGroups = await _itemGroupRepository.GetApprovedItemGroupsByDepartmentIdAsync(user.DepartmentId);
            var itemGroupsDto = await ItemGroupListDto(itemGroups);
            return View(itemGroupsDto);

        }
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateItemGroup)]
        public async Task<IActionResult> ActivateItemGroup(string itemGroupId)
        {
            var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupId);
            if (itemGroup is null)
            {
                var msg = $"ItemGroup with ID:{itemGroupId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllNotActiveItemGroups");
            }

            var result = await _itemGroupRepository.ActivateItemGroupAsync(itemGroup);
             await _itemRepository.ActivateItemsByItemGroupIdAsync(itemGroup.Id);
            if (result.Successful)
            {
                Alert(result.Message, Notifications.success);

            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllNotActiveItemGroups");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateItemGroup)]
        public async Task<IActionResult> AllNotActiveItemGroups()
        {
            var itemGroups = await _itemGroupRepository.GetAllNotActiveItemGroupsAsync();
            var itemGroupsDto = new List<ItemGroupDto>();
            foreach (var itemGroup in itemGroups)
            {
                var map = _mapper.Map<ItemGroupDto>(itemGroup);
                itemGroupsDto.Add(map);
            }
            ViewBag.IsAllItemGroups = false;
            return View("AllItemGroups", itemGroupsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItemGroup)]
        public async Task<IActionResult> AllApprovedItemGroups()
        {
            var user = await GetCurrentUser();
            if (user.DepartmentId == null)
            {
                return View("AllItemGroups", await ItemGroupListDto(await _itemGroupRepository.GetAllApprovedItemGroupsAsync()));
            }
            var itemGroups = await _itemGroupRepository.GetItemGroupsByCreatedByDepartmentIdAsync(user.DepartmentId);
            var myItemGroups = itemGroups.Where(x => x.IsApproved == true);
            var itemGroupsDto = await ItemGroupListDto(myItemGroups);
            return View("AllItemGroups", itemGroupsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItemGroup)]
        public async Task<IActionResult> AllItemGroupsByCategory(string categoryId)
        {
            var user = await GetCurrentUser();
            if (user.DepartmentId == null)
            {
                return View("AllItemGroups", await ItemGroupListDto(await _itemGroupRepository.GetAllActiveItemGroupsByCategoryIdAsync(categoryId)));
            }
            var itemGroups = await _itemGroupRepository.GetAllActiveItemGroupsByCategoryIdAsync(categoryId);
            var result = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            TempData["CategoryName"] = result.Name;
            var itemGroupsDto = await ItemGroupListDto(itemGroups);
            return View("AllItemGroups", itemGroupsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItemGroup)]
        public async Task<IActionResult> AllItemGroupsByMyDepartmentByCategory(string categoryId)
        {
            var user = await GetCurrentUser();
            var result = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (user.DepartmentId == null)
            {
                TempData["CategoryName"] = result.Name;
                return View("AllItemGroups", await ItemGroupListDto(await _itemGroupRepository.GetAllActiveItemGroupsByCategoryIdAsync(categoryId)));
            }
            var itemGroups = await _itemGroupRepository.GetItemGroupsByMyDepartmentCategoryIdAsync(user.DepartmentId, categoryId);
            TempData["CategoryName"] = result.Name;
            var itemGroupsDto = await ItemGroupListDto(itemGroups);
            return View("AllItemGroups", itemGroupsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItemGroup)]
        public async Task<IActionResult> AllNotApprovedItemGroups()
        {
            var user = await GetCurrentUser();
            if (user.DepartmentId == null)
            {
                return View("AllItemGroups", await ItemGroupListDto(await _itemGroupRepository.GetAllNotApprovedItemGroupsAsync()));
            }
            var itemGroups = await _itemGroupRepository.GetItemGroupsByCreatedByDepartmentIdAsync(user.DepartmentId);
            var myItemGroups = itemGroups.Where(x => x.IsApproved == false);
            var itemGroupsDto = await ItemGroupListDto(myItemGroups);
            return View("AllItemGroups", itemGroupsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItemGroup)]
        public async Task<IActionResult> Details(string itemGroupId)
        {
            var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupId);
            if (itemGroup is null)
            {
                var msg = $"ItemGroup with ID:{itemGroupId} does not exist";
                Alert(msg, Notifications.error);
                //TempData["msg"] = $"<script>Swal.fire('Failed!','ItemGroup with ID:{ItemGroupId} does not exist','error');</script>";
                return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
            }
            var itemGroupDto = _mapper.Map<ItemGroupDto>(itemGroup);
            var dept = await _departmentRepository.GetDepartmentByIdAsync(itemGroup.DepartmentId);
            var deptDto = _mapper.Map<DepartmentDto>(dept);
            ViewBag.Items = await _itemRepository.GetItemsByItemGroupIdAsync(itemGroupId);
            //ItemGroupDto.Department = deptDto;
            var createdUser = await _userRepository.GetUserByIdAsync(itemGroup.CreatedById);
            var approvedUser = await _userRepository.GetUserByIdAsync(itemGroup.ApprovedById);
            var deletedUser = await _userRepository.GetUserByIdAsync(itemGroup.DeletedById);
            var updatedUser = await _userRepository.GetUserByIdAsync(itemGroup.UpdatedById);
            itemGroupDto.CreatedBy = _mapper.Map<UserDto>(createdUser);
            itemGroupDto.ApprovedBy = _mapper.Map<UserDto>(approvedUser);
            itemGroupDto.DeletedBy = _mapper.Map<UserDto>(deletedUser);
            itemGroupDto.UpdatedBy = _mapper.Map<UserDto>(updatedUser);
            return View(itemGroupDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItemGroup)]
        public async Task<IActionResult> ItemGroupsByDepartmentId(string departmentId)
        {
            //if (departmentId is null) return NotFound;
            var itemGroups = await _itemGroupRepository.GetApprovedItemGroupsByDepartmentIdAsync(departmentId);
            var itemGroupsDto = await ItemGroupListDto(itemGroups);
            return View(itemGroupsDto);
        }
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.CreateItemGroup)]
        public async Task<IActionResult> CreateItemGroup()
        {
            var result = await DropDownListDto();
            ViewBag.Departments = result.departments;
            ViewBag.Categories = result.categories;
            return View();
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.CreateItemGroup)]
        public async Task<IActionResult> CreateItemGroup(CreateItemGroupDto itemGroupDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var result = await DropDownListDto();
                    ViewBag.Departments = result.departments;
                    ViewBag.Categories = result.categories;
                    return View(itemGroupDto);
                }
                var itemGroupcheck = await _itemGroupRepository.GetItemGroupByTagNoAsync(itemGroupDto.TagNo.Trim());
                if (itemGroupcheck == null)
                {
                    itemGroupDto.TagNo = itemGroupDto.TagNo.Trim();
                    var itemGroup = _mapper.Map<ItemGroup>(itemGroupDto);
                    var user = await GetCurrentUser();
                    itemGroup.CreatedById = user.Id;
                    var result = await _itemGroupRepository.CreateItemGroupAsync(itemGroup);
                    if (result.Successful)
                    {

                        Alert(result.Message, Notifications.success);

                        await _notificationService.SendCreateItemGroupNotifications(itemGroup.Id, user);
                    }
                    else
                    {
                        Alert(result.Message, Notifications.error);
                    }
                }
                else
                {
                    Alert($"ItemGroup with Tag Number: {itemGroupDto.TagNo} already exists", Notifications.error);
                    var result = await DropDownListDto();
                    ViewBag.Departments = result.departments;
                    ViewBag.Categories = result.categories;
                    return View(itemGroupDto);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            //TempData["msg"] = $"<script>Swal.fire('Success!','{result.Message}','success');</script>";
            //TempData["msg"] = $"<script>Swal.fire('Failed!','{result.Message}','error');</script>";  
            return RedirectToAction("AllNotApprovedItemGroups");
        }



        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.UpdateItemGroup)]
        public async Task<IActionResult> UpdateItemGroup(string itemGroupId)
        {
            var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupId);
            if (itemGroup is null)
            {
                var msg = $"ItemGroup with ID:{itemGroupId} does not exist";
                Alert(msg, Notifications.error);
                //TempData["msg"] = $"<script>Swal.fire('Failed!','ItemGroup with ID:{ItemGroupId} does not exist','error');</script>";
                return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
            }
            if (itemGroup.IsApproved == true)
            {
                var msg = $"ItemGroup with Tag Number:{itemGroup.TagNo} has been approved and can't be edited";
                Alert(msg, Notifications.error);
                //TempData["msg"] = $"<script>Swal.fire('Failed!','ItemGroup with ID:{ItemGroupId} does not exist','error');</script>";
                return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
            }
            var itemGroupDto = _mapper.Map<ItemGroupDto>(itemGroup);
            var result = await DropDownListDto();
            ViewBag.Departments = result.departments;
            ViewBag.Categories = result.categories;
            return View(itemGroupDto);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.UpdateItemGroup)]
        public async Task<IActionResult> UpdateItemGroup(ItemGroupDto itemGroupDto)
        {
            if (!ModelState.IsValid)
            {
                var dropdownLists = await DropDownListDto();
                ViewBag.Departments = dropdownLists.departments;
                ViewBag.Categories = dropdownLists.categories;
                return View(itemGroupDto);
            }

            var itemGroup = _mapper.Map<ItemGroup>(itemGroupDto);
            var user = await GetCurrentUser();
            itemGroup.UpdatedById = user.Id;

            var result = await _itemGroupRepository.UpdateItemGroupAsync(itemGroup);
            if (result.Successful)
            {
                Alert(result.Message, Notifications.success);
                var creatorOfItemGroup = await _userManager.FindByIdAsync(itemGroup.CreatedById);
                creatorOfItemGroup = creatorOfItemGroup ?? new User();
                /*BackgroundJob.Enqueue(() =>*/
                //await _reminderService.UpdateReminderLog(itemGroup, creatorOfItemGroup.Email);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.DeleteItemGroup)]
        public async Task<IActionResult> DeleteItemGroup(string itemGroupId)
        {
            var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupId);
            if (itemGroup is null)
            {
                var msg = $"ItemGroup with ID:{itemGroupId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
            }

            var user = await GetCurrentUser();
            itemGroup.DeletedById = user.Id;
            var result = await _itemGroupRepository.DeleteItemGroupAsync(itemGroup);
            await _itemRepository.DeleteItemsByItemGroupIdAsync(itemGroup.Id);


            if (result.Successful)
            {
                Alert(result.Message, Notifications.success);
                //remove reminder logs once an ItemGroup has been deleted
                /*BackgroundJob.Enqueue(() =>*/
                await _reminderService.DeleteReminderLog(itemGroup.Id);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllItemGroupsCreatedByMyDepartment");

        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ApproveItemGroup)]
        public async Task<IActionResult> ApproveItemGroup(string itemGroupId)
        {
            var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupId);

            if (itemGroup is null)
            {
                var msg = $"ItemGroup with ID:{itemGroupId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
            }
            if (itemGroup.IsApproved == true)
            {
                var msg = $"{itemGroup.Name} has already been approved";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
            }
            itemGroup.IsApproved = true;
            //itemGroup.Status = "Approved";
            itemGroup.DateApproved = DateTime.Now;
            var user = await GetCurrentUser();
            itemGroup.ApprovedById = user.Id;
            var result = await _itemGroupRepository.UpdateItemGroupAsync(itemGroup);
            if (result.Successful)
            {
                //var creatorOfItemGroup = await _userManager.FindByIdAsync(itemGroup.CreatedById);
                //BackgroundJob.Enqueue(() => 
                //await _autoGenServicePeriodService.FirstRecordOfNextServicePeriod(itemGroup.Id, itemGroup.ExpiredDate,
                //    itemGroup.DateCreated, itemGroup.ReoccurenceValue, itemGroup.ReoccurenceFrequency);
                //await _reminderService.SetReminderLog(itemGroup, creatorOfItemGroup?.Email);
                await _notificationService.SendApproveItemGroupNotifications(itemGroup.Id, user);
                Alert($"{itemGroup.Name} was approved successfully", Notifications.success);

                /*BackgroundJob.Enqueue(() =>*/
                //await _notificationService.SendApproveItemGroupNotifications(itemGroupId, user);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.ApproveItemGroup)]
        public async Task<IActionResult> ApproveItemGroup(ItemGroupDto itemGroupDto)
        {
            var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupDto.Id);
            if (itemGroup is null)
            {
                var msg = $"ItemGroup with ID:{itemGroupDto.Id} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
            }
            if (itemGroup.IsApproved)
            {
                var msg = $"{itemGroup.Name} has already been approved";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
            }
            itemGroup.IsApproved = itemGroupDto.IsApproved;
            //itemGroup.ApproveOrRejectComments = itemGroupDto.ApproveOrRejectComments;
            itemGroup.DateApproved = DateTime.Now;
            var user = await GetCurrentUser();
            itemGroup.ApprovedById = user.Id;
            var result = await _itemGroupRepository.UpdateItemGroupAsync(itemGroup);
            if (result.Successful)
            {
                var creatorOfItemGroup = await _userManager.FindByIdAsync(itemGroup.CreatedById);
                //await _reminderService.SetReminderLog(itemGroup, creatorOfItemGroup?.Email);
                //Alert($"{itemGroup.Name} was approved successfully", Notifications.success);
                ///*BackgroundJob.Enqueue(() =>*/
                //await _notificationService.SendApproveItemGroupNotifications(itemGroupDto.Id, user);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllItemGroupsCreatedByMyDepartment");
        }

        private async Task<User> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }
    }
}
