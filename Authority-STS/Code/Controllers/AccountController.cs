using AuthoritySTS.Data;
using AuthoritySTS.Models;
using AuthoritySTS.Models.AccountViewModels;
using AuthoritySTS.Services;
using AuthoritySTS.Services.Interface;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Logging.ILogger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace AuthoritySTS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IPersistedGrantService _persistedGrantService;
        private readonly IBuildLoginView _buildLoginView;
        private readonly string _angularHomeUrl;
        private readonly string _angularResetPasswordUrl;
        private readonly string _angularPostbackUrl;
        private readonly IEmailSender _emailSender;
        private readonly string _angularErrorPageUrl;
        private readonly string _catalogConnectionString;
        private readonly TriggerDbContext _triggerDbContext;

        public AccountController(
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IPersistedGrantService persistedGrantService, IEmailSender emailSender,
            ILogger logger, IIdentityServerInteractionService interaction, IBuildLoginView buildLoginView, IConfiguration iConfiguration, TriggerDbContext triggerDbContext
            )
        {
            _userManager = userManager;
            _persistedGrantService = persistedGrantService;
            _signInManager = signInManager;
            _logger = logger;
            _interaction = interaction;
            _buildLoginView = buildLoginView;
            _emailSender = emailSender;
            _angularHomeUrl = iConfiguration.GetSection("appSettings").GetSection("AngularHomeUrl").Value;
            _angularResetPasswordUrl = iConfiguration.GetSection("appSettings").GetSection("AngularResetPasswordUrl").Value;
            _angularPostbackUrl = iConfiguration.GetSection("appSettings").GetSection("AngularPostBackUrl").Value;
            _angularErrorPageUrl = iConfiguration.GetSection("appSettings").GetSection("AngularErrorPageUrl").Value;
            _catalogConnectionString = iConfiguration.GetConnectionString("1AuthorityConnection");
            _triggerDbContext = triggerDbContext;
        }

        
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            _logger.Info("IP address and hostname is - " + GetLocalIPAddress() + " " + Dns.GetHostName());
            _logger.Info("Login method start step-1");

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var vm = await _buildLoginView.BuildLoginViewModelAsync(returnUrl, context);

            _logger.Info("vm return url" + vm.ReturnUrl);
            if (!vm.EnableLocalLogin && vm.ExternalProviders.Count() >= 1)
            {
                _logger.Info("Login method start step-2");
                _logger.Info("returnUrl" + returnUrl);

                string acrValue = HttpUtility.ParseQueryString(returnUrl).Get("acr_values");
                string loginHintValue = HttpUtility.ParseQueryString(returnUrl).Get("login_hint");
                
                if(acrValue==null)
                {
                    acrValue= vm.ExternalProviders.FirstOrDefault(x => x.AuthenticationScheme.Contains("AzureAD"))?.AuthenticationScheme??String.Empty;         
                }
                _logger.Info("acr_value:" + acrValue);
                if(context.ClientId.ToString() == "TriggerAzureMobileApp")
                {
                    _logger.Info("Azure ad login redirect to action");
                    return RedirectToAction(ControllerResources.ExternalLogin, ControllerResources.External, new { returnUrl, provider = acrValue, loginHint = loginHintValue });
                }
                else if (context.ClientId.ToString() == "TriggerAzureiOSApp")
                {
                    _logger.Info("Azure ad ios login redirect to action");
                    return RedirectToAction(ControllerResources.ExternalLogin, ControllerResources.External, new { returnUrl, provider = acrValue, loginHint = loginHintValue });
                }
                else
                {
                    _logger.Info("Google login redirect to action");
                    return RedirectToAction(ControllerResources.ExternalLogin, ControllerResources.External, new { returnUrl, provider = vm.ExternalProviders.Where(x => x.AuthenticationScheme.Contains("Google")).FirstOrDefault().AuthenticationScheme });
                }
            }
            else
            {
                ViewData["AngularHomeUrl"] = _angularHomeUrl;
                _logger.Info("angularHomeUrl " + _angularHomeUrl);
                ViewData["AngularResetPasswordUrl"] = _angularResetPasswordUrl;
                ViewData["AngularPostBackUrl"] = _angularPostbackUrl;
                _logger.Info("AngularPostBackUrl " + _angularPostbackUrl);
                ViewData["AngularErrorPageUrl"] = _angularErrorPageUrl;
            }
            _logger.Info("Login method end step-1");
            _logger.Info("IP address and hostname is - " + GetLocalIPAddress() + " " + Dns.GetHostName());
            _logger.Info("Status Code : " + HttpContext.Response.StatusCode);
            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            _logger.Info("IP address and hostname is - " + GetLocalIPAddress() + " " + Dns.GetHostName());
            ViewData["AngularHomeUrl"] = _angularHomeUrl;
            ViewData["AngularResetPasswordUrl"] = _angularResetPasswordUrl;

            _logger.Info("angularHomeUrl " + _angularHomeUrl);

           if (ModelState.IsValid)
           {
                var user = await _userManager.FindByEmailAsync(model.UserName);
                if (user == null || !(user.EmailConfirmed))
                {
                    ViewBag.Message = "User does not exist";
                    _logger.Info("If user is null or user's email is not confirmed");
                    return View(await _buildLoginView.BuildLoginViewModelAsync(model));
                }
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberLogin, lockoutOnFailure: Convert.ToBoolean(ViewData["LockNewUser"]));
                _logger.Info("IP address and hostname is - " + GetLocalIPAddress() + " " + Dns.GetHostName());
                _logger.Info("Status Code : " + HttpContext.Response.StatusCode);
                return await RedirectUser(result, model);             
           }        
           return View(await _buildLoginView.BuildLoginViewModelAsync(model));
        }


        private async Task<IActionResult> RedirectUser(Microsoft.AspNetCore.Identity.SignInResult result, LoginInputModel model)
        {
            _logger.Info("IP address and hostname is - " + GetLocalIPAddress() + " " + Dns.GetHostName());
            if (result.Succeeded)
            {
                _logger.Info("User logged in");

                if (model.ReturnUrl != null)
                {
                    _logger.Info("ReturnUrl is - "+ model.ReturnUrl);
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    Response.Redirect(_angularPostbackUrl);
                    _logger.Info("Redirected to - " +_angularPostbackUrl);
                }
            }
            if (result.IsLockedOut)
            {
                _logger.Warn("This account has been locked out, please try again later.");
                ViewBag.Message = "This account has been locked out, please try again later.";
                return View(await _buildLoginView.BuildLoginViewModelAsync(model));
            }
            else
            {
                ViewBag.Message = "Invalid password";
                return View(await _buildLoginView.BuildLoginViewModelAsync(model));
            }
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            try
            {
                var model = new LogoutViewModel
                {
                    LogoutId = logoutId
                };
                if (!User.Identity.IsAuthenticated)
                {
                    // if the user is not authenticated, then just show logged out page
                    return await Logout(model);
                }
                var context = await _interaction.GetLogoutContextAsync(logoutId);
                if (context?.ShowSignoutPrompt == false)
                {
                    // it's safe to automatically sign-out
                    return await Logout(model);
                }
                // show the logout prompt. this prevents attacks where the user is automatically signed out by another malicious web page.
                return Redirect(_angularHomeUrl);
            }
            catch (Exception)
            {
                return Redirect(_angularHomeUrl);
            }
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            var subjectId = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                subjectId = HttpContext.User.Identity.GetSubjectId();
            }
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                if (model.LogoutId == null)
                {
                    model.LogoutId = await _interaction.CreateLogoutContextAsync();
                }
                await _signInManager.SignOutAsync();
            }

            // delete authentication cookie
            await _signInManager.SignOutAsync();
            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);
            if (!string.IsNullOrEmpty(subjectId))
            {
                await _persistedGrantService.RemoveAllGrantsAsync(subjectId, logout.ClientId);
            }
            return Redirect(logout.PostLogoutRedirectUri);
        }

        /// <summary>
        /// Show forgot password page
        /// </summary>
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewData["AuthUrl"] = _angularPostbackUrl;
            ViewData["AngularHomeUrl"] = _angularHomeUrl;
            return View();
        }

        /// <summary>
        /// Handle forgot password postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewData["AuthUrl"] = _angularPostbackUrl;
            ViewData["AngularHomeUrl"] = _angularHomeUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                ViewBag.ErrorMessage = "User does not exist";
                return View(model);
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = user.Id, code = code }, HttpContext.Request.Scheme);
            var strEmailSubject = "Truvelop - Reset Password Link";
            var strBodyMessage = _emailSender.GetTemplateByName(callbackUrl, user.Id);
            await _emailSender.SendEmailAsync(user.Email, strEmailSubject, strBodyMessage);

            ViewBag.Message = "Password reset link sent to your email";
            return View(model);
        }

        /// <summary>
        /// Show reset password page
        /// </summary>
        [HttpGet]
        public IActionResult ResetPassword(string userId = null, string code = null)
        {
            ViewData["AuthUrl"] = _angularPostbackUrl;
            ViewData["AngularHomeUrl"] = _angularHomeUrl;
            return View(new ResetPasswordViewModel { Code = code, UserId = userId });
        }

        /// <summary>
        /// Handle reset password postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ViewData["AuthUrl"] = _angularPostbackUrl;
            ViewData["AngularHomeUrl"] = _angularHomeUrl;
             if (!ModelState.IsValid)
             {
                 return View(model);
             }

            var error = string.Empty;
            if (model.Password != model.ConfirmPassword)
            {
                error = "Password and confirm password do not match.";
            }

            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.Message = error;
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            foreach (var err in result.Errors)
            {
                if (err.Description.ToUpper().Contains("INVALID TOKEN"))
                {
                    ViewBag.Message = "Opps! Reset password link expired";
                }
                else
                {
                    ViewBag.Message = err.Description;
                }
            }
            return View(model);
        }

        /// <summary>
        /// Show reset password confirm page
        /// </summary>
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            ViewData["AuthUrl"] = _angularPostbackUrl;
            ViewData["AngularHomeUrl"] = _angularHomeUrl;
            return View();
        }


        /// <summary>
        /// Show account confirm page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            ViewData["AuthUrl"] = _angularPostbackUrl;
            ViewData["AngularHomeUrl"] = _angularHomeUrl;

            if (userId == null || code == null)
            {
                ViewData["Error"] = "Opps! activation link expired";
                return View("ConfirmEmail");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewData["Error"] = "Session expired";
                return View("ConfirmEmail");
            }

            var generatedToken = GenerateToken(user.SecurityStamp);
            if (string.Equals(code, generatedToken))
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.ResetAccessFailedCountAsync(user);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("GeneratePassword", "Account", new { UserId = user.Id, code = token }, HttpContext.Request.Scheme);
                return Redirect(callbackUrl);
            }
            ViewData["Error"] = "Opps! activation link expired";
            return View("ConfirmEmail");
        }

        /// <summary>
        /// Show Generate password page
        /// </summary>
        [HttpGet]
        public IActionResult GeneratePassword(string userId = null, string code = null)
        {
            ViewData["AuthUrl"] = _angularPostbackUrl;
            ViewData["AngularHomeUrl"] = _angularHomeUrl;
            return View(new GeneratePasswordViewModel { Code = code, UserId = userId });
        }

        /// <summary>
        /// Handle Generate password postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GeneratePassword(GeneratePasswordViewModel model)
        {
            ViewData["AuthUrl"] = _angularPostbackUrl;
            ViewData["AngularHomeUrl"] = _angularHomeUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var error = string.Empty;
            if (model.Password != model.ConfirmPassword)
            {
                error = "Password and confirm password do not match.";
            }

            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.Message = error;
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.GeneratePasswordConfirmation), "Account");
            }

            foreach (var err in result.Errors)
            {
                if (err.Description.ToUpper().Contains("INVALID TOKEN"))
                {
                    ViewBag.Message = "Opps! activation link expired";
                }
                else
                {
                    ViewBag.Message = err.Description;
                }
            }
            return View(model);
        }

        /// <summary>
        /// Show Generate password confirm page
        /// </summary>
        [HttpGet]
        public IActionResult GeneratePasswordConfirmation()
        {
            ViewData["AuthUrl"] = _angularPostbackUrl;
            ViewData["AngularHomeUrl"] = _angularHomeUrl;
            return View();
        }

        public string GenerateToken(string securityStamp)
        {
            var b = System.Text.ASCIIEncoding.ASCII.GetBytes(securityStamp);
            return Convert.ToBase64String(b);
        }
    }
}