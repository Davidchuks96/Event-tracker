using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Tracker.Data;
using E_Tracker.Dto;
using E_Tracker.Repository.DepartmentRepo;
using E_Tracker.Repository.ItemRepository;
using E_Tracker.Repository.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
   [Authorize]
    public class AdminController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public AdminController
            (IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserController> logger,
            UserManager<User> userManager, IItemRepository itemRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _itemRepository = itemRepository;
        }
        // GET: /<controller>/
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var user = await GetCurrentUser();
            _logger.LogInformation($"-----{user?.Email} requested to View Dashboard page -----");


            var allItems = await _itemRepository.GetItemsByCreatedByDepartmentIdAsync(user.DepartmentId);
            var expiredItems = allItems.Where(x => x.IsApproved == true && x.ExpiredDate <= DateTime.Now);
            var activeItems = allItems.Where(x => x.IsApproved == true && x.ExpiredDate >= DateTime.Now);
            var ApproachingExpiry = allItems.Where(x => x.IsApproved == true && x.ExpiredDate != DateTime.MinValue
                                                                            && x.ExpiredDate.AddDays(-14) <= DateTime.Now ).Take(5); 
            var dashboard = new DashboardDto
            {
                AllItems = allItems.Count(),
                AllExpiredItems = expiredItems.Count(),
                AllActiveItems = activeItems.Count(),
                ApproachingExipringDate = ApproachingExpiry.ToList()
            };
            return View(dashboard);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(AdminController.Dashboard), "Dashboard");
            }
        }

        private async Task<User> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }
    }
}
