using AuthoritySTS.Services.Factory.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AuthoritySTS.Services.Factory
{
    /// <summary>
    /// Class Name      : ServiceFactory
    /// Author          : Nehal Patel
    /// Creation Date   : 11-06-2018
    /// Purpose         : Class to creates object of T
    /// Revision        : 
    /// </summary>
    public class ServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Method Name     : CreateInstance
        /// Author          : Nehal Patel
        /// Creation Date   : 11-06-2018
        /// Purpose         : Method to creates object of T class
        /// Parameters      : 
        /// Revision        : 
        /// </summary> 
        public T CreateInstance<T>()
        {
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        }
    }
}
