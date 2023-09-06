using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace Authority
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource ( "api1","My API" ),
                new ApiResource
                {
                    Name = "TriggerApi",
                    DisplayName="Trigger Api",
                    ApiSecrets =
                    {
                        new Secret("Triggersecretapi".Sha256())
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

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "Trigger",
                    ClientName = "Trigger",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent=false,

                    RedirectUris = {
                        "http://localhost:4200/signin-callback.html",
                        "http://localhost:4200/",
                        "http://localhost:4200/silent-renew.html",
                        "http://localhost:4200/user-manager-sample.html",
                        "http://localhost:4200/user-manager-sample-silent.html"
                    },
                    PostLogoutRedirectUris = {
                        "http://localhost:4200/"
                    },

                    AllowAccessTokensViaBrowser = true,

                    AllowedCorsOrigins = {
                        "http://localhost:4200"
                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TriggerApi"
                    }
                },
                new Client
                {
                    ClientId="TriggerMobile",
                    ClientName="Trigger Mobile",
                    AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,
                    RequireConsent=false,
                    ClientSecrets={
                        new Secret("Triggersecretmobile".Sha256())
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
                    AlwaysSendClientClaims=true
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
                        "api1"
                    },
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=false,
                    AlwaysSendClientClaims=true
                }
            };
        }
    }
}
