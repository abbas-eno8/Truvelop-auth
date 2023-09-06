using System.ComponentModel.DataAnnotations;

namespace AuthoritySTS.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {        
        public string Email { get; set; }

       // [Required(ErrorMessage = "Please enter password")]
       // [StringLength(15, MinimumLength = 10, ErrorMessage = "Password must be at least 10 characters")]
       // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{10,15}$", ErrorMessage = "Password must be at least 10 characters and should contain one uppercase letter, one lowercase letter, one number and one special character")]        
        [DataType(DataType.Password)]
        public string Password { get; set; }

       // [Required(ErrorMessage = "Please enter confirm password")]
       // [StringLength(15, MinimumLength = 10, ErrorMessage = "Password must be at least 10 characters")]
       // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{10,15}$", ErrorMessage = "Confirm Password must be at least 10 characters and should contain one uppercase letter, one lowercase letter, one number and one special character")]        
        [DataType(DataType.Password)]        
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public string UserId { get; set; }

        public string OTP { get; set; }
    }
}
