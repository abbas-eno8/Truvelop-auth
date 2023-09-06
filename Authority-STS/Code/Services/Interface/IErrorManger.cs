using Microsoft.AspNetCore.Identity;

namespace AuthoritySTS.Services.Interface
{
    public interface IErrorManger
    {
        IdentityResult AddError(string code, string message, string format = null);
    }
}
