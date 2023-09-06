using AuthoritySTS.Models;
using AuthoritySTS.Services.Factory.Interface;
using AuthoritySTS.Services.Interface;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthoritySTS.Services.BLL
{
    /// <summary>
    /// Class Name      : PasswordRuleManger
    /// Author          : Nehal Patel
    /// Creation Date   : 15-06-2018
    /// Purpose         : Class to validate password against password policy
    /// Revision        : 
    /// </summary>
    public class PasswordRuleManger : IPasswordRuleManger
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly AppUserStore _appUserStore;

        public PasswordRuleManger(IServiceFactory serviceFactory, AppUserStore appUserStore)
        {
            _serviceFactory = serviceFactory;
            _appUserStore = appUserStore;
        }

        /// <summary>        
        /// Method Name     : CheckForlength
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to check length of password
        /// Revision        :         
        /// </summary>
        /// <param name="passMinimumLength"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckForlength(int passMinimumLength, string password)
        {
            if (String.IsNullOrEmpty(password) || password.Length < passMinimumLength)
            {
                return true;
            }
            return false;
        }

        /// <summary>        
        /// Method Name     : CheckForLowercase
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to check password contains lowercase character
        /// Revision        :         
        /// </summary>
        /// <param name="currentClient"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckForLowercase(Client currentClient, string password)
        {
            if (currentClient.Properties.ContainsKey("passIsLowercase") && Convert.ToBoolean(currentClient.Properties["passIsLowercase"]))
            {
                if (!password.Any(char.IsLower))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>        
        /// Method Name     : CheckForUppercase
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to check password contains uppercase character
        /// Revision        :         
        /// </summary>
        /// <param name="currentClient"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckForUppercase(Client currentClient, string password)
        {
            if (currentClient.Properties.ContainsKey("passIsUppercase") && Convert.ToBoolean(currentClient.Properties["passIsUppercase"]))
            {
                if (!password.Any(char.IsUpper))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>        
        /// Method Name     : CheckForNumbers
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to check password contains numeric character
        /// Revision        :         
        /// </summary>
        /// <param name="currentClient"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckForNumbers(Client currentClient, string password)
        {
            if (currentClient.Properties.ContainsKey("passIsDigit") && Convert.ToBoolean(currentClient.Properties["passIsDigit"]))
            {
                if (!password.Any(char.IsNumber))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>        
        /// Method Name     : CheckForSpecialChars
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to check password contains special character
        /// Revision        :         
        /// </summary>
        /// <param name="currentClient"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckForSpecialChars(Client currentClient, string password)
        {
            if (currentClient.Properties.ContainsKey("passIsSymbol") && Convert.ToBoolean(currentClient.Properties["passIsSymbol"]))
            {
                var regexItem = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9 ]*$");
                if (regexItem.IsMatch(password))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>        
        /// Method Name     : CheckForUniqueChars
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to check password contains number of unique character
        /// Revision        :         
        /// </summary>
        /// <param name="currentClient"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckForUniqueChars(Client currentClient, string password)
        {
            var passUniqueChars = currentClient.Properties.ContainsKey("passUniqueChars") ? Convert.ToInt32(currentClient.Properties["passUniqueChars"]) : 0;
            if (password.Distinct().Count() == passUniqueChars)
            {
                return true;
            }
            return false;
        }

        /// <summary>        
        /// Method Name     : CheckForUsedPassword
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to check password is previously used 
        /// Revision        :         
        /// </summary>
        /// <param name="currentClient"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckForUsedPassword(Client currentClient, ApplicationUser user, string password)
        {
            var passPreviousUsed = currentClient.Properties.ContainsKey("passPreviousUsed") ? Convert.ToInt32(currentClient.Properties["passPreviousUsed"]) : 0;
            if (passPreviousUsed != 0)
            {
                var previousPasswords = _appUserStore.Context.Set<PreviousPassword>().ToList();
                var passwordHasher = _serviceFactory.CreateInstance<PasswordHasher<ApplicationUser>>();

                //var previousUsedPasswords = user.PreviousUserPasswords.OrderByDescending(up => up.CreatedDate).
                //   Select(up => up.PasswordHash).Take(passPreviousUsed).ToList();

                //foreach (var previousUsedPassword in previousUsedPasswords)
                //{
                //    if (passwordHasher.VerifyHashedPassword(user, previousUsedPassword, password) == PasswordVerificationResult.Success)
                //    {
                //        return true;
                //    }
                //}
            }
            return false;
        }
    }
}
