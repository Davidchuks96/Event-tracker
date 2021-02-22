using AutoMapper;
using E_Tracker.Authorization;
using E_Tracker.CreateDto;
using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Dto;
using E_Tracker.Repository.DepartmentRepo;
using E_Tracker.Repository.EmailRepo;
using E_Tracker.Repository.UserRepository;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly INotificationService _notificationService;

        public UserController(IUserRepository userRepository,
            IMapper mapper, SignInManager<User> signInManager,
            IDepartmentRepository departmentRepository,
            IEmailRepository emailRepository,
            ILogger<UserController> logger, UserManager<User> userManager,
            RoleManager<Role> roleManager,
            INotificationService notificationService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _signInManager = signInManager;
            _logger = logger;
            _departmentRepository = departmentRepository;
            _emailRepository = emailRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationService = notificationService;
        }

        private async Task<List<UserDto>> UserListDto(IEnumerable<User> users)
        {
            var userDto = new List<UserDto>();
            foreach (var user in users)
            {
                //var itemType = await _itemTypeRepository.GetItemTypeByIdAsync(item.ItemTypeId);
                var dept = await _departmentRepository.GetDepartmentByIdAsync(user.DepartmentId);
                //item.ItemType = itemType;
                user.Department = dept;
                var map = _mapper.Map<UserDto>(user);
                userDto.Add(map);
            }

            return userDto;
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewUser)]
        public async Task<IActionResult> UserList()
        {
            var users = await _userRepository.GetAllUserAsync();
            var currentUser = await GetCurrentUserAsync();
            List<UserDto> userDtos = new List<UserDto>();
            if (await _userManager.IsInRoleAsync(currentUser, RolesList.SuperAdmin))
            {
                userDtos = await UserListDto(users);
            }
            else
            {
                var departmentUsers = users.Where(usr => usr.DepartmentId == currentUser.DepartmentId);
                userDtos = await UserListDto(departmentUsers);
            }

            return View(userDtos);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateUser)]
        public async Task<IActionResult> DeactivatedUserList()
        {
            var users = await _userRepository.GetAllDeactivatedUsersAsync();
            var currentUser = await GetCurrentUserAsync();
            List<UserDto> userDtos = new List<UserDto>();
            if (await _userManager.IsInRoleAsync(currentUser, RolesList.SuperAdmin))
            {
                userDtos = await UserListDto(users);
            }
            else
            {
                var departmentUsers = users.Where(usr => usr.DepartmentId == currentUser.DepartmentId);
                userDtos = await UserListDto(departmentUsers);
            }

            return View(userDtos);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateUser)]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Alert("User does not exist", Notifications.error);
                return RedirectToAction("DeactivatedUserList");
            }
            user.IsActive = true;
            var currentUser = await GetCurrentUserAsync();
            user.UpdatedById = currentUser.Id;

            var result = await _userRepository.UpdateUserAsync(user);
            if (result.successful)
            {
                return RedirectToAction("UserList");
            }
            return RedirectToAction("DeactivatedUserList");
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var currentUser = await GetCurrentUserAsync();
            var dept = await _departmentRepository.GetDepartmentByIdAsync(currentUser.DepartmentId);
            currentUser.Department = dept;
            var roles = await _userManager.GetRolesAsync(currentUser);
            var userDto = _mapper.Map<UserDto>(currentUser);
            foreach (var role in roles)
            {
                userDto.Roles.Add(role);
            }
            return View(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            var dept = await _departmentRepository.GetDepartmentByIdAsync(user.DepartmentId);
            user.Department = dept;
            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            if (ModelState.IsValid)
            {

                foreach (var role in roles)
                {
                    userDto.Roles.Add(role);
                }
                if (user == null)
                {
                    //return RedirectToAction("Login");
                    ModelState.AddModelError("", "Something went wrong! Kindly logout and try again");
                    return View("UserProfile",userDto);
                }

                // ChangePasswordAsync changes the user password
                var result = await _userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("UserProfile", userDto);
                }

                // Upon successfully changing the password refresh sign-in cookie
                await _signInManager.RefreshSignInAsync(user);
                Alert("Password changed successfully", Notifications.success);
            }

            return View("UserProfile", userDto);
        }

        [HttpGet]
        [Authorize(Roles = RolesList.SuperAdmin)]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Alert("User does not exist", Notifications.error);
                return RedirectToAction("UserList");
            }
            user.Department = await _departmentRepository.GetDepartmentByIdAsync(user.DepartmentId);
            var userDto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                userDto.Roles.Add(role);
            }
            return View(userDto);
        }

        [HttpPost]
        [Authorize(Roles = RolesList.SuperAdmin)]
        public async Task<IActionResult> ResetPassword(string userId, string newPassword, string confirmPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Alert("User does not exist", Notifications.error);
                return View(new UserDto());
            }
            user.Department = await _departmentRepository.GetDepartmentByIdAsync(user.DepartmentId);
            var userDto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                userDto.Roles.Add(role);
            }
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(string.Empty,"Passwords do not match");
                return View(userDto);
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (result.Succeeded)
            {
                user.IsFirstLogin = true;
                var resultTuple = await _userRepository.UpdateUserAsync(user);
                if (resultTuple.result.Succeeded)
                {
                    Alert(resultTuple.message, Notifications.success);
                }
                else
                {
                    foreach (var error in resultTuple.result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    Alert(resultTuple.message, Notifications.error);
                }
                Alert("Password reset successful", Notifications.success);
                return RedirectToAction("UserList");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                Alert("Something went wrong, please try again later", Notifications.error);
            }
            //ModelState.AddModelError(string.Empty, "Something went wrong, please try again later");
            return View(userDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewUser)]
        public async Task<IActionResult> UserDetails(string userId)
        {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Alert("User not found", Notifications.error);
                return View(new UserDto());
            }
            var createdBy = await _userManager.FindByIdAsync(user.CreatedById);
            var updatedBy = await _userManager.FindByIdAsync(user.UpdatedById);
            var deletedBy = await _userManager.FindByIdAsync(user.DeletedById);
            var model = _mapper.Map<UserDto>(user);
            model.IsActive = user.IsActive;
            model.CreatedByFullName = createdBy?.Surname + " " + createdBy?.OtherNames;
            model.UpdatedByFullName = updatedBy?.Surname + " " + updatedBy?.OtherNames;
            model.DeletedByFullName = deletedBy?.Surname + " " + deletedBy?.OtherNames;
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                model.Roles.Add(role);
            }
            var department = await _departmentRepository.GetDepartmentByIdAsync(user.DepartmentId);
            //var user = await _userManager.FindByIdAsync(user.);
            model.Department = _mapper.Map<DepartmentDto>(department);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = RolesList.SuperAdmin)]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Alert("User not found", Notifications.error);
                return View();
            }

            var model = new List<UserRolesDto>();

            foreach (var role in _roleManager.Roles.Where(r => r.IsActive == true).ToList())
            {
                var userRolesDto = new UserRolesDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesDto.IsSelected = true;
                }
                else
                {
                    userRolesDto.IsSelected = false;
                }

                model.Add(userRolesDto);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RolesList.SuperAdmin)]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesDto> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Alert("User not found", Notifications.error);
                return View();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                Alert("Cannot remove user's existing roles", Notifications.error);
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                Alert("Cannot add selected roles to user", Notifications.error);
                return View(model);
            }
            else
            {
                Alert($"User roles updated successfully", Notifications.success);
            }

            return RedirectToAction("UserList", "User");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.CreateUser)]
        public async Task<IActionResult> AddUser()
        {
            var departments = await _departmentRepository.GetAllDepartmentAsync();
            ViewBag.Departments = departments;
            return View();
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.CreateUser)]
        public async Task<IActionResult> AddUser(CreateUserDto model)
        {
            var departments = await _departmentRepository.GetAllDepartmentAsync();
            ViewBag.Departments = departments;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = _mapper.Map<User>(model);
                user.Id = Guid.NewGuid().ToString();
                var currentUser = await GetCurrentUserAsync();
                user.CreatedById = currentUser.Id;
                var result = await _userRepository.CreateUserAsync(user, model.Password);
                if (result.successful)
                {
                    await AddUserToUserRoleAsync(user.Email);

                    //generate a confirmation email token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    _logger.Log(LogLevel.Information, confirmationLink);

                    var mail = new MailLog();
                    mail.RecipientName = user.OtherNames;
                    mail.Receiver = model.Email;
                    mail.Subject = "EMAIL CONFIRMATION";
                    mail.MessageBody =$"{confirmationLink}";


                    BackgroundJob.Enqueue(() => _emailRepository.SendMailAsync(mail));
                    ///*var emailResult =*/ /*BackgroundJob.Enqueue(()=>*/ var success =_notificationService.SendMail(mail);
                    //if (!success)
                    //{
                    //    ////let them know it is an email issue
                    //    Alert("User registered, but an error occured while sending confirm email. Please try again later", Notifications.error);
                    //}
                    //else
                    //{
                    //}
                    Alert("Registration successful! Please confirm email", Notifications.success);
                }
                else
                {
                    foreach (var error in result.result.Errors)
                    {
                        ModelState.AddModelError(string.Empty,error.Description);
                    }
                    Alert(result.message, Notifications.error);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                _logger.LogError($"The source {ex?.Source} " + $"threw an exception {ex.ToString()}");
                return View("Error");
            }
            return View(model);
        }

        private async Task AddUserToUserRoleAsync(string userEmail)
        {
            User registeredUser = await _userManager.FindByEmailAsync(userEmail);
            if (registeredUser != null)
            {
                IdentityResult result = await _userManager.AddToRoleAsync(registeredUser, "User");

                if (!result.Succeeded)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.UpdateUser)]
        public async Task<IActionResult> UpdateUser(string userId)
        {
            var departments = await _departmentRepository.GetAllDepartmentAsync();
            ViewBag.Departments = departments;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Alert("User does not exist", Notifications.error);
                return RedirectToAction("UserList");
            }

            var roles = await _userManager.GetRolesAsync(user);
            user.Department = await _departmentRepository.GetDepartmentByIdAsync(user.DepartmentId);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles;
            return View(userDto);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.UpdateUser)]
        public async Task<IActionResult> UpdateUser(UserDto model)
        {
            var departments = await _departmentRepository.GetAllDepartmentAsync();
            ViewBag.Departments = departments;

            if (!ModelState.IsValid)
            {
                model.Roles = ViewBag.Roles;
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.Id);
            try
            {

                if (user == null)
                {
                    Alert("User does not exist", Notifications.error);
                    return RedirectToAction("UserList");
                }
                else
                {
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    user.Surname = model.Surname;
                    user.OtherNames = model.OtherNames;
                    user.DepartmentId = model.DepartmentId;
                }

                var currentUser = await GetCurrentUserAsync();

                user.UpdatedById = currentUser.Id;
                var result = await _userRepository.UpdateUserAsync(user);
                if (result.successful)
                {
                    Alert(result.message, Notifications.success);
                }
                else
                {
                    foreach (var error in result.result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
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
        [Authorize(Policy = CustomClaimsValues.DeleteUser)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Alert ("This user does not exist", Notifications.error);
                return RedirectToAction("UserList");
            }

            try
            {
                var currentUser = GetCurrentUserAsync().Result;
                user.DeletedById = currentUser.Id;
                var result = await _userRepository.DeleteUserAsync(user);
                if (result.successful)
                {
                    Alert(result.message, Notifications.success);
                    return RedirectToAction("UserList");
                }
                else
                {
                    Alert(result.message, Notifications.error);
                    return RedirectToAction("UserList");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return RedirectToAction("UserList");
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
