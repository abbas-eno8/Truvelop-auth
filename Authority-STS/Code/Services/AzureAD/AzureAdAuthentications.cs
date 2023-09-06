using AuthoritySTS.Data;
using AuthoritySTS.Services.Shared;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthoritySTS.Services
{
    /// <summary>
    /// this class is fetching azure ad details
    /// </summary>
    public class AzureAdAuthentications
    {
        private static readonly string _callbackPath = $"/signin-";
        private static readonly string _signedOutCallbackPath = "/signout-callback-add";
        private static readonly string _remoteSignOutPath = "/signout-add";
        private static readonly string _authority = "https://login.microsoftonline.com/";
        private readonly ILogger _logger;
        private readonly TriggerDbContext _triggerDbContext;

        public AzureAdAuthentications(TriggerDbContext triggerDbContext,ILogger logger)
        {
            _triggerDbContext = triggerDbContext;
            _logger=logger;
        }

        /// <summary>
        /// fetching all list of azure ad details
        /// </summary>
        /// <param name="services"></param>
        public void AzureAdConfiguration(IServiceCollection services)
        {
            try
            {
                _logger.LogInformation("AzureAdConfiguration Method Start.");
                
                var items = _triggerDbContext.ExternalProvider.ToList();
                var azureAdDetails = items.GroupBy(x => x.CompanyId).Select(x => x.First()).OrderByDescending(x => x.CompanyId).ToList();

                if (azureAdDetails!=null)
                {
                    foreach (var azureAdDetail in azureAdDetails)
                    {
                        string providerName = Messages.AzureAD + azureAdDetail?.CompanyId;
                        services.AddAuthentication()
                        .AddOpenIdConnect(providerName, azureAdDetail.Name, options =>
                        {
                            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                            options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                            options.Authority = _authority + azureAdDetail?.TenantId; 
                            options.ClientId = azureAdDetail?.ClientId;
                            options.ResponseType = OpenIdConnectResponseType.IdToken;
                            options.CallbackPath = _callbackPath + providerName;
                            options.SignedOutCallbackPath = _signedOutCallbackPath;
                            options.RemoteSignOutPath = _remoteSignOutPath;
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                NameClaimType = "name",
                                RoleClaimType = "role",
                            };
                            
                            options.Events.OnRedirectToIdentityProvider = context =>
                            {
                                // Get the login hint from the HTTP request
                                var loginHint = context.Properties.Parameters["login_hint"]?.ToString().Trim();
                                _logger.LogInformation("loginHint is:" + loginHint);
                                if (!string.IsNullOrEmpty(loginHint))
                                {
                                     // Add the login hint as a parameter to the authentication request
                                    context.ProtocolMessage.SetParameter("login_hint", loginHint);
                                }
                            return Task.CompletedTask;
                            };
                        });

                    }
                    _logger.LogInformation("AzureAdConfiguration Method End.");
                }
                else
                {
                    _logger.LogInformation("AzureAdDetails is null");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
            }
        }

        /// <summary>
        /// get azure ad details by company id 
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public OpenIdConnectOptions GetAzureAdDetailsByProviderName(int companyId)
        {     
            var azureAdDetail = _triggerDbContext.ExternalProvider.FirstOrDefault(a => a.CompanyId == companyId);
            
            if (azureAdDetail!=null)
            {
                _logger.LogInformation("TenantId is - " + azureAdDetail.TenantId);
                _logger.LogInformation("ClientId is - " + azureAdDetail.ClientId);
                var openIdOption =  new OpenIdConnectOptions()
                {
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    SignOutScheme = IdentityServerConstants.SignoutScheme,
                    Authority = _authority + azureAdDetail.TenantId,
                    ClientId = azureAdDetail.ClientId,
                    ResponseType = OpenIdConnectResponseType.IdToken,
                    CallbackPath = _callbackPath + Messages.AzureAD + companyId.ToString(),
                    SignedOutCallbackPath = _signedOutCallbackPath,
                    RemoteSignOutPath = _remoteSignOutPath,
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role",
                    }
                };
                openIdOption.Events.OnRedirectToIdentityProvider = context => 
                {
                    // Get the login hint from the HTTP request
                    var loginHint = context.Properties.Parameters["login_hint"]?.ToString().Trim();
                    _logger.LogInformation("loginHint is:" + loginHint);
                    if (!string.IsNullOrEmpty(loginHint)) 
                    {
                       // Add the login hint as a parameter to the authentication request
                       context.ProtocolMessage.SetParameter("login_hint", loginHint);
                    }
                
                    return Task.CompletedTask; 
                };
                return openIdOption;
            }
            else 
            {
                _logger.LogInformation("AzureAdDetail is null");
                return new OpenIdConnectOptions();
            }     
        }
    }
}
