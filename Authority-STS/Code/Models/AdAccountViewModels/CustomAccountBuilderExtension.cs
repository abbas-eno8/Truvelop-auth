namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomAccountBuilderExtension
    {
        public static IIdentityServerBuilder AddActiveDirectoryAccount(this IIdentityServerBuilder builder)
        {            
            builder.Services.AddSingleton<IUserRepository, UserRepository>();            
            return builder;
        }
    }
}
