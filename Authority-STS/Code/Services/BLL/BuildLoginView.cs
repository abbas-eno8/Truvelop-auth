using AuthoritySTS.Services.Interface;
using System.Threading.Tasks;
using AuthoritySTS.Models.AccountViewModels;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using AuthoritySTS.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Logging.ILogger;

namespace AuthoritySTS.Services.BLL
{
    /// <summary>
    /// Class Name      : BuildLoginView
    /// Author          : Nehal Patel
    /// Creation Date   : 28-06-2018
    /// Purpose         : Class to build login view
    /// Revision        : 
    /// </summary>
    public class BuildLoginView : IBuildLoginView
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger _logger;



        public BuildLoginView(
           SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction, IClientStore clientStore, IHttpContextAccessor contextAccessor, ILogger logger)
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        /// <summary>        
        /// Method Name     : BuildLoginViewModelAsync
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to build login view
        /// Revision        :         
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        //public LoginViewModel BuildLoginViewModelAsync(string returnUrl, AuthorizationRequest context)
        //{
        //    return new LoginViewModel
        //    {
        //        EnableLocalLogin = true,
        //        ReturnUrl = returnUrl,
        //        UserName = context?.LoginHint,
        //    };
        //}

        /// <summary>        
        /// Method Name     : BuildLoginViewModelAsync
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to build login view
        /// Revision        :         
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            _logger.Info("BuildLoginViewModelAsync method start:");
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            _logger.Info("AuthorizationContext returns : " + context);
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl, context);
            vm.UserName = model.UserName;
            vm.RememberLogin = model.RememberLogin;
            _logger.Info("UserName is -" + vm.UserName);
            return vm;
        }

        public async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl, AuthorizationRequest context)
        {
            _logger.Info("Login Internal method BuildLoginViewModelAsync start");
            var loginProviders = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var providers = loginProviders
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                });
            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    _logger.Info("Client name:" + client);
                    allowLocal = client.EnableLocalLogin;
                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme));
                    }
                }
            }
            var model = new LoginViewModel();
            model.EnableLocalLogin = allowLocal;
            model.ReturnUrl = returnUrl;
            model.UserName = context?.LoginHint;
            model.ExternalProviders = providers.ToArray();
            _logger.Info("Login Internal method BuildLoginViewModelAsync end");
            return model;
        }

    }

    public class ExternalProvider
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }
        public string ProviderIcon { get; set; }
    }
}
