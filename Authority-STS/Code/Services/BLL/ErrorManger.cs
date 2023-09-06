using AuthoritySTS.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;

namespace AuthoritySTS.Services
{
    /// <summary>
    /// Class Name      : ErrorManger
    /// Author          : Nehal Patel
    /// Creation Date   : 28-06-2018
    /// Purpose         : Class to return error
    /// Revision        : 
    /// </summary>
    public class ErrorManger : IErrorManger
    {
        /// <summary>
        /// Method Name     : AddError
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to return error
        /// Revision        : 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public IdentityResult AddError(string code, string message, string format = null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = code,
                Description = String.Format(message, format)
            });
        }
    }
}
