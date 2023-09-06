using AuthoritySTS.Models.AccountViewModels;
using AuthoritySTS.Services.Interface;
using System;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using AuthoritySTS.Services.Factory.Interface;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using AuthoritySTS.Models;

namespace AuthoritySTS.Services.BLL
{
    /// <summary>
    /// Class Name      : ValidateData
    /// Author          : Nehal Patel
    /// Creation Date   : 11-06-2018
    /// Purpose         : Class to validate data
    /// Revision        : 
    /// </summary> 
    public class ValidateData : IValidateData
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IOptions<OTPSettings> _otpSettings;
        private readonly AppUserManager _userManager;
        private readonly AppUserStore _appUserStore;
        private readonly IResourceManger _resourceManger;

        public ValidateData(IUserManagerFacade userFacade, IServiceFactory serviceFactory, IOptions<OTPSettings> otpSettings,
            AppUserStore appUserStore, IResourceManger resourceManger)
        {
            _userManager = userFacade.AppUserManager;
            _serviceFactory = serviceFactory;
            _otpSettings = otpSettings;
            _appUserStore = appUserStore;
            _resourceManger = resourceManger;
        }

        /// <summary>
        /// Method Name     : VerifyOtp
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to verify Otp        
        /// Revision        : 
        /// </summary> 
        /// <param name="model"></param>
        /// <param name="timeOut"></param>
        /// <param name="otp"></param>
        /// <returns></returns>
        public string VerifyOtp(VerifyOTPViewModel model, DateTime timeOut, string otp)
        {
            string error = string.Empty;

            if (DateTime.Now >= timeOut.AddMinutes(_otpSettings.Value.TimeOut))
            {
                error = AddError("OTP_EXPIRED", _resourceManger.SharedStringLocalizer["OTP_EXPIRED"]);
                return error;
            }
            if (model.OTP != otp)
            {
                error = AddError("OTP_MISMATCH", _resourceManger.SharedStringLocalizer["OTP_MISMATCH"]);
                return error;
            }
            return error;
        }

        /// <summary>
        /// Method Name     : ValidateRegisterData
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to validate Register Data        
        /// Revision        : PasswordReset
        /// </summary> 
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> ValidateRegisterData(RegisterViewModel model)
        {
            string error = string.Empty;
            if (model.Password != model.ConfirmPassword)
            {
                error = AddError("PASS_CONF_NOT_SAME", _resourceManger.SharedStringLocalizer["PASS_CONF_NOT_SAME"]);
                return error;
            }

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                error = AddError("DUPLICATE_EMAIL", _resourceManger.SharedStringLocalizer["DUPLICATE_EMAIL", model.Email]);
                return error;
            }

            var users = await Task.Run(() =>
            {
                return _userManager.Users.ToList();
            });

            if (users.FirstOrDefault(u => u.PhoneNumber == model.PhoneCode + model.PhoneNumber) != null)
            {
                error = AddError("DUPLICATE_PHONE", _resourceManger.SharedStringLocalizer["DUPLICATE_PHONE"]);
                return error;
            }
            return error;
        }

        /// <summary>
        /// Method Name     : ValidateForgetPasswordData
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to validate ForgetPassword Data
        /// Revision        : 
        /// </summary> 
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ApplicationUserErrorModel> ValidateForgetPasswordData(ForgotPasswordViewModel model)
        {
            var vm = _serviceFactory.CreateInstance<ApplicationUserErrorModel>();
            vm.AppUser = null;
            vm.IsEmail = true;

            var user = _serviceFactory.CreateInstance<ApplicationUser>();

            if (string.IsNullOrEmpty(model.Email))
            {
                vm.ErrorMessage = AddError("FORGET_PASSWORD_DATA_INVALID", _resourceManger.SharedStringLocalizer["FORGET_PASSWORD_DATA_INVALID"]);
                return vm;
            }
            else
            {
                user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    vm.ErrorMessage = AddError("EMAIL_NOT_VERIFIED", _resourceManger.SharedStringLocalizer["EMAIL_NOT_VERIFIED"]);
                    return vm;
                }
                vm.IsEmail = true;
            }
            //else if (!string.IsNullOrEmpty(model.PhoneNumber))
            //{
            //    var users = await Task.Run(() =>
            //    {
            //        return _userManager.Users.ToList();
            //    });

            //    user = users.FirstOrDefault(u => u.PhoneNumber == model.PhoneCode + model.PhoneNumber);
            //    if (user == null || !user.PhoneNumberConfirmed)
            //    {
            //        vm.ErrorMessage = AddError("PHONE_NOT_VERIFIED", _resourceManger.SharedStringLocalizer["PHONE_NOT_VERIFIED"]);
            //        return vm;
            //    }
            //}
            vm.ErrorMessage = string.Empty;
            vm.AppUser = user;
            return vm;
        }

        /// <summary>
        /// Method Name     : ValidateResetPasswordData
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to validate ResetPassword Data
        /// Revision        : 
        /// </summary> 
        /// <param name="model"></param>
        /// <returns></returns>
        public string ValidateResetPasswordData(ResetPasswordViewModel model)
        {
            var vm = string.Empty;

            if (model.Password != model.ConfirmPassword)
            {
                vm = AddError("PASS_CONF_NOT_SAME", _resourceManger.SharedStringLocalizer["PASS_CONF_NOT_SAME"]);
            }
            return vm;
        }

        /// <summary>
        /// Method Name     : AddError
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to return error
        /// Parameters      : model
        /// Revision        : 
        /// </summary> 
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private string AddError(string code, string message)
        {
            var model = new IdentityError
            {
                Code = code,
                Description = string.Format(message)
            };

            return IdentityResult.Failed(model).Errors.FirstOrDefault().Description;
        }
    }
}
