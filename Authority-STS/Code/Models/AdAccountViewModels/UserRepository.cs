using IdentityModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection
{
    public class UserRepository : IUserRepository
    {
        public readonly List<CustomUser> _users = new List<CustomUser>
        {
            new CustomUser{
                SubjectId = "123",
                Username = "damienbod",
                Password = "damienbod"
            },
            new CustomUser{
                SubjectId = "124",
                Username = "raphael",
                Password = "raphael"
            },
        };

        public void AddADUsertoLocal(string username, string password, string subject)
        {

            CustomUser objCust = new CustomUser
            {
                Username = username,
                Password = password,
                SubjectId = subject
            };

            _users.Add(objCust);
        }

        public bool ValidateCredentials(string username, string password)
        {
            var user = FindByUsername(username);
            if (user != null)
            {
                return user.Password.Equals(password);
            }

            return false;
        }

        public CustomUser FindBySubjectId(string subjectId)
        {
            return _users.FirstOrDefault(x =>
                x.SubjectId == subjectId);
        }


        public CustomUser FindByUsername(string username)
        {
            return _users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public CustomUser FindByExternalProvider(string provider, string userId)
        {
            return _users.FirstOrDefault(x =>
                x.ProviderName == provider &&
                x.ProviderSubjectId == userId);
        }


        public CustomUser AutoProvisionUser(string provider, string userId, List<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            foreach (var claim in claims)
            {
                // if the external system sends a display name - translate that to the standard OIDC name claim
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                // if the JWT handler has an outbound mapping to an OIDC claim use that
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                // copy the claim as-is
                else
                {
                    filtered.Add(claim);
                }
            }

            // if no display name was provided, try to construct by first and/or last name
            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
            {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // create a new unique subject id
            var sub = CryptoRandom.CreateUniqueId();

            // check if a display name is available, otherwise fallback to subject id
            var name = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? sub;

            // create new user
            var user = new CustomUser
            {
                SubjectId = sub,
                Username = name,
                ProviderName = provider,
                ProviderSubjectId = userId,
                Claims = filtered
            };

            // add user to in-memory store
            _users.Add(user);

            return user;
        }
    }
}