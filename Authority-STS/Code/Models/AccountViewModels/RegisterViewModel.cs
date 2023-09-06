using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthoritySTS.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "FULLNAME_REQUIRED")]
        public string FullName { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "EMAIL_REQUIRED")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "EMAIL_NOT_VALID")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PASSWORD_REQUIRED")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "CONF_PASSWORD_REQUIRED")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "MOBILE_REQUIRED")]
        [Phone(ErrorMessage = "MOBILE_NOT_VALID")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "COUNTRY_CODE_REQUIRED")]
        [Phone(ErrorMessage = "COUNTRY_CODE_INVALID")]
        public string PhoneCode { get; set; }
    }
}
