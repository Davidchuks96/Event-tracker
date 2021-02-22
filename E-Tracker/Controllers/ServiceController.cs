using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using E_Tracker.Authorization;
using E_Tracker.CreateDto;
using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Dto;
using E_Tracker.Repository.AutoGenServicePeriodRepository;
using E_Tracker.Repository.DepartmentRepo;
using E_Tracker.Repository.EmailRepo;
using E_Tracker.Repository.ItemRepository;
using E_Tracker.Repository.ItemTypeRepository;
using E_Tracker.Repository.ReminderRepo;
using E_Tracker.Repository.ServiceRepo;
using E_Tracker.Repository.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace E_Tracker.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly IMapper _mapper;
        private readonly IServiceRepository _serviceRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly UserManager<User> _userManager;
        private readonly IReminderService _reminderService;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IAutoGenServicePeriodService _autoGenServicePeriodService;
        private readonly IDepartmentRepository _departmentRepository;

        public ServiceController(ILogger<ServiceController> logger, IMapper mapper, IItemRepository itemRepository,
            IItemTypeRepository itemTypeRepository, IDepartmentRepository departmentRepository, 
            IServiceRepository serviceRepository, UserManager<User> userManager, IReminderService reminderService,
            IUserRepository userRepository, INotificationService notificationService, IAutoGenServicePeriodService autoGenServicePeriodService)
        {
            _logger = logger;
            _mapper = mapper;
            _serviceRepository = serviceRepository;
            _itemRepository = itemRepository;
            _itemTypeRepository = itemTypeRepository;
            _departmentRepository = departmentRepository;
            _userManager = userManager;
            _reminderService = reminderService;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _autoGenServicePeriodService = autoGenServicePeriodService;
        }
        private async Task<List<ServiceDto>> ServiceListDto(IEnumerable<Service> services)
        {
            var servicesDto = new List<ServiceDto>();
            foreach (var service in services)
            {
                var map = _mapper.Map<ServiceDto>(service);
                var item = await _itemRepository.GetItemByIdAsync(service.ItemId);
                var itemDto = _mapper.Map<ItemDto>(item);

                var createdUser = await _userRepository.GetUserByIdAsync(service.CreatedById);
                var approvedUser = await _userRepository.GetUserByIdAsync(service.ServiceApprovedById);
                var updatedUser = await _userRepository.GetUserByIdAsync(service.UpdatedById);

                map.Item = itemDto;
                map.CreatedBy = _mapper.Map<UserDto>(createdUser);
                map.ServiceApprovedBy = _mapper.Map<UserDto>(approvedUser);
                map.UpdatedBy = _mapper.Map<UserDto>(updatedUser);
                servicesDto.Add(map);
            }
            return servicesDto;
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewServices)]
        public async Task<IActionResult> Services()
        {
            var user = await GetCurrentUser();
            var serv = await _serviceRepository.GetAllServicesByServiceDepartmentAsync(user.DepartmentId);
            var servDto = new List<ServiceDto>();
            foreach (var services in serv)
            {
                var map = _mapper.Map<ServiceDto>(services);
                servDto.Add(map);
            }
            return View(servDto);
        }
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewServices)]
        public async Task<IActionResult> ServiceHistory()
        {
            var user = await GetCurrentUser();
            var serv = await _serviceRepository.GetApprovedServicesByServiceDepartmentAsync(user.DepartmentId);
            var servDto = new List<ServiceDto>();
            foreach(var services in serv)
            {
                var map = _mapper.Map<ServiceDto>(services);
                servDto.Add(map);
            }
            return View(servDto);
        }
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewServices)]
        public async Task<IActionResult> ServiceReport(DateTime? startDate, DateTime? endDate)
        {
            var services = await _serviceRepository.GetAllApprovedServicesByDateAsync(startDate,endDate);
            var servicesDto = new List<ServiceDto>();
            foreach(var serv in services)
            {
                var map = _mapper.Map<ServiceDto>(services);
                servicesDto.Add(map);
            }
            return View(servicesDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.UpdateService)]
        public async Task <IActionResult> CreateService(string itemId)
        {
            var user = await GetCurrentUser();
            var item = await _itemRepository.GetItemByIdAsync(itemId);
            ViewBag.Item = item;
            CreateServiceDto serviceDto = new CreateServiceDto()
            {
                ItemId = itemId,
                ServiceDepartmentId = user.DepartmentId,
                DateCreated = DateTime.Now,
                CreatedById = user.Id
            };
            return View(serviceDto);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.UpdateService)]
        public async Task<IActionResult> CreateService(CreateServiceDto serviceDto)
        {
            var item = await _itemRepository.GetItemByIdAsync(serviceDto.ItemId);
            ViewBag.Item = item;
            if (!ModelState.IsValid)
            {
                return View(serviceDto);
            }

            
            if (serviceDto.IsANewReoccurenceFrequency && serviceDto.NewReoccurenceValue <= 0)
            {
                ModelState.AddModelError("", "The New reoccurence value can not be zero");
                return View(serviceDto);
            }

            var service = _mapper.Map<Service>(serviceDto);
            
            if (item is null)
            {
                var msg = $"Item with ID:{service.ItemId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("Services");
            }

            if (item.IsApproved == false)
            {
                var msg = $"{item.Name} has not been approved";
                Alert(msg, Notifications.error);
                return RedirectToAction("Services");
            }

            var loggedOnUser = await GetCurrentUser();
            serviceDto.ServiceDepartmentId = loggedOnUser.DepartmentId;
            var result = await _serviceRepository.CreateServiceAsync(service);

            if (result.Successful)
            {
                //Update only services here, item would be updated once the service is approved
                service.NewExpiryDate = await _autoGenServicePeriodService.DetermineNewExpiryDate(service.IsANewCycle, service.IsANewReoccurenceFrequency, service.Id);
                await _serviceRepository.UpdateServicesAsync(service);
                await _notificationService.SendCreateServiceNotifications(item.Id, service.Id, loggedOnUser);
                Alert(result.Message, Notifications.success);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("Services");
        }

        [HttpGet]
        public async Task<IActionResult> AllServicesApproved()
        {
            var services = await _serviceRepository.GetAllServicesApprovedAsync();
            var servDto = await ServiceListDto(services);
            return View("Services", servDto);
        }

        [HttpGet]
        public async Task<IActionResult> AllServicesNotApproved()
        {
            var services = await _serviceRepository.GetAllNotApprovedServicesAsync();
            var servicesDto = await ServiceListDto(services);
            return View("Services", servicesDto);
        }

        [HttpGet]
        public async Task<IActionResult> AllServicesByDate(DateTime startDate, DateTime endDate)
        {
            var services = await _serviceRepository.GetAllApprovedServicesByDateAsync(startDate,endDate);
            var servicesDto = await ServiceListDto(services);
            return View("Services", servicesDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetServicesByServiceDepartment()
        {
            var loggedOnUser = await GetCurrentUser();
            var services = await _serviceRepository.GetApprovedServicesByServiceDepartmentAsync(loggedOnUser.DepartmentId);
            var servicesDto = await ServiceListDto(services);
            return View("Services", servicesDto);           
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ApproveService)]
        public async Task<IActionResult> ApproveService(string serviceId)
        {

            var service = await _serviceRepository.GetServiceByIdAsync(serviceId);
            var item = await _itemRepository.GetItemByIdAsync(service.ItemId);
            var user = await GetCurrentUser();
            if (item is null)
            {
                var msg = $"Item with ID:{service.ItemId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("ServiceHistory");
            }
            service.IsServiceApproved = true;
            service.Status = "Approved";
            service.ServiceApprovedById = user.Id;
            service.DateApproved = DateTime.Now;
            item.LastDateServiced = service.DateServiced;
            
            //After approval Record the new expiry Date and update the details on the Item model
            await _autoGenServicePeriodService.RecordApprovedNewExpiryDate(service.IsANewCycle, service.IsANewReoccurenceFrequency, service.Id);
            item.ExpiredDate = service.NewExpiryDate;
            if (service.IsANewReoccurenceFrequency)
            {
                item.ReoccurenceValue = service.NewReoccurenceValue;
                item.ReoccurenceFrequency = service.NewReoccurenceFrequency;
            }

            var result = await _itemRepository.UpdateItemAsync(item);

            if (result.Successful)
            {
                Alert($"Service of {item.Name} was approved successfully", Notifications.success);
                var loggedOnUser = await GetCurrentUser();
                await _notificationService.SendApproveServiceNotifications(item.Id, service.Id, loggedOnUser);
                var creatorOfItem = await _userManager.FindByIdAsync(item.CreatedById);
                //Record new expected service date
                

                //update reminder log only after service has been approved
                /*BackgroundJob.Enqueue(()=> */
                await _reminderService.DeleteReminderLog(item.Id);
                //This part throws a Newtonsoft.Json exception
                /*BackgroundJob.Enqueue(()=>*/ await _reminderService.SetReminderLog(item,creatorOfItem?.Email);
            }
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("Services");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItem)]
        public async Task<IActionResult> Details(string serviceId)
        {
            var service = await _serviceRepository.GetServiceByIdAsync(serviceId);
            if (service is null)
            {
                var msg = $"Item with ID:{serviceId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("Services");
            }
            var serviceDto = _mapper.Map<ServiceDto>(service);
            
            //The person who created the item
            var itemCreator = await _userRepository.GetUserByIdAsync(service.Item?.CreatedById);
            serviceDto.Item.CreatedBy = _mapper.Map<UserDto>(itemCreator);

            //The person who created the service
            var createdUser = await _userRepository.GetUserByIdAsync(service.CreatedById);
            serviceDto.CreatedBy = _mapper.Map<UserDto>(itemCreator);
            
            var approvedUser = await _userRepository.GetUserByIdAsync(service.ServiceApprovedById);
            serviceDto.ServiceApprovedBy = _mapper.Map<UserDto>(approvedUser);
            
            var deletedUser = await _userRepository.GetUserByIdAsync(service.DeletedById);
            serviceDto.DeletedBy = _mapper.Map<UserDto>(deletedUser);
            
            var updatedUser = await _userRepository.GetUserByIdAsync(service.UpdatedById);
            serviceDto.UpdatedBy = _mapper.Map<UserDto>(updatedUser);
            
            return View(serviceDto);
        }

        private async Task<User> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }
    }
}