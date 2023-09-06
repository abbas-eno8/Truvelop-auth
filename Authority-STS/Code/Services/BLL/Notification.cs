using System.Threading.Tasks;
using AuthoritySTS.Services.Interface;
using System.Text;
using AuthoritySTS.Services.Factory.Interface;

namespace AuthoritySTS.Services.BLL
{
    /// <summary>
    /// Class Name      : Notification
    /// Author          : Nehal Patel
    /// Creation Date   : 28-06-2018
    /// Purpose         : Class to send notification
    /// Revision        : 
    /// </summary>
    public class Notification : INotification
    {
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IResourceManger _resourceManger;

        public Notification(IEmailSender emailSender, ISmsSender smsSender, IResourceManger resourceManger)
        {
            _emailSender = emailSender;
            _smsSender = smsSender;
            _resourceManger = resourceManger;
        }

        /// <summary>        
        /// Method Name     : SendEmailConfirmationAsync
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to send account confirmation email
        /// Revision        :         
        /// </summary>
        /// <param name="email"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public async Task SendEmailConfirmationAsync(string email, string link)
        {
            await _emailSender.SendEmailConfirmationAsync(email, link);
        }

        /// <summary>        
        /// Method Name     : SendOtp
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to send OTP
        /// Revision        :         
        /// </summary>
        /// <param name="isemail"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="otp"></param>
        /// <param name="isPasswordreset"></param>
        /// <returns></returns>
        public async Task SendOtp(bool isemail, string email, string phoneNumber, string otp, bool isPasswordreset)
        {
            if (isemail)
            {
                var body = SetEmailTemplte(email, otp, true);
                await _emailSender.SendEmailAsync(email, _resourceManger.SharedStringLocalizer["NOTF_RESET_PASS_SUB"], body.ToString());
            }
            else
            {
                await _smsSender.SendSmsAsync(phoneNumber, isPasswordreset ? (_resourceManger.SharedStringLocalizer["NOTF_OTP_PASS", otp]) : (_resourceManger.SharedStringLocalizer["NOTF_OTP_VERIFY_PHONE", otp]));
            }
        }

        /// <summary>        
        /// Method Name     : SendCode
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to send 2FA code
        /// Revision        :         
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task SendCode(string provider, string email, string phoneNumber, string code)
        {
            if (provider == "Email")
            {
                var body = SetEmailTemplte(email, code, false);
                await _emailSender.SendEmailAsync(email, _resourceManger.SharedStringLocalizer["NOTF_2FA_SUB"], body.ToString());
            }
            else if (provider == "Phone")
            {
                await _smsSender.SendSmsAsync(phoneNumber, _resourceManger.SharedStringLocalizer["NOTF_2FA_BODY", code]);
            }
        }

        /// <summary>        
        /// Method Name     : SetEmsilTemplte
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to set email template
        /// Revision        :         
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <param name="ispasswordReset"></param>
        /// <returns></returns>
        private StringBuilder SetEmailTemplte(string email, string code, bool ispasswordReset)
        {
            var body = new StringBuilder();
            body.Append("Dear User,");
            body.Append("<br />");
            if (ispasswordReset)
            {
                body.Append("Please use this code to reset the password for the DRC e-Service account " + email + ".");
                body.Append("<br />");
            }
            else
            {
                body.Append("Please use this code for two factor authentication for the DRC e-Service account " + email + ".");
                body.Append("<br />");
            }
            body.Append("Here is your code: " + code);
            body.Append("<br />");
            body.Append("In case, you have not generated this OTP, please contact our customer care +243-4888999.");
            body.Append("<br />");
            body.Append("<br />");
            body.Append("Best Regards,");
            body.Append("<br />");
            body.Append("The DGI Team");
            body.Append("<br />");
            body.Append("Democratic Republic of the Congo");
            body.Append("<br />");
            body.Append("<br />");
            body.Append("Disclaimer:");
            body.Append("<br />");
            body.Append("The information transmitted as part of this mail is meant only for the intended person/entity only and may contain confidential, proprietary and/or privileged information/material of GSTN. GSTN does not accept or assume any liability of any nature against any person/entity in relation to the accuracy, completeness, usefulness and/or relevance or otherwise of the information as part of this mail.");
            body.Append("<br />");
            body.Append("Any use, reuse, review, retransmission, dissemination, paraphrasing, distribution or other uses of the information contained in this mail, through any medium whatsoever, by any person/entity/recipient shall strictly be at their own risks and for any claims/issues in relation thereto GSTN shall not be liable for any expense, losses, damages and/or liability thereof. ");
            body.Append("<br />");
            body.Append("If you are not the intended recipient of this mail or information contained therein, please forthwith, contact the sender and delete the material completely from your computer/s and/or the device/s wherein the contents/information of this mail may have been stored.");
            return body;
        }
    }
}
