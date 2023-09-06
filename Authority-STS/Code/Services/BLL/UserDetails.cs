using AuthoritySTS.Services.Interface;
using AuthoritySTS.Models.AccountViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AuthoritySTS.Models;
using AuthoritySTS.Services;

namespace AuthoritySTS.Services.BLL
{
    /// <inheritdoc />
    /// <summary>
    /// Contains method to get user details
    /// </summary>
    public class UserDetails : IUserDetails
    {
        private const string Email = "email";
        private const string FullName = "fullname";

        private readonly UserManager<ApplicationUser> _userManager;

        public UserDetails(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get user details
        /// </summary>
        /// <param name="model">The RegisterViewModel</param>
        /// <param name="twoFactor">Is two factor enabled</param>
        /// <param name="userClient">User Client</param>
        /// <param name="isEmailUsernameSame">Is Email and Username Same</param>
        /// <param name="userStoreId">Client UserStore</param>
        /// <returns>ApplicationUser</returns>
        public ApplicationUser GetUserDetails(RegisterViewModel model, bool twoFactor, string userClient, bool isEmailUsernameSame, string userStoreId)
        {
            var user = new ApplicationUser
            {
                UserName = isEmailUsernameSame ? model.Email : model.UserName,
                Email = model.Email,
               // FullName = model.FullName,
                PhoneNumber = model.PhoneCode + model.PhoneNumber,
                TwoFactorEnabled = twoFactor,
               // UserClient = userClient,
                NormalizedEmail = model.Email.ToUpper(),
                NormalizedUserName = isEmailUsernameSame ? model.Email.ToUpper() : model.UserName.ToUpper(),
               // UserStoreId = userStoreId
            };
            return user;
        }

        /// <inheritdoc />
        /// <summary>
        /// Provision user with role
        /// </summary>
        public async Task<ApplicationUser> GetExternalUser(string provider, string userId, List<Claim> claims)
        {
            // var user = await _userManager.FindByLoginAsync(provider, userId);
            var user = await _userManager.FindByNameAsync(claims.FirstOrDefault(x => x.Type.ToString() == Email)?.Value);
            if (user == null)
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = userId.Replace(ControllerResources.Delimiter, "@"),
                    Email = claims.FirstOrDefault(x => x.Type.ToString() == Email)?.Value,
                    //FullName = claims.FirstOrDefault(x => x.Type.ToString() == FullName)?.Value,
                    // UserStoreId = userStoreId,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    ConcurrencyStamp = Guid.NewGuid().ToString("B"),
                    EmailConfirmed = true
                };

                var resultUser = await _userManager.CreateAsync(applicationUser);
                if (resultUser.Succeeded)
                {
                    await _userManager.AddLoginAsync(applicationUser, new UserLoginInfo(provider, userId, applicationUser.Email));
                    user = applicationUser;
                }
            }
            else
            {
                user.Email = claims.FirstOrDefault(x => x.Type.ToString() == Email)?.Value;
               // user.FullName = claims.FirstOrDefault(x => x.Type.ToString() == FullName)?.Value;
                await _userManager.UpdateAsync(user);
            }

            return user;
        }

    }
}
