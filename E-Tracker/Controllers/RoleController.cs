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
using E_Tracker.Repository.RoleRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleController> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleController(IRoleRepository roleRepository,
            IMapper mapper, UserManager<User> userManager,
            ILogger<RoleController> logger, RoleManager<Role> roleManager)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewRole)]
        public async Task<IActionResult> RoleList()
        {
            var roles = await _roleRepository.GetAllRoleAsync();
            var rolesDto = new List<RoleDto>();
            foreach (var role in roles)
            {
                var map = _mapper.Map<RoleDto>(role);
                rolesDto.Add(map);
            }
            return View(rolesDto);
        }
        
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateRole)]
        public async Task<IActionResult> DeactivatedRoleList()
        {
            var roles = await _roleRepository.GetAllDeactivatedRoleAsync();
            var rolesDto = new List<RoleDto>();
            foreach (var role in roles)
            {
                var map = _mapper.Map<RoleDto>(role);
                rolesDto.Add(map);
            }
            return View(rolesDto);
        }
        
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateRole)]
        public async Task<IActionResult> RestoreRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                Alert("Role does not exist", Notifications.error);
                return RedirectToAction("DeactivatedRoleList");
            }
            role.IsActive = true;
            var currentUser = await GetCurrentUserAsync();
            role.UpdatedById = currentUser.Id;

            var result = await _roleRepository.UpdateRoleAsync(role);
            if (result.successful)
            {
                return RedirectToAction("RoleList");
            }
            return RedirectToAction("DeactivatedRoleList");
        }

        // GET: /<controller>/
        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.CreateRole)]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.CreateRole)]
        public async Task<IActionResult> AddRole(CreateRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var role = _mapper.Map<Role>(model);
                role.Id = Guid.NewGuid().ToString();               
                var result = await _roleRepository.CreateRoleAsync(role);
                if (result.successful)
                {
                    Alert(result.message, Notifications.success);
                    return RedirectToAction("RoleList");
                }
                else
                {
                    Alert(result.message, Notifications.error);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.EditRole)]
        public async Task<IActionResult> EditRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                Alert("Role does not exist", Notifications.error);
                RedirectToAction("RoleList");
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            var roleDto = _mapper.Map<RoleDto>(role);
            roleDto.Claims = claims.Select(c => c.Value).ToList();
            return View(roleDto);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.EditRole)]
        public async Task<IActionResult> EditRole(RoleDto model)
        {   
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                Alert("Role does not exist", Notifications.error);
                RedirectToAction("RoleList");
            }

            try
            {
                var currentUser = await GetCurrentUserAsync();
                role.UpdatedById = currentUser.Id;
                role.Name = model.Name;
                var result = await _roleRepository.UpdateRoleAsync(role);
                if (result.successful)
                {
                    Alert(result.message, Notifications.success);
                }
                else
                {
                    Alert(result.message, Notifications.error);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.DeleteRole)]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                Alert("This role does not exist", Notifications.error);
                return RedirectToAction("RoleList");
            }

            try
            {
                var currentUser = GetCurrentUserAsync().Result;
                role.DeletedById = currentUser.Id;
                var result = await _roleRepository.DeleteRoleAsync(role);
                
                //remove all users from 'deleted' role
                var users = await _userManager.GetUsersInRoleAsync(role.Name);
                foreach (var user in users)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                
                if (result.successful)
                {
                    Alert(result.message, Notifications.success);
                    return RedirectToAction("RoleList");
                }
                else
                {
                    Alert(result.message, Notifications.error);
                    return RedirectToAction("RoleList");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return RedirectToAction("RoleList");
        }
        
        //[HttpGet]
        //public async Task<IActionResult> DeletedRoles()
        //{
        //    var role = await _roleManager.FindByIdAsync(roleId);

        //    if (role == null)
        //    {
        //        Alert("This role does not exist", Notifications.error);
        //        return RedirectToAction("RoleList");
        //    }

        //    try
        //    {
        //        var currentUser = GetCurrentUserAsync().Result;
        //        role.DeletedById = currentUser.Id;
        //        var result = await _roleRepository.DeleteRoleAsync(role);
        //        if (result.successful)
        //        {
        //            Alert(result.message, Notifications.success);
        //            return RedirectToAction("RoleList");
        //        }
        //        else
        //        {
        //            Alert(result.message, Notifications.error);
        //            return RedirectToAction("RoleList");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = ex.Message;
        //    }
        //    return RedirectToAction("RoleList");
        //}

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

    }
}
