using System;
using AuthoritySTS.Models.AccountViewModels;
using System.Threading.Tasks;
using AuthoritySTS.Models;

namespace AuthoritySTS.Services.Interface
{
    public interface IValidateData
    {        
        string VerifyOtp(VerifyOTPViewModel model, DateTime timeOut, string otp);

        Task<string> ValidateRegisterData(RegisterViewModel model);

        Task<ApplicationUserErrorModel> ValidateForgetPasswordData(ForgotPasswordViewModel model);

        string ValidateResetPasswordData(ResetPasswordViewModel model);
    }
}
