using System.Threading.Tasks;
using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Dto;
using E_Tracker.Repository.EmailRepo;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailRepository _emailRepository;
        private readonly INotificationService _notificationService;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager,
             ILogger<AccountController> logger, IEmailRepository emailRepository,
            INotificationService notificationService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailRepository = emailRepository;
            _notificationService = notificationService;
        }
        // GET: /<controller>/
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            LoginDto model = new LoginDto();
            model.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if(user == null || !(await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return View(model);
                }
                if (user.IsActive == false)
                {
                    ModelState.AddModelError(string.Empty, "Your account has been deactivated, Kindly contact your administrator!");
                    return View(model);
                }
                if (!user.EmailConfirmed)
                {
                    ViewBag.IsEmailNotConfirmned = true;
                    ModelState.AddModelError(string.Empty, "Email not confirmed, Kindly check your email to confirm");
                    return View(model);
                }
                if (user.IsFirstLogin == true)
                {
                    ChangePasswordDto changePasswordDto = new ChangePasswordDto { Email = model.Email};
                    return View("FirstLoginPasswordChange", changePasswordDto);
                }
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation($"----- {user.Email} signed in successfully -----");

                    if (string.IsNullOrEmpty(model.ReturnUrl) || string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    //to prevent foreign redirect urls
                    return RedirectToLocal(model.ReturnUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                _logger.LogWarning($"The supplied URL {returnUrl} is not local.");
                return RedirectToAction(nameof(AdminController.Dashboard), "Admin");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            TempData["Message"] = $"Email sent to {email}! Kindly check your inbox or spam to confirm your registration";
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            string confirmEmailTokenLink = await GenerateEmailTokenLinkAsync(user);
            _logger.Log(LogLevel.Warning, confirmEmailTokenLink);
            var mail = new MailLog();
            mail.RecipientName = user.OtherNames;
            mail.Receiver = email;
            mail.Subject = "EMAIL CONFIRMATION";
            mail.MessageBody = $"{confirmEmailTokenLink}";

            BackgroundJob.Enqueue(() => _emailRepository.SendMailAsync(mail));
            //_notificationService.SendMail(mail);

            return RedirectToAction("Login");
        }

        private async Task<string> GenerateEmailTokenLinkAsync(User user)
        {
            string cToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string cTokenLink = Url.Action("ConfirmEmail", "Account",
                values: new { userId = user.Id, token = cToken },
                protocol: HttpContext.Request.Scheme);

            return cTokenLink;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ViewBag.Error = $"The user ID {userId} is invalid";
                ModelState.AddModelError(string.Empty, $"The user ID {userId} is invalid");
                return RedirectToAction("Login", "Account");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if(result.Succeeded)
            {
                return View("Successful", "Account");
            }

            //View bag error message
            ModelState.AddModelError(string.Empty, "Something went wrong, kindly try again");
            return RedirectToAction("Login", "Account");
        } 
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> FirstLoginPasswordChange(ChangePasswordDto changePassword)
        {
            if (!ModelState.IsValid)
            {
                return View(changePassword);
            }
            var user = await _userManager.FindByEmailAsync(changePassword.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong, please try again");
                return View(changePassword);
            }
            var result  = await _userManager.CheckPasswordAsync(user,changePassword.CurrentPassword);

            if(!result)
            {
                ModelState.AddModelError(string.Empty, "Invalid current password provided");
                return View(changePassword);
            }

            if (changePassword.CurrentPassword == changePassword.NewPassword)
            {
                ModelState.AddModelError(string.Empty, "Current password cannot be the same as the New Password");
                return View(changePassword);
            }
            
            IdentityResult identityResult =  await _userManager.ChangePasswordAsync(user,changePassword.CurrentPassword,changePassword.NewPassword);
            if (identityResult.Succeeded)
            {
                user.IsFirstLogin = false;
                await _userManager.UpdateAsync(user);
                var signInResult =  await _signInManager.PasswordSignInAsync(changePassword.Email, changePassword.NewPassword, false, false);
                Alert("Password was changed successfully", Notifications.success);
                return RedirectToAction("Dashboard", "Admin");
            }
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            //ModelState.AddModelError(string.Empty, "Something went wrong, please try again");
            return View(changePassword);
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    //return RedirectToAction("Login");
                    ModelState.AddModelError("", "Something went wrong! Kindly logout and try again");
                    return View("UserProfile");
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
                    return View("UserProfile");
                }

                // Upon successfully changing the password refresh sign-in cookie
                await _signInManager.RefreshSignInAsync(user);
                Alert("Password changed successfully", Notifications.success);
            }

            return View("UserProfile");
        }
    }
}
