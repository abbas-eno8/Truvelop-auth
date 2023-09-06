using AuthoritySTS.Models;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace AuthoritySTS.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = _userManager.FindByEmailAsync(context.UserName).Result;
            if (user == null || !(user.EmailConfirmed))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist", null);
                return Task.FromResult(context.Result);
            }
            var passwordValid = _userManager.CheckPasswordAsync(user, context.Password).Result;
            if (!passwordValid)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid password", null);
                return Task.FromResult(context.Result);
            }
            
            context.Result = new GrantValidationResult(user.Id, "password");
            return Task.FromResult(context.Result);
        }
    }
}
