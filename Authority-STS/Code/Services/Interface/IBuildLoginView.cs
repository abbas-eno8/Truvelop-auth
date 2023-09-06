using AuthoritySTS.Models.AccountViewModels;
using IdentityServer4.Models;
using System.Threading.Tasks;

namespace AuthoritySTS.Services.Interface
{
    public interface IBuildLoginView
    {
        Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl, AuthorizationRequest context);

        Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model);
    }
}
