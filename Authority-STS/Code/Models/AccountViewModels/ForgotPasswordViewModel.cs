using System.ComponentModel.DataAnnotations;

namespace AuthoritySTS.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        //[Required(ErrorMessage = "Please enter email address")]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "MOBILE_NOT_VALID")]
        public string PhoneNumber { get; set; }

        [Phone(ErrorMessage = "COUNTRY_CODE_INVALID")]
        public string PhoneCode { get; set; }
    }
}
