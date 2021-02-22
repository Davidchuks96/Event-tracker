using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using E_Tracker.Data;
using E_Tracker.Dto;
using E_Tracker.Repository.EmailRepo;
using E_Tracker.Repository.ItemRepository;
using E_Tracker.Repository.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Tracker.Controllers
{
    [AllowAnonymous]
    [Route("Notification/")]
    public class NotificationController : Controller
    {
        private INotificationService _notificationService;
        private UserManager<User> _userManager;
        private readonly IItemRepository _itemRespository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public NotificationController(INotificationService notificationService,
                                        UserManager<User> userManager, IItemRepository itemRespository, 
                                        IUserRepository userRepository, IMapper mapper)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _itemRespository = itemRespository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("GetNotification")]
        public IActionResult GetNotification()
        {
            var userId = _userManager.GetUserId(User);
            var notification = _notificationService.GetUserNotifications(userId);

            return Ok(new { UserNotification = notification, Count = notification.Count });
        }
        
        [HttpGet("AllNotifications")]
        public async Task<IActionResult> AllUnReadNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifications = _notificationService.GetUserNotifications(user.Id);
            ViewBag.Username = user.Surname + " " + user.OtherNames;
            return View("AllNotifications",notifications);
        }

        [HttpGet("ReadNotification")]
        public async Task<IActionResult> ReadNotification(int notificationId)
        {
            _notificationService.ReadNotification(notificationId, _userManager.GetUserId(User));
            var items = await _itemRespository.GetAllNotApprovedItemsAsync();
            var itemsDto = await ItemListDto(items);
            return View("AllItems", itemsDto);
        }
        
        [HttpGet("ReadNotificationAjax")]
        public async Task<IActionResult> ReadNotificationAjax(int notificationId)
        {
            _notificationService.ReadNotification(notificationId, _userManager.GetUserId(User));
            var items = await _itemRespository.GetAllNotApprovedItemsAsync();
            var itemsDto = await ItemListDto(items);
            return Ok(new {Done = "Notification read" });
        }

        private async Task<List<ItemDto>> ItemListDto(IEnumerable<Item> items)
        {
            var itemsDto = new List<ItemDto>();
            foreach (var item in items)
            {
                //var itemType = await _itemTypeRepository.GetItemTypeByIdAsync(item.ItemTypeId);
                //var dept = await _departmentRepository.GetDepartmentByIdAsync(item.DepartmentId);
                //item.ItemType = itemType;
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
    }

}