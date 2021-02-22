using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using E_Tracker.Authorization;
using E_Tracker.CreateDto;
using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Dto;
using E_Tracker.Repository.PermissionRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    public class PermissionController : BaseController
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PermissionController> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public PermissionController(IPermissionRepository permissionRepository,
            IMapper mapper,
            ILogger<PermissionController> logger,
            RoleManager<Role> roleManager, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
            _logger = logger;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> PermissionList(string roleName)
        {
            var permissions = await _permissionRepository.GetAllPermissionAsync();
            var PermissionDto = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                var map = _mapper.Map<PermissionDto>(permission);
                PermissionDto.Add(map);
            }
            return View(PermissionDto);
        }

        [HttpGet]
        public IActionResult AddPermission()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPermission(CreatePermissionDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var permission = _mapper.Map<Permission>(model);
                permission.Id = Guid.NewGuid().ToString();
                var result = await _permissionRepository.CreatePermissionAsync(permission);
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
            return View();

        }

        [HttpGet]
        public async Task<IActionResult> AllNonActivePermission()
        {
            var permissions = await _permissionRepository.GetAllNonActivePermissionAsync();
            var permissionDto = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                var map = _mapper.Map<PermissionDto>(permission);
                permissionDto.Add(map);
            }
            return View("PermissionList", permissionDto);
        }

        [HttpGet]
        public async Task<IActionResult> RoleClaims(string roleId)
        {
            var model = new RoleClaimsDto
            {
                RoleId = roleId,
            };
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    Alert("Role was not found", Notifications.error);
                    return View();
                }
                model.IsRoleActive = role.IsActive;
                model.RoleName = role.Name;
                var roleClaims = await _roleManager.GetClaimsAsync(role);
              
                foreach (Claim claim in ClaimsStore.AllClaims)
                {
                    RoleClaim roleClaim = new RoleClaim
                    {
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                    };

                    if (roleClaims.Any(c => c.Value == claim.Value))
                    {
                        roleClaim.IsSelected = true;
                    }

                    model.Claims.Add(roleClaim);
                }
            }catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> ManageRoleClaims(string roleId)
        {
            var model = new RoleClaimsDto
            {
                RoleId = roleId,
            };
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    Alert("Role was not found", Notifications.error);
                    return View();
                }
                model.IsRoleActive = role.IsActive;
                var roleClaims = await _roleManager.GetClaimsAsync(role);
              
                foreach (Claim claim in ClaimsStore.AllClaims)
                {
                    RoleClaim roleClaim = new RoleClaim
                    {
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                    };

                    if (roleClaims.Any(c => c.Value == claim.Value))
                    {
                        roleClaim.IsSelected = true;
                    }

                    model.Claims.Add(roleClaim);
                }
            }catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageRoleClaims(RoleClaimsDto model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                Alert("Role was not found", Notifications.error);
                return View(model);
            }

            // Get all the user existing claims and delete them
            var claims = await _roleManager.GetClaimsAsync(role);
            IdentityResult result = null;

            if (claims.Count > 0)
            {
                foreach (var claim in claims)
                {
                    result = await _roleManager.RemoveClaimAsync(role, claim);
                }

                if (!result.Succeeded)
                {
                    Alert("Cannot remove user existing claim", Notifications.error);
                    return View(model);
                }
            }

            // Add all the claims that are selected on the UI
            var claimList = model.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimValue));
            if (claimList.Any())
            {
                foreach (var claim in claimList)
                {
                    result = await _roleManager.AddClaimAsync(role, claim);
                }
                if (!result.Succeeded)
                {
                    Alert("Cannot add selected claims role", Notifications.error);
                    return View(model);
                }
                else
                {
                    Alert($"Role claims updated successfully", Notifications.success);
                }
            }

            return RedirectToAction("RoleList", "Role");

        }
    }
}
