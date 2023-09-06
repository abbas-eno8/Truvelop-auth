using IdentityServer4.Validation;
using System.Threading.Tasks;
using IdentityModel;

namespace Microsoft.Extensions.DependencyInjection
{
    public class CustomUserResourceOwnerPasswordValidator: IResourceOwnerPasswordValidator
    {        
        private readonly IUserRepository _userRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUserResourceOwnerPasswordValidator"/> class.
        /// </summary>
        /// <param name="users">The users.</param>
        public CustomUserResourceOwnerPasswordValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (_userRepository.ValidateCredentials(context.UserName, context.Password))
            {
                var user = _userRepository.FindByUsername(context.UserName);
                context.Result = new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password);
            }

            return Task.FromResult(0);
        }
    }
}