using AuthoritySTS.Models;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthoritySTS.Services.Interface
{
    public interface IPasswordRuleManger
    {
        bool CheckForlength(int passMinimumLength, string password);

        bool CheckForLowercase(Client currentClient, string password);

        bool CheckForUppercase(Client currentClient, string password);

        bool CheckForNumbers(Client currentClient, string password);

        bool CheckForSpecialChars(Client currentClient, string password);

        bool CheckForUniqueChars(Client currentClient, string password);

        bool CheckForUsedPassword(Client currentClient, ApplicationUser user, string password);        
    }
}
