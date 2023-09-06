using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;

namespace AuthoritySTS.Controllers
{
    public class AuthController : Controller
    {
        string scheme = "AzureAD_95";
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IOptionsMonitorCache<OpenIdConnectOptions> _openIdConnectOptionsCache;
        private readonly OpenIdConnectPostConfigureOptions _openIdConnectPostConfigureOptions;

        public AuthController(IAuthenticationSchemeProvider schemeProvider,
           IOptionsMonitorCache<OpenIdConnectOptions> openIdConnectOptionsCache, IDataProtectionProvider dataProtection)
        {
            _schemeProvider = schemeProvider;
            _openIdConnectOptionsCache = openIdConnectOptionsCache;
            _openIdConnectPostConfigureOptions = new OpenIdConnectPostConfigureOptions(dataProtection);
        }

        [HttpPost]
        public IActionResult AddOpenIdConnect(string providerName)
        {
            if (_schemeProvider.GetSchemeAsync(providerName).Result == null)
            {
                _schemeProvider.AddScheme(new AuthenticationScheme(scheme, scheme, typeof(OpenIdConnectHandler)));
            }
            var options = new OpenIdConnectOptions
            {
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                SignOutScheme = IdentityServerConstants.SignoutScheme,
                Authority = "XXX/<TenantId>",
                ClientId = "XXX",
                ResponseType = OpenIdConnectResponseType.IdToken,
                CallbackPath = "XXX",
                SignedOutCallbackPath = "XXX",
                RemoteSignOutPath = "/XXX",
            };
            _openIdConnectOptionsCache.TryAdd(scheme, options);
            _openIdConnectPostConfigureOptions.PostConfigure(scheme, options);
            return Redirect("/");
        }
    }
}