using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace AuthoritySTS
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("TriggerClientApi", "Trigger Client API"),

                new ApiResource
                {
                    Name = "TriggerApi",
                    DisplayName="Trigger Api",
                    ApiSecrets =
                    {
                        new Secret("Triggersecretapi".Sha256())
                    },
                    Enabled=true,
                    Description="Trigger Api",
                    Scopes =
                        {
                            new Scope()
                            {
                                Name = "TriggerApi",
                                DisplayName = "Trigger Api"
                            }
                        }
                }
            };
        }
      
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration iConfiguration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "TriggerClient",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("TriggerClientSecret".Sha256())
                    },
                    AllowedScopes = { "TriggerClientApi", "TriggerApi" }
                },
                 new Client
                {
                    ClientId = "Trigger",
                    ClientName = "Trigger",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent=false,
                    //RedirectUris ={"http://localhost:5000/" },
                    RedirectUris =iConfiguration.GetSection("appSettings").GetSection("SilentRenewUrl").Value.ToString().Split(",",System.StringSplitOptions.None),
                    //PostLogoutRedirectUris= {"http://localhost:5000/" },
                     PostLogoutRedirectUris = iConfiguration.GetSection("appSettings").GetSection("PostLogoutRedirectUrl").Value.ToString().Split(",",System.StringSplitOptions.None),
                    AllowAccessTokensViaBrowser = true,
                    //AllowedCorsOrigins ={ "http://localhost:5000/" },
                    AllowedCorsOrigins =iConfiguration.GetSection("appSettings").GetSection("AllowOriginUrl").Value.ToString().Split(",",System.StringSplitOptions.None),
                    //AccessTokenType = AccessTokenType.Jwt,
                    //AccessTokenType = AccessTokenType.Reference,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AccessTokenLifetime=172800,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Address,
                        "birthday",
                        "gender",
                        "photo",
                        "TriggerApi"
                    }
                    
                    //IdentityProviderRestrictions={ "Google"}
                },
                new Client
                {
                    ClientId="TriggerMobileApp",
                    ClientName="Trigger Mobile App",
                    AllowedGrantTypes=GrantTypes.Code,
                    RedirectUris={"com.trigger.transformation:/oauth2redirect" },
                    RequireConsent=false,
                    ClientSecrets={
                        new Secret("Triggersecretmobile".Sha256())
                    },
                    AllowedScopes={
                         IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TriggerApi"
                    },
                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AccessTokenLifetime=172800,
                    RequirePkce=false,
                    AllowPlainTextPkce=false,
                    EnableLocalLogin=false,
                    IdentityProviderRestrictions= {"Google"}
                    //RequireClientSecret = false

                },
                new Client
                {
                    ClientId="TriggerAzureMobileApp",
                    ClientName="Trigger Azure Mobile App",
                    AllowedGrantTypes=GrantTypes.Code,
                    RedirectUris ={"msauth://com.trigger.transformation/2jmj7l5rSw0yVb%2FvlWAYkK%2FYBwk%3D" },
                    RequireConsent=false,
                    ClientSecrets={
                    new Secret("Triggersecretmobile".Sha256())
                    },
                    AllowedScopes={
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "TriggerApi"
                    },
                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AccessTokenLifetime=172800,
                    RequirePkce=false,
                    AllowPlainTextPkce=false,
                    EnableLocalLogin=false,
                    Enabled=true,
                    AllowedCorsOrigins =iConfiguration.GetSection("appSettings").GetSection("AllowOriginUrl").Value.ToString().Split(",",System.StringSplitOptions.None),
                    // IdentityProviderRestrictions= { "AzureAD_26" }
                    //RequireClientSecret = false

        },
                new Client
                {
                    ClientId="TriggerAzureiOSApp",
                    ClientName="Trigger Azure iOS App",
                    AllowedGrantTypes=GrantTypes.Code,
                    RedirectUris ={"msauth.com.trigger.transformation://auth","msauth.com.trigger.transformation://auth/" },
                    RequireConsent=false,
                    ClientSecrets={
                    new Secret("Triggersecretmobile".Sha256())
                    },
                    AllowedScopes={
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "TriggerApi"
                    },
                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AccessTokenLifetime=172800,
                    RequirePkce=false,
                    AllowPlainTextPkce=false,
                    EnableLocalLogin=false,
                    Enabled=true,
                    AllowedCorsOrigins =iConfiguration.GetSection("appSettings").GetSection("AllowOriginUrl").Value.ToString().Split(",",System.StringSplitOptions.None),
                    // IdentityProviderRestrictions= { "AzureAD_26" }
                    //RequireClientSecret = false

        },
                new Client
                {
                    ClientId="TriggerMobile",
                    ClientName="Trigger Mobile",
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    RequireConsent=false,
                    ClientSecrets={
                        new Secret("Triggersecretmobile".Sha256())
                    },
                    AllowedScopes={
                         IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TriggerApi"
                    },
                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AccessTokenLifetime=172800,                    
                    //RefreshTokenExpiration = TokenExpiration.Sliding,
                    //SlidingRefreshTokenLifetime = 172800,
                    //RefreshTokenUsage=TokenUsage.ReUse
                },
                new Client
                {
                    ClientId="hybrid",
                    ClientName="hybrid",
                    AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,
                    RequireConsent=false,
                    ClientSecrets={
                        new Secret("secret".Sha256())
                    },
                    RedirectUris           = { "http://localhost:5001/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },

                    AllowedScopes={
                         IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TriggerApi"
                    },
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=false,
                    AlwaysSendClientClaims=true,
                     EnableLocalLogin=false,
                   // IdentityProviderRestrictions= {"Google"}
                }
            };
        }
    }
}
