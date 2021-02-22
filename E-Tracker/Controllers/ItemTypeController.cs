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
using E_Tracker.Repository.ItemRepository;
using E_Tracker.Repository.ItemTypeRepository;
using E_Tracker.Repository.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    
    public class ItemTypeController : BaseController
    {
        // GET: /<controller>/
        private readonly ILogger<ItemTypeController> _logger;
        private readonly IMapper _mapper;
        private readonly IItemRepository _itemRepository;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public ItemTypeController(ILogger<ItemTypeController> logger, IMapper mapper, IItemRepository itemRepository, IItemTypeRepository itemTypeRepository, UserManager<User> userManager, IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _itemRepository = itemRepository;
            _itemTypeRepository = itemTypeRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        private async Task<List<ItemTypeDto>> ItemTypeListDto(IEnumerable<ItemType> itemTypes)
        {
            var itemTypesDto = new List<ItemTypeDto>();
            foreach (var item in itemTypes)
            {              
                var map = _mapper.Map<ItemTypeDto>(item);
                var createdUser = await _userRepository.GetUserByIdAsync(item.CreatedById);
                var deletedUser = await _userRepository.GetUserByIdAsync(item.DeletedById);
                var updatedUser = await _userRepository.GetUserByIdAsync(item.UpdatedById);
                map.CreatedBy = _mapper.Map<UserDto>(createdUser);
                map.DeletedBy = _mapper.Map<UserDto>(deletedUser);
                map.UpdatedBy = _mapper.Map<UserDto>(updatedUser);
                itemTypesDto.Add(map);
            }
            return itemTypesDto;
        }
        // GET: /<controller>/
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewItemType)]
        public async Task<IActionResult> AllItemTypes()
        {
            var itemTypes = await _itemTypeRepository.GetAllItemTypesAsync();
            var itemTypesDto = await ItemTypeListDto(itemTypes);
            return View(itemTypesDto);
        }

        [Authorize(Policy = CustomClaimsValues.CreateItemType)]
        public IActionResult CreateItemType()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateItemType)]
        public async Task<IActionResult> ActivateItemType(string itemTypeId)
        {
            var itemType = await _itemTypeRepository.GetItemTypeByIdAsync(itemTypeId);
            if (itemType is null)
            {
                var msg = $"Item type with ID:{itemTypeId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemTypes");
            }
            var result = await _itemTypeRepository.ActivateItemTypeAsync(itemType);
            if (result.Successful)
                Alert(result.Message, Notifications.success);
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllNotActiveItemTypes");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateItemType)]
        public async Task<IActionResult> AllNotActiveItemTypes()
        {
            var itemTypes = await _itemTypeRepository.GetAllNotActiveItemTypesAsync();
            var itemTypesDto = await ItemTypeListDto(itemTypes);
            return View("AllItemTypes", itemTypesDto);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.CreateItemType)]
        public async Task<IActionResult> CreateItemType(CreateItemTypeDto itemTypeDto)
        {
            if (!ModelState.IsValid)
            {
                return View(itemTypeDto);
            }
            var itemTypecheck = await _itemTypeRepository.GetItemTypeByNameAsync(itemTypeDto.Name);
            if (itemTypecheck == null)
            {
                var itemType = _mapper.Map<ItemType>(itemTypeDto);
                var user = await GetCurrentUser();
                itemType.CreatedById = user.Id;
                var result = await _itemTypeRepository.CreateItemTypeAsync(itemType);
                if (result.Successful)
                    Alert(result.Message, Notifications.success);
                else
                    Alert(result.Message, Notifications.error);
                return RedirectToAction("AllItemTypes");
            }
            else
            {
                Alert($"Item Type with Name: {itemTypeDto.Name} already exists", Notifications.error);
                return View(itemTypeDto);
            }

        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.UpdateItemType)]
        public async Task<IActionResult> UpdateItemType(string itemTypeId)
        {
            var user = User;
           
            var itemType = await _itemTypeRepository.GetItemTypeByIdAsync(itemTypeId);
            if (itemType is null)
            {
                var msg = $"Item type with ID:{itemTypeId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemTypes");
            }
            var itemTypeDto = _mapper.Map<ItemTypeDto>(itemType);
            return View(itemTypeDto);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.UpdateItemType)]
        public async Task<IActionResult> UpdateItemType(ItemTypeDto itemTypeDto)
        {
            if (!ModelState.IsValid)
            {
                return View(itemTypeDto);
            }
            var itemType = _mapper.Map<ItemType>(itemTypeDto);
            //var itemType = await _itemTypeRepository.GetItemTypeByIdAsync(itemTypeDto.Id);
            //itemType.DateUpdated = DateTime.Now;
            //itemType.Name = itemTypeDto.Name;
            var user = await GetCurrentUser();
            itemType.UpdatedById = user.Id;
            var result = await _itemTypeRepository.UpdateItemTypeAsync(itemType);
            if (result.Successful)
                Alert(result.Message, Notifications.success);
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllItemTypes");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.DeleteItemType)]
        public async Task<IActionResult> DeleteItemType(string itemTypeId)
        {
            var itemType = await _itemTypeRepository.GetItemTypeByIdAsync(itemTypeId);
            if (itemType is null)
            {
                var msg = $"Item type with ID:{itemTypeId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("AllItemTypes");
            }
            var user = await GetCurrentUser();
            itemType.DeletedById = user.Id;
            var result = await _itemTypeRepository.DeleteItemTypeAsync(itemType);
            if (result.Successful)
                Alert(result.Message, Notifications.success);
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("AllItemTypes");
        }

        public async Task<User> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }
    }
}
