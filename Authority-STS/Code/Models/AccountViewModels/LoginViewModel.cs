using AuthoritySTS.Services.BLL;
using System.Collections.Generic;

namespace AuthoritySTS.Models.AccountViewModels
{
    public class LoginViewModel : LoginInputModel
    {
        public bool EnableLocalLogin { get; set; }
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
    }
}
