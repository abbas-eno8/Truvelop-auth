using System.Collections.Generic;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IUserRepository
    {
        bool ValidateCredentials(string username, string password);

        CustomUser FindBySubjectId(string subjectId);

        CustomUser FindByUsername(string username);

        CustomUser FindByExternalProvider(string provider, string userId);

        CustomUser AutoProvisionUser(string provider, string userId, List<Claim> claims);

        void AddADUsertoLocal(string username, string password, string subject);        
    }
}