using AuthoritySTS.Models;
using AuthoritySTS.Models.AccountViewModels;
using AuthoritySTS.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AuthoritySTS.Controllers
{
    [Produces("application/json")]
    public class ManageController : Controller
    {
        private readonly IPersistedGrantService _persistedGrantService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ManageController(IPersistedGrantService persistedGrantService, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _persistedGrantService = persistedGrantService;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Route("api/Manage/{subId}")]
        //[Authorize(AuthenticationSchemes ="Bearer")]
        public IActionResult Get(string subId)
        {
            _persistedGrantService.RemoveAllGrantsAsync(subId, "Trigger");
            _persistedGrantService.RemoveAllGrantsAsync(subId, "TriggerMobile");
            return new JsonResult(new { Message = "success" });
        }
        /// <summary>
        /// Created By : Nehal Patel,Vivek Bhavsar
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/resetpassword")]
        //[Authorize(AuthenticationSchemes ="Bearer")]
        public async System.Threading.Tasks.Task<JsonData> ResetPasswordAsync([FromBody] ForgotPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return JsonSettings.UserDataWithStatusMessage(null, 203, "User does not exist");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = user.Id, code = code }, HttpContext.Request.Scheme);
                var strEmailSubject = "Truvelop - Reset Password Link";
                var strBodyMessage = _emailSender.GetTemplateByName(callbackUrl,user.Id);
                await _emailSender.SendEmailAsync(user.Email, strEmailSubject, strBodyMessage);

                return JsonSettings.UserDataWithStatusMessage(null, 200, "Password reset link sent to your email");
            }
            catch (Exception ex)
            {
                return JsonSettings.UserDataWithStatusMessage(null, 500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("api/test")]
        public IActionResult GetTest()
        {
            return new JsonResult(new { Message = "success" });
        }

    }
}