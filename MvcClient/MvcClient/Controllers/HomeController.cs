using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityModel.Client;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Globalization;
using System;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Login()
        {

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("MyCookies");
            await HttpContext.SignOutAsync("oidc");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> CallApiUsingUserAccessToken()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var client = new HttpClient();
                client.SetBearerToken(accessToken);
                var content = await client.GetStringAsync("http://localhost:5002/api/values/");

                List<TokenData> tokenData = JsonConvert.DeserializeObject<List<TokenData>>(content);
                return View("JsonMsg", tokenData);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Error");
            }
        }

        public async Task<IActionResult> CallApiUsingClientCredentials()
        {
            try
            {
                //var tokenClient = new TokenClient("http://localhost:5000/connect/token", "TriggerMobile", "Triggersecretmobile");
                //var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("sanketbhandari@mailinator.com", "Admin@123", "TriggerApi");


                var tokenClient = new TokenClient(new HttpClient() { BaseAddress = new Uri("http://localhost:5000/connect/token") }, new TokenClientOptions { ClientId = "TriggerMobile", ClientSecret = "Triggersecretmobile" });
                var tokenResponse = await tokenClient.RequestPasswordTokenAsync("sanketbhandari@mailinator.com", "Admin@123", "TriggerApi");

                var client = new HttpClient();
                client.SetBearerToken(tokenResponse.AccessToken);
                var content = await client.GetStringAsync("http://localhost:5002/api/CallApiUsingClientCredentials");

                List<TokenData> tokenData = JsonConvert.DeserializeObject<List<TokenData>>(content);
                return View("JsonMsg", tokenData);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Error");
            }
        }

        public async Task<IActionResult> CallApiUsingUserInfo()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");


                //var userInfoClient = new UserInfoClient(new Uri("http://172.16.1.97:8081/connect/userinfo"));

                var client = new HttpClient();

                var response = await client.GetUserInfoAsync(new UserInfoRequest { Token = accessToken, Address = "http://172.16.1.97:8081/connect/userinfo" });

                //var response = await userInfoClient.GetAsync(accessToken);

                var claims = response.Claims;

                var tokenData = new List<TokenData>();
                foreach (var cl in claims)
                {
                    var userCl = new TokenData
                    {
                        Type = cl.Type,
                        Value = cl.Value
                    };
                    tokenData.Add(userCl);
                }
                return View("JsonMsg", tokenData);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Error");
            }
        }

        public async Task<IActionResult> RenewTokens()
        {
            try
            {
                //var discoveryClient = new DiscoveryClient("http://172.16.1.97:8081");
                //discoveryClient.Policy = new DiscoveryPolicy { RequireHttps = false };
                //var disco = await discoveryClient.GetAsync();
                //if (disco.IsError) throw new Exception(disco.Error);

                // var tokenClient = new TokenClient(disco.TokenEndpoint, "hybrid", "secret");
                var tokenClient = new TokenClient(new HttpClient() { BaseAddress = new Uri("http://172.16.1.97:8081") }, new TokenClientOptions { ClientId = "hybrid", ClientSecret = "secret" });
                var rt = await HttpContext.GetTokenAsync("refresh_token");
                var tokenResult = await tokenClient.RequestRefreshTokenAsync(rt);

                if (!tokenResult.IsError)
                {
                    var old_id_token = await HttpContext.GetTokenAsync("id_token");
                    var new_access_token = tokenResult.AccessToken;
                    var new_refresh_token = tokenResult.RefreshToken;

                    var tokens = new List<AuthenticationToken>
                {
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = old_id_token },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = new_access_token },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = new_refresh_token }
                };

                    var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                    tokens.Add(new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) });

                    var info = await HttpContext.AuthenticateAsync("MyCookies");
                    info.Properties.StoreTokens(tokens);
                    await HttpContext.SignInAsync("MyCookies", info.Principal, info.Properties);

                    return Redirect("~/Home/Index");
                }

                ViewData["Error"] = tokenResult.Error;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Error");
            }
        }

        public async Task<IActionResult> CallApiUsingDesignerRole()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var client = new HttpClient();
                client.SetBearerToken(accessToken);
                var content = await client.GetStringAsync("http://localhost:5002/api/CallApiUsingDesignerRole");

                List<TokenData> tokenData = JsonConvert.DeserializeObject<List<TokenData>>(content);
                return View("JsonMsg", tokenData);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Error");
            }
        }

        public async Task<IActionResult> CallApiUsingClaim()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var client = new HttpClient();
                client.SetBearerToken(accessToken);
                var content = await client.GetStringAsync("http://localhost:5002/api/CallApiUsingClaim");

                List<TokenData> tokenData = JsonConvert.DeserializeObject<List<TokenData>>(content);
                return View("JsonMsg", tokenData);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Error");
            }
        }

        public async Task<IActionResult> CallApiUsingReadWrite()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var client = new HttpClient();
                client.SetBearerToken(accessToken);
                var content = await client.GetStringAsync("http://localhost:5002/api/CallApiUsingReadWrite");

                List<TokenData> tokenData = JsonConvert.DeserializeObject<List<TokenData>>(content);
                return View("JsonMsg", tokenData);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Error");
            }
        }

    }

    public class TokenData
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
