using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using AuthoritySTS.Models;

namespace AuthoritySTS.Services.BLL
{
    /// <summary>
    /// Class Name      : SmsSender
    /// Author          : Nehal Patel
    /// Creation Date   : 11-06-2018
    /// Purpose         : Class to send SMS
    /// Revision        : 
    /// </summary>  
    public class SmsSender : ISmsSender
    {
        private readonly TwilioSettings _twilioSettings;
                
        public SmsSender(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value;
        }

        /// <summary>
        /// Method Name     : SendSmsAsync
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to send SMS
        /// Revision        : 
        /// </summary> 
        /// <param name="number"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<string> SendSmsAsync(string number, string message)
        {
            try
            {
                TwilioClient.Init(_twilioSettings.Sid, _twilioSettings.Token);
                await MessageResource.CreateAsync(new PhoneNumber(number),
                     from: new PhoneNumber(_twilioSettings.From),
                     body: message);
                return "sent";
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
