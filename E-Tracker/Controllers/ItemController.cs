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
using E_Tracker.Repository.ItemTypeRepository;
using E_Tracker.Repository.ReminderRepo;
using E_Tracker.Repository.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    public class ItemController : BaseController
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IMapper _mapper;
        private readonly IItemRepository _itemRepository;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IReminderService _reminderService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAutoGenServicePeriodService _autoGenServicePeriodService;
        private readonly IItemGroupRepository _itemGroupRepository;

        public ItemController(ILogger<ItemController> logger, IMapper mapper, IItemRepository itemRepository, IItemTypeRepository itemTypeRepository, IDepartmentRepository departmentRepository, UserManager<User> userManager, IUserRepository userRepository,
            INotificationService notificationService, IReminderService reminderService, ICategoryRepository categoryRepository, IAutoGenServicePeriodService autoGenServicePeriodService,
            IItemGroupRepository itemGroupRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _itemRepository = itemRepository;
            _itemTypeRepository = itemTypeRepository;
            _departmentRepository = departmentRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _reminderService = reminderService;
            _categoryRepository = categoryRepository;
            _autoGenServicePeriodService = autoGenServicePeriodService;
            _itemGroupRepository = itemGroupRepository;
        }
        // GET: /<controller>/
        private async Task<List<ItemDto>> ItemListDto(IEnumerable<Item> items)
        {
            var itemsDto = new List<ItemDto>();
            foreach (var item in items)
            {
                var itemType = await _itemTypeRepository.GetItemTypeByIdAsync(item.ItemTypeId);
                var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(item.ItemGroupId);
                //var dept = await _departmentRepository.GetDepartmentByIdAsync(item.DepartmentId);
                item.ItemType = itemType;
                item.ItemGroup = itemGroup;
                //item.Department = dept;
                var map = _mapper.Map<ItemDto>(item);
                var createdUser = await _userRepository.GetUserByIdAsync(item.CreatedById);
                var approvedUser = await _userRepository.GetUserByIdAsync(item.ApprovedById);
                var deletedUser = await _userRepository.GetUserByIdAsync(item.DeletedById);
                var updatedUser = await _userRepository.GetUserByIdAsync(item.UpdatedById);
                
                map.CreatedBy = _mapper.Map<UserDto>(createdUser);
                map.ApprovedBy = _mapper.Map<UserDto>(approvedUser);
                map.DeletedBy = _mapper.Map<UserDto>(deletedUser);
                map.UpdatedBy = _mapper.Map<UserDto>(updatedUser);
                itemsDto.Add(map);
            }

            return itemsDto;
        }

        private async Task<(IEnumerable<Department> departments, IEnumerable<ItemType> itemTypes, IEnumerable<Category> categories)> DropDownListDto()
        {
            var departments = await _departmentRepository.GetAllDepartmentAsync();
            var itemtypes = await _itemTypeRepository.GetAllItemTypesAsync();
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return (departments, itemtypes, categories);
        }

        //[HttpGet]
        //[Authorize(Policy = CustomClaimsValues.ViewItem)]
        //public IActionResult AllItems()
        //{
        //    return View(new List<ItemDto>());
        //}

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewAllItems)]
        public async Task<IActionResult> AllItemsInEveryDepartment()
        {///test
            var items = await _itemRepository.GetAllItemsAsync();
            var itemsDto = await ItemListDto(items);
            ViewBag.IsAllItems = true;
            return View("AllItems",itemsDto);
        }
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> AllItemsCreatedByMyDepartment()
        {
            var user = await GetCurrentUser();
            var items = await _itemRepository.GetItemsByCreatedByDepartmentIdAsync(user.DepartmentId);
            var itemsDto = await ItemListDto(items);
            return View("AllItems", itemsDto);

        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> AllItemsLocatedInMyDepartment()
        {
            var user = await GetCurrentUser();
            var items = await _itemRepository.GetApprovedItemsByDepartmentIdAsync(user.DepartmentId);
            var itemsDto = await ItemListDto(items);
            return View(itemsDto);

        }
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateItem)]
        public async Task<IActionResult> ActivateItem(string itemId)
        {
            var item = await _itemRepository.GetItemByIdAsync(itemId);
            if (item is null)
            {
                var msg = $"Item with ID:{itemId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllNotActiveItems");
            }
            var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(item.ItemGroupId);
            if (itemGroup is null )
            {
                var msg = $"Item with ID:{itemId} is not associated with an existing ItemGroup";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllNotActiveItems");
            }
            if (!itemGroup.IsActive)
            {
                var msg = $"ItemGroup '{itemGroup?.Name}' for Item:{item?.Name} is not active. Kindly restore the ItemGroup first";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllNotActiveItems");
            }
            var result = await _itemRepository.ActivateItemAsync(item);
            if (result.Successful)
            { 
                Alert(result.Message, Notifications.success);

                //set reminder log for activated item
                var creatorOfItem = await _userManager.FindByIdAsync(item.CreatedById);
                /*BackgroundJob.Enqueue(() =>*/ await _reminderService.SetReminderLog(item, creatorOfItem.Email);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllNotActiveItems");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateItem)]
        public async Task<IActionResult> AllNotActiveItems()
        {
            var items = await _itemRepository.GetAllNotActiveItemsAsync();
            var itemsDto = await ItemListDto(items);
            ViewBag.IsAllItems = false;
            return View("AllItems", itemsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> AllApprovedItems()
        {
            var user = await GetCurrentUser();
            if (user.DepartmentId == null)
            {
                return View("AllItems", await ItemListDto(await _itemRepository.GetAllApprovedItemsAsync()));
            }
            var items = await _itemRepository.GetItemsByCreatedByDepartmentIdAsync(user.DepartmentId);
            var myItems = items.Where(x => x.IsApproved == true);
            var itemsDto = await ItemListDto(myItems);
            return View("AllItems", itemsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> AllItemsByCategory(string categoryId)
        {
            var user = await GetCurrentUser();
            if (user.DepartmentId == null)
            {
                return View("AllItems", await ItemListDto(await _itemRepository.GetItemsByCategoryIdAsync(categoryId)));
            }
            var items = await _itemRepository.GetItemsByCategoryIdAsync(categoryId);
             var result = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            TempData["CategoryName"] = result.Name;
            var itemsDto = await ItemListDto(items);
            return View("AllItems", itemsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> AllItemsCreatedByMyDepartmentByCategory(string categoryId)
        {
            var user = await GetCurrentUser();
            var result = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (user.DepartmentId == null)
            {
                TempData["CategoryName"] = result.Name;
                return View("AllItems", await ItemListDto(await _itemRepository.GetItemsByCategoryIdAsync(categoryId)));
            }
            var items = await _itemRepository.GetItemsByCreatedByDepartmentIdAsync(user.DepartmentId, categoryId);
            TempData["CategoryName"] = result.Name;
            var itemsDto = await ItemListDto(items);
            return View("AllItems", itemsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> AllNotApprovedItems()
        {
            var user = await GetCurrentUser();
            if (user.DepartmentId == null)
            {
                return View("AllItems", await ItemListDto(await _itemRepository.GetAllNotApprovedItemsAsync()));
            }
            var items = await _itemRepository.GetItemsByCreatedByDepartmentIdAsync(user.DepartmentId);
            var myItems = items.Where(x => x.IsApproved == false);
            var itemsDto = await ItemListDto(myItems);
            return View("AllItems", itemsDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> Details(string itemId)
        {
            var callingUrl = Request.Headers["Referer"].ToString();
            
            ViewBag.ReturnUrl = IsLocalUrl(callingUrl) ? callingUrl : null;

            var item = await _itemRepository.GetItemByIdAsync(itemId);
            if (item is null)
            {
                var msg = $"Item with ID:{itemId} does not exist";
                Alert(msg, Notifications.error);
                //TempData["msg"] = $"<script>Swal.fire('Failed!','Item with ID:{itemId} does not exist','error');</script>";
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            var itemDto = _mapper.Map<ItemDto>(item);
            var itemType = await _itemTypeRepository.GetItemTypeByIdAsync(item.ItemTypeId);
            var itemTypeDto = _mapper.Map<ItemTypeDto>(itemType);
           
            itemDto.ItemType = itemTypeDto;
            var createdUser = await _userRepository.GetUserByIdAsync(item.CreatedById);
            var approvedUser = await _userRepository.GetUserByIdAsync(item.ApprovedById);
            var deletedUser = await _userRepository.GetUserByIdAsync(item.DeletedById);
            var updatedUser = await _userRepository.GetUserByIdAsync(item.UpdatedById);
            itemDto.CreatedBy = _mapper.Map<UserDto>(createdUser);
            itemDto.ApprovedBy = _mapper.Map<UserDto>(approvedUser);
            itemDto.DeletedBy = _mapper.Map<UserDto>(deletedUser);
            itemDto.UpdatedBy = _mapper.Map<UserDto>(updatedUser);
            return View(itemDto);
        }

        private bool IsLocalUrl(string returnUrl)
        {
            var requestScheme = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            if (returnUrl.Contains(requestScheme))
            {
                var trimmedreturnUrl = returnUrl.Remove(0, requestScheme.Length);
                if (Url.IsLocalUrl(trimmedreturnUrl))
                {
                    return true;
                }
                return false;
            }
            else
            {
                _logger.LogWarning($"The supplied URL {returnUrl} is not local.");
                return false;
            }
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> ItemsByDepartmentId(string departmentId)
        {
            //if (departmentId is null) return NotFound;
            var items = await _itemRepository.GetApprovedItemsByDepartmentIdAsync(departmentId);
            var itemsDto = await ItemListDto(items);
            return View(itemsDto);
        }
        
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> AllItemsByItemGroupId(string itemGroupId)
        {
            //if (departmentId is null) return NotFound;
            var items = await _itemRepository.GetItemsByItemGroupIdAsync(itemGroupId);
            var itemsDto = await ItemListDto(items);
            ViewBag.ItemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupId);
            TempData["CategoryName"] = ViewBag.ItemGroup?.Category?.Name;
            return View("AllItems", itemsDto);
        }
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.CreateItem)]
        public async Task<IActionResult> CreateItem(string itemGroupId)
        {
            var callingUrl = Request.Headers["Referer"].ToString();

            TempData["ReturnUrl"] = IsLocalUrl(callingUrl) ? callingUrl : null;
            var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupId);
            if (itemGroup == null)
            {
                var msg = $"ItemGroup with ID:{itemGroupId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemGroupsCreatedByMyDepartment", "ItemGroup");
            }
            var result = await DropDownListDto();
            ViewBag.Itemtypes = result.itemTypes;
            CreateItemDto createItemDto = new CreateItemDto
            {
                ItemGroupId = itemGroupId,
                ReturnUrl = TempData["ReturnUrl"] as string
            };
            return View(createItemDto);
        }
       
        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.CreateItem)]
        public async Task<IActionResult> CreateItem(CreateItemDto itemDto)
        {
            string returnUrl = IsLocalUrl(itemDto.ReturnUrl) ? itemDto.ReturnUrl : null;
            try
            {
                if(!ModelState.IsValid)
                {
                    var result = await DropDownListDto();
                    ViewBag.Departments = result.departments;
                    ViewBag.Itemtypes = result.itemTypes;
                    ViewBag.Categories = result.categories;
                    return View(itemDto);
                }
                var itemcheck = await _itemRepository.GetItemByTagNoAsync(itemDto.TagNo);
                if (itemcheck == null)
                {
                    var item = _mapper.Map<Item>(itemDto);
                    var user = await GetCurrentUser();
                    item.CreatedById = user.Id;
                    var result = await _itemRepository.CreateItemAsync(item);
                    if (result.Successful)
                    {
                        
                        Alert(result.Message, Notifications.success);

                        await _notificationService.SendCreateItemNotifications(item.Id, user);
                    }
                    else
                    {
                        Alert(result.Message, Notifications.error);
                    }
                }
                else
                {
                    Alert($"Item with Tag Number: {itemDto.TagNo} already exists", Notifications.error);
                    var result = await DropDownListDto();
                    ViewBag.Departments = result.departments;
                    ViewBag.Itemtypes = result.itemTypes;
                    ViewBag.Categories = result.categories;
                    return View(itemDto);
                }
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            //TempData["msg"] = $"<script>Swal.fire('Success!','{result.Message}','success');</script>";
            //TempData["msg"] = $"<script>Swal.fire('Failed!','{result.Message}','error');</script>";  
            //Redirect to the details view of the Item Group
            if (returnUrl == null)
            {
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            return Redirect(returnUrl);
            //return RedirectToAction("Details","ItemGroup",itemDto.ItemGroupId);
            //return RedirectToAction("AllNotApprovedItems");
        }

       

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.UpdateItem)]
        public async Task<IActionResult> UpdateItem(string itemId)
        {
            var callingUrl = Request.Headers["Referer"].ToString();

            ViewBag.ReturnUrl = IsLocalUrl(callingUrl) ? callingUrl : null;

            var item = await _itemRepository.GetItemByIdAsync(itemId);
            if (item is null)
            {
                var msg = $"Item with ID:{itemId} does not exist";
                Alert(msg, Notifications.error);
                //TempData["msg"] = $"<script>Swal.fire('Failed!','Item with ID:{itemId} does not exist','error');</script>";
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            //var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(item.ItemGroupId);
            if (item.IsApproved == true)
            {
                var msg = $"Item with Tag Number:{item.TagNo} has been approved and can't be edited";
                Alert(msg, Notifications.error);
                //TempData["msg"] = $"<script>Swal.fire('Failed!','Item with ID:{itemId} does not exist','error');</script>";
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            var itemDto = _mapper.Map<ItemDto>(item);
            //ReturnUrl
            itemDto.ReturnUrl = ViewBag.ReturnUrl;
            
            var result = await DropDownListDto();
            ViewBag.Departments = result.departments;
            ViewBag.Itemtypes = result.itemTypes;
            ViewBag.Categories = result.categories;
            return View(itemDto);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.UpdateItem)]
        public async Task<IActionResult> UpdateItem(ItemDto itemDto)
        {
            string returnUrl = IsLocalUrl(itemDto.ReturnUrl) ? itemDto.ReturnUrl : null;
            if (!ModelState.IsValid)
            {
                var dropdownLists = await DropDownListDto();
                ViewBag.Departments = dropdownLists.departments;
                ViewBag.Itemtypes = dropdownLists.itemTypes;
                ViewBag.Categories = dropdownLists.categories;
                return View(itemDto);
            }

            var item = _mapper.Map<Item>(itemDto);
            var user = await GetCurrentUser();
            item.UpdatedById = user.Id;
            
            var result = await _itemRepository.UpdateItemAsync(item);
            if (result.Successful)
            {
                Alert(result.Message, Notifications.success);
                var creatorOfItem = await _userManager.FindByIdAsync(item.CreatedById);
                creatorOfItem = creatorOfItem ?? new User();
                /*BackgroundJob.Enqueue(() =>*/ await _reminderService.UpdateReminderLog(item, creatorOfItem.Email);
            }
            else
                Alert(result.Message, Notifications.error);
            if (returnUrl == null)
            {
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            return Redirect(returnUrl);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.DeleteItem)]
        public async Task<IActionResult> DeleteItem(string itemId)
        {
            var callingUrl = Request.Headers["Referer"].ToString();

            string returnUrl = IsLocalUrl(callingUrl) ? callingUrl : null;
            var item = await _itemRepository.GetItemByIdAsync(itemId);
            if (item is null)
            {
                var msg = $"Item with ID:{itemId} does not exist";
                Alert(msg, Notifications.error);
                if (returnUrl == null)
                {
                    return RedirectToAction("AllItemsCreatedByMyDepartment");
                }
                return Redirect(returnUrl);
            }

            var user = await GetCurrentUser();
            item.DeletedById = user.Id;
            var result = await _itemRepository.DeleteItemAsync(item);


            if (result.Successful)
            {
                Alert(result.Message, Notifications.success);
                //remove reminder logs once an item has been deleted
                /*BackgroundJob.Enqueue(() =>*/ await _reminderService.DeleteReminderLog(item.Id);
            }
            else
                Alert(result.Message, Notifications.error);
            if (returnUrl == null)
            {
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            return Redirect(returnUrl);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ApproveItem)]
        public async Task<IActionResult> ApproveItem(string itemId)
        {
            var callingUrl = Request.Headers["Referer"].ToString();

            string returnUrl = IsLocalUrl(callingUrl) ? callingUrl : null;

            var item = await _itemRepository.GetItemByIdAsync(itemId);
            if (item is null)
            {
                var msg = $"Item with ID:{itemId} does not exist";
                Alert(msg, Notifications.error);
                if (returnUrl == null)
                {
                    return RedirectToAction("AllItemsCreatedByMyDepartment");
                }
                return Redirect(returnUrl);
            }
            var itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(item.ItemGroupId);
            
            //Dont approve an item that does not belong to an itemGroup
            if (itemGroup is null)
            {
                var msg = $"Item with ID:{itemId} does not belong to an existing ItemGroup";
                Alert(msg, Notifications.error);
                if (returnUrl == null)
                {
                    return RedirectToAction("AllItemsCreatedByMyDepartment");
                }
                return Redirect(returnUrl);
            }
            
            //Dont approve an item if its itemGroup has not been approved
            if (!itemGroup.IsApproved)
            {
                var msg = $"ItemGroup {itemGroup?.Name} has not been approved. Kindly approve this first!";
                Alert(msg, Notifications.error);
                if (returnUrl == null)
                {
                    return RedirectToAction("AllItemsCreatedByMyDepartment");
                }
                return Redirect(returnUrl);
            }
            if (item.IsApproved == true)
            {
                var msg = $"{item.Name} has already been approved";
                Alert(msg, Notifications.error);
                if (returnUrl == null)
                {
                    return RedirectToAction("AllItemsCreatedByMyDepartment");
                }
                return Redirect(returnUrl);
            }
            item.IsApproved = true;
            item.Status = "Approved";
            item.DateApproved = DateTime.Now;
            var user = await GetCurrentUser();
            item.ApprovedById = user.Id;
            var result = await _itemRepository.UpdateItemAsync(item);
            if (result.Successful)
            {
                var creatorOfItem = await _userManager.FindByIdAsync(item.CreatedById);
                //BackgroundJob.Enqueue(() => 
                await _autoGenServicePeriodService.FirstRecordOfNextServicePeriod(item.Id, item.ExpiredDate,
                    item.DateCreated,item.ReoccurenceValue, item.ReoccurenceFrequency);
                await _reminderService.SetReminderLog(item, creatorOfItem?.Email);
                Alert($"{item.Name} was approved successfully", Notifications.success);

                /*BackgroundJob.Enqueue(() =>*/ await _notificationService.SendApproveItemNotifications(itemId, user);
            }
            else
                Alert(result.Message, Notifications.error);
            if (returnUrl == null)
            {
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            return Redirect(returnUrl);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.ApproveItem)]
        public async Task<IActionResult> ApproveItem(ItemDto itemDto)
        {
            var item = await _itemRepository.GetItemByIdAsync(itemDto.Id);
            if (item is null)
            {
                var msg = $"Item with ID:{itemDto.Id} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            if (item.IsApproved)
            {
                var msg = $"{item.Name} has already been approved";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            item.IsApproved = itemDto.IsApproved;
            item.ApproveOrRejectComments = itemDto.ApproveOrRejectComments;
            item.DateApproved = DateTime.Now;
            var user = await GetCurrentUser();
            item.ApprovedById = user.Id;
            var result = await _itemRepository.UpdateItemAsync(item);
            if (result.Successful)
            {
                var creatorOfItem = await _userManager.FindByIdAsync(item.CreatedById);
                //BackgroundJob.Enqueue(() => 
                await _autoGenServicePeriodService.FirstRecordOfNextServicePeriod(item.Id, item.ExpiredDate,
                    item.DateCreated, item.ReoccurenceValue, item.ReoccurenceFrequency);
                await _reminderService.SetReminderLog(item, creatorOfItem?.Email);
                Alert($"{item.Name} was approved successfully", Notifications.success);
                /*BackgroundJob.Enqueue(() =>*/
                await _notificationService.SendApproveItemNotifications(itemDto.Id, user);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllItemsCreatedByMyDepartment");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EscalateItem(string itemId)
        {
            var callingUrl = Request.Headers["Referer"].ToString();

            string returnUrl = IsLocalUrl(callingUrl) ? callingUrl : null;
            var item = await _itemRepository.GetItemByIdAsync(itemId);
            if (item is null)
            {
                var msg = $"Item with ID:{itemId} does not exist";
                Alert(msg, Notifications.error);
                if (returnUrl == null)
                {
                    return RedirectToAction("AllItemsCreatedByMyDepartment");
                }
                return Redirect(returnUrl);
            }

            var user = await GetCurrentUser() ?? new User();
            var itemCreator = await _userManager.FindByIdAsync(item.CreatedById) ?? new User();
            //remove EscalateItemReminder
            /*BackgroundJob.Enqueue(() =>*/ await _reminderService.EscalateItemReminder(item, itemCreator.Email, user.Id);

                Alert("Reminder sent successfully", Notifications.success);

            if (returnUrl == null)
            {
                return RedirectToAction("AllItemsCreatedByMyDepartment");
            }
            return Redirect(returnUrl);

        }


        [HttpGet]
        public IActionResult ItemReport()
        {
            return View(new List<ItemDto>());
        }

        [HttpPost]
        public async Task<IActionResult> ItemReport(DateTime startDate, DateTime endDate)
        {
            var items = await _itemRepository.GetAllApprovedItemsByDateAsync(startDate, endDate);
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
        private async Task<User> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }

    }
}
