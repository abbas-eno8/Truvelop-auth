using AuthoritySTS.Models;
using AuthoritySTS.Models.AccountViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthoritySTS.Services.Interface
{
    /// <summary>
    /// Map user details
    /// </summary>
    public interface IUserDetails
    {
        /// <summary>
        /// Map user details
        /// </summary>
        /// <param name="model">The RegisterViewModel</param>
        /// <param name="twoFactor">Is 2FA</param>
        /// <param name="userClient">User default application</param>
        /// <param name="isEmailUsernameSame">Is email and username same</param>
        /// <param name="userStoreId">User store id</param>
        /// <returns>The ApplicationUser</returns>
        ApplicationUser GetUserDetails(RegisterViewModel model, bool twoFactor, string userClient, bool isEmailUsernameSame, string userStoreId);

        /// <summary>
        /// Map external user
        /// </summary>
        /// <param name="provider">The identity provider</param>
        /// <param name="userId">User Id</param>
        /// <param name="claims">List of user claims</param>
        /// <param name="userClient">User default application</param>
        /// <param name="userStoreId">User store id</param>
        /// <returns>The ApplicationUser</returns>
        Task<ApplicationUser> GetExternalUser(string provider, string userId, List<Claim> claims);
    }
}
