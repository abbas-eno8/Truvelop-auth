using System.ComponentModel.DataAnnotations;

namespace AuthoritySTS.Models.AccountViewModels
{
    public class LoginInputModel
    {
       // [Required(ErrorMessage = "Please enter email address")]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email address")]
        public string UserName { get; set; }
        
       // [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }

        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; set; }
        public string Provider { get; set; }

        public string IsActiveDirectory { get; set; }
    }
}
