using System.Net.Mail;
using AuthoritySTS.Services.Factory.Interface;

namespace AuthoritySTS.Services.Factory
{
    /// <summary>
    /// Class Name      : MailMessageFactory
    /// Author          : Nehal Patel
    /// Creation Date   : 11-06-2018
    /// Purpose         : Class to creates MailMessage object
    /// Revision        : 
    /// </summary>
    public class MailMessageFactory : IMailMessageFactory
    {
        /// <summary>
        /// Method Name     : CreateMailMessage
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to return object of MailMessage
        /// Parameters      : from, to
        /// Revision        : 
        /// </summary>  
        public MailMessage CreateMailMessage(string from, string to)
        {
            return new MailMessage(from, to);
        }
    }
}
