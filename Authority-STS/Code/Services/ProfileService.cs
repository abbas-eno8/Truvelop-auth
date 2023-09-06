using IdentityServer4.Models;
using IdentityServer4.Services;
using AuthoritySTS.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System;

namespace AuthoritySTS.Services
{
    /// <summary>
    /// Class Name      : ProfileService
    /// Author          : Nehal Patel
    /// Creation Date   : 28-06-2018
    /// Purpose         : Class to get user profile
    /// Revision        : 
    /// </summary>
    public class ProfileService : IProfileService
    {
        /// <summary>
        /// Method Name     : GetProfileDataAsync
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to get user profile data
        /// Revision        : 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// 
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //Add FullName as claim
            if (context.Client.ClientId == "TriggerMobile")
            {
                var user = _userManager.GetUserAsync(context.Subject).Result;
                var claims = _userManager.GetClaimsAsync(user);

                context.IssuedClaims.AddRange(claims.Result);
                context.IssuedClaims.Add(new Claim("email", user.Email));
            }

            context.IssuedClaims.AddRange(context.Subject.Claims);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Method Name     : IsActiveAsync
        /// Author          : Nehal Patel
        /// Creation Date   : 28-06-2018
        /// Purpose         : Method to check user is active or not
        /// Revision        : 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(0);
        }
    }
}
