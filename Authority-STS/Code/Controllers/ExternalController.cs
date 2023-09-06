using AuthoritySTS.Data;
using AuthoritySTS.Models;
using AuthoritySTS.Models.AccountViewModels;
using AuthoritySTS.Services;
using AuthoritySTS.Services.Shared;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthoritySTS.Controllers
{
    public class ExternalController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IOptionsMonitorCache<OpenIdConnectOptions> _openIdConnectOptionsCache;
        private readonly OpenIdConnectPostConfigureOptions _openIdConnectPostConfigureOptions;
        private readonly AzureAdAuthentications _azureAdAuthentications;
        private readonly TriggerDbContext _triggerDbContext;
        private readonly ILogger<ExternalController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ExternalController(UserManager<ApplicationUser> userManager,
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IOptionsMonitorCache<OpenIdConnectOptions> openIdConnectOptionsCache,
            IDataProtectionProvider dataProtection,
            ILogger<ExternalController> logger, TriggerDbContext triggerDbContext,
             SignInManager<ApplicationUser> signInManager)
        {
            _interaction = interaction;
            _userManager = userManager;
            _logger = logger;
            _schemeProvider = schemeProvider;
            _openIdConnectOptionsCache = openIdConnectOptionsCache;
            _openIdConnectPostConfigureOptions = new OpenIdConnectPostConfigureOptions(dataProtection);
            _triggerDbContext = triggerDbContext;
            _azureAdAuthentications = new AzureAdAuthentications(_triggerDbContext, _logger);
            _signInManager = signInManager;
        }

        /// <summary>
        /// Initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl = null, string loginHint = null)
        {
            try
            {
                if (provider.Contains("AzureAD"))
                {
                    var redirectUrl = Url.Action("ExternalLoginCallback", "External", new { ReturnUrl = returnUrl });
                    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                    _logger.LogInformation("External login AzureAD");
                    _logger.LogInformation("External login redirectUrl : " + redirectUrl);
                    properties.Items.Add(ControllerResources.Scheme, provider);
                    properties.Items.Add(ControllerResources.ReturnUrl, returnUrl);
                    properties.SetParameter("login_hint", loginHint);
                    return Challenge(properties, provider);
                }
                else
                {
                    var props = new AuthenticationProperties
                    {
                        RedirectUri = Url.Action(ControllerResources.ExternalLoginCallback, ControllerResources.External, new
                        {
                            returnUrl
                        }),
                        Items = {
                        {
                            ControllerResources.ReturnUrl, returnUrl
                        }
                    }
                    };

                    props.Items.Add(ControllerResources.Scheme, provider);
                    _logger.LogInformation("External login Google");
                    return Challenge(props, provider);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                ViewData["ReturnUrl"] = returnUrl;
                ViewData[ControllerResources.Error] = Messages.ContactAdministrator;
                return View(ControllerResources.Errors);
            }
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            _logger.LogInformation("ExternalLoginCallback start step-4");
            string returnUrl = string.Empty;
            try
            {
                // read external identity from the temporary cookie
                var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

                if (result?.Succeeded != true)
                {
                    ViewData["ReturnUrl"] = "";
                    ViewData[ControllerResources.Error] = Messages.ContactAdministrator;
                    return View(ControllerResources.Errors);
                }
                _logger.LogInformation("ExternalLoginCallback result provider name :" + result.Properties.Items[ControllerResources.Scheme]);
                _logger.LogInformation("ExternalLoginCallback result return url :" + result.Properties.Items[ControllerResources.ReturnUrl]);
                _logger.LogInformation("ExternalLogin login Succeeded ");

                string providerName = result.Properties.Items[ControllerResources.Scheme];
                _logger.LogInformation("Provider Name is :" + providerName);
                returnUrl = result.Properties.Items[ControllerResources.ReturnUrl];
                // returnUrl = "/connect/authorize/callback?client_id=TriggerAzureMobileApp&redirect_uri=msauth%3A%2F%2Fcom.trigger.transformation%2F2jmj7l5rSw0yVb%252FvlWAYkK%252FYBwk%253D&response_type=code&scope=TriggerApi%20offline_access&state=qnvpvtuvgogqfwxr&authorization_user_agent=DEFAULT&authorities=%5B%7B%22type%22%3A%20%22AAD%22,%22audience%22%3A%20%7B%22type%22%3A%20%22AzureADMyOrg%22,%22tenant_id%22%3A%20%22eab845cd-162c-460c-a878-edb4c387d231%22%7D%7D%5D";
                var context = await _interaction.GetAuthorizationContextAsync(returnUrl);


                _logger.LogInformation("ExternalLogin return url:" + returnUrl);
                // retrieve claims of the external user            
                var claims = result.Principal.Claims.ToList();

                // try to determine the unique id of the external user (issued by the provider) the most common claim type for that are the sub claim and the NameIdentifier            
                var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject) ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                _logger.LogInformation("User Id Claims :" + userIdClaim);
                if (userIdClaim == null)
                {
                    ViewData["ReturnUrl"] = returnUrl;
                    ViewData[ControllerResources.Error] = Messages.ContactAdministrator;
                    return View(ControllerResources.Errors);
                }

                string userEmail = GetUserEmailId(result);
                _logger.LogInformation("Logged user Email Id : " + userEmail);
                var user = await _userManager.FindByEmailAsync(userEmail);

                if (user == null)
                {
                    _logger.LogInformation("ExternalLogin : Logged user not found ");
                    ViewData["ReturnUrl"] = returnUrl;
                    ViewData[ControllerResources.Error] = Messages.LoggedUserNotFound.Replace("{0}", "\"" + userEmail + "\"");
                    return View(ControllerResources.Errors);
                }

                var aspNetUsers = _triggerDbContext.AspNetUsers.FirstOrDefault(x => x.Email == user.Email);

                _logger.LogInformation("ExternalLogin : get aspNetUsers UserId " + aspNetUsers.Id);

                var userClaims = _triggerDbContext.AspNetUserClaims.Where(x => x.UserId == aspNetUsers.Id).ToList();

                var companyDetails = _triggerDbContext.CompanyDetails.FirstOrDefault(x => x.CompId == Convert.ToInt32(userClaims.FirstOrDefault(y => y.ClaimType == "CompId").ClaimValue));
                _logger.LogInformation("Get companyDetails compId " + companyDetails.CompId);
                _logger.LogInformation("Get companyDetails External Provider Type :" + companyDetails.ExternalProviderType);

                if (companyDetails?.ExternalProviderType == 0)
                {
                    ViewData["ReturnUrl"] = returnUrl;
                    ViewData[ControllerResources.Error] = Messages.ExternalLoginNotAllowed;
                    return View(ControllerResources.Errors);
                }

                //get user and provider to signin
                await ExternalUserSignIn(userClaims, result, userIdClaim, user);

                _logger.LogInformation("ExternalLogin : sign in Done ");

                // validate return URL and redirect back to authorization endpoint or a local page
                if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
                {
                    _logger.LogInformation("ExternalLoginCallback Validate return URL" + returnUrl);
                    return Redirect(returnUrl);
                }
                _logger.LogInformation("ExternalLoginCallback end step-4");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                ViewData["ReturnUrl"] = returnUrl;
                ViewData[ControllerResources.Error] = Messages.SomethingWentWrong;
                return View(ControllerResources.Errors);
            }
            return Redirect("~/");
        }

        /// <summary>
        /// Get user and provider to signin
        /// </summary>
        private async Task ExternalUserSignIn(List<AspNetUserClaims> userClaims, AuthenticateResult result, Claim userIdClaim, ApplicationUser user)
        {
            try
            {
                _logger.LogInformation("ExternalUserSignIn start ");
                var provider = result.Properties.Items[ControllerResources.Scheme];
                var userId = userIdClaim.Value;

                // if the external provider issued an id_token, we'll keep it for sign out
                var idToken = result.Properties.GetTokenValue(ControllerResources.IdToken);
                if (idToken != null)
                {
                    var props = new AuthenticationProperties();
                    props.StoreTokens(new[] { new AuthenticationToken { Name = ControllerResources.IdToken, Value = idToken } });
                }
                _logger.LogInformation("ExternalUserSignIn : set claims in token ");

                var newClaims = new List<Claim>();
                foreach (AspNetUserClaims clm in userClaims)
                {
                    newClaims.Add(new Claim(clm.ClaimType, clm.ClaimValue));
                }
                newClaims.Add(new Claim("username", user.Email));
                newClaims.Add(new Claim("email", user.Email));
                var identity = new ClaimsIdentity(newClaims);
                var principal = new ClaimsPrincipal(identity);

                //ISystemClock clock = HttpContext.GetClock();
                IdentityServerUser user1 = new IdentityServerUser(userId)
                {
                    AdditionalClaims = newClaims,
                    AuthenticationTime = DateTime.UtcNow //clock.UtcNow.UtcDateTime
                };
                await HttpContext.SignInAsync(user1);

                //await HttpContext.SignInAsync(userId, user.Email, provider, newClaims.ToArray());
                _logger.LogInformation("ExternalUserSignIn : Sign In ");
                // delete temporary cookie used during external authentication
                await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }


        /// <summary>
        /// Find User's azure Ad details by passed email Id
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AzureAdLogin(LoginInputModel model)
        {
            try
            {
                _logger.LogInformation("AzureAdLogin method start");
                _logger.LogInformation("AzureAdLogin Method - Model details" + model);

                var aspNetUsers = _triggerDbContext.AspNetUsers.FirstOrDefault(x => x.Email == model.UserName);
                if (aspNetUsers != null)
                {
                    var aspNetUserClaims = _triggerDbContext.AspNetUserClaims.FirstOrDefault(x => x.UserId == aspNetUsers.Id && x.ClaimType == "CompId");
                    string providerName = Messages.AzureAD + aspNetUserClaims.ClaimValue;

                    _logger.LogInformation("AzureAdLogin Method " + providerName);


                    //if (_schemeProvider.GetSchemeAsync(providerName).Result == null)
                    //{

                    if (GetCheck(aspNetUserClaims.ClaimValue, providerName, model.ReturnUrl))
                    {
                        if (_schemeProvider.GetSchemeAsync(providerName).Result != null)
                        {
                            _schemeProvider.RemoveScheme(providerName);
                            //  _openIdConnectOptionsCache.TryRemove(providerName);
                            // _openIdConnectPostConfigureOptions.(providerName, options);
                        }
                        _schemeProvider.AddScheme(new AuthenticationScheme(providerName, providerName, typeof(OpenIdConnectHandler)));
                        return ExternalLogin(providerName, model.ReturnUrl, model.UserName);
                    }
                    else
                    {
                        _logger.LogInformation("Return URL is - " + model.ReturnUrl);
                        ViewData["ReturnUrl"] = model.ReturnUrl;
                        ViewData[ControllerResources.Error] = Messages.ClaimValueNotFound;
                        return View(ControllerResources.Errors);
                    }
                }
                else
                {
                    _logger.LogInformation("Return URL is - " + model.ReturnUrl);
                    ViewData["ReturnUrl"] = model.ReturnUrl;
                    ViewData[ControllerResources.Error] = Messages.EnteredUserNotFound;
                    return View(ControllerResources.Errors);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                _logger.LogInformation("Return URL is - " + model.ReturnUrl);
                ViewData["ReturnUrl"] = model.ReturnUrl;
                ViewData[ControllerResources.Error] = Messages.SomethingWentWrong;
                return View(ControllerResources.Errors);
            }
        }

        private bool GetCheck(string claimValue, string providerName, string returnUrl)
        {
            if (!string.IsNullOrEmpty(claimValue))
            {
                _logger.LogInformation("Azure Ad Login Method - Claim Value is - " + claimValue);
                var options = _azureAdAuthentications.GetAzureAdDetailsByProviderName(Convert.ToInt32(claimValue));
                if (options != null && !string.IsNullOrEmpty(options.ClientId))
                {
                    _openIdConnectOptionsCache.TryAdd(providerName, options);
                    _openIdConnectPostConfigureOptions.PostConfigure(providerName, options);
                    _logger.LogInformation("Azure Ad Login Method - options.ClientId is - " + options.ClientId);
                    _logger.LogInformation("AzureAdLogin method end");
                    return true;
                }
                else
                {
                    _logger.LogInformation("Azure Ad Login Method - options.ClientId is - " + options.ClientId);
                    _logger.LogInformation("AzureAdLogin method end");
                    return false;
                }
            }
            else
            {
                _logger.LogInformation("Azure Ad Login Method - Claim Value is - " + claimValue);
                _logger.LogInformation("AzureAdLogin method end");
                return false;
            }
        }

        private string GetUserEmailId(AuthenticateResult result)
        {

            string providerName = result.Properties.Items[ControllerResources.Scheme];
            var claims = result.Principal.Claims.ToList();

            if (providerName == Messages.Google)
            {
                return claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            }
            else
            {
                return claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
            }
        }

        [HttpGet]
        public async Task<CustomJsonData> GetProvider(string userName)
        {
            var aspNetUsers = _triggerDbContext.AspNetUsers.FirstOrDefault(x => x.Email == userName);
            ProviderDetail providerDetail = new ProviderDetail();
            if (aspNetUsers != null)
            {
                var aspNetUserClaims = _triggerDbContext.AspNetUserClaims.FirstOrDefault(x => x.UserId == aspNetUsers.Id && x.ClaimType == "CompId");
                var azureAdDetail = _triggerDbContext.ExternalProvider.FirstOrDefault(a => a.CompanyId == Convert.ToInt32(aspNetUserClaims.ClaimValue));
                if (azureAdDetail != null)
                {
                    providerDetail.ProviderName = Messages.AzureAD + aspNetUserClaims.ClaimValue;
                    if (GetCheck(aspNetUserClaims.ClaimValue, providerDetail.ProviderName, null))
                    {
                        if (_schemeProvider.GetSchemeAsync(providerDetail.ProviderName).Result != null)
                        {
                            _schemeProvider.RemoveScheme(providerDetail.ProviderName);
                        }
                        _schemeProvider.AddScheme(new AuthenticationScheme(providerDetail.ProviderName, providerDetail.ProviderName, typeof(OpenIdConnectHandler)));
                    }
                    return JsonSettings.UserCustomDataWithStatusMessage(providerDetail, 200, "Got Provider name based on UserName");
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(providerDetail, 204, "User not found");
                }
            }
            return JsonSettings.UserCustomDataWithStatusMessage(providerDetail, 204, "User not found");
        }
    }
}