using System.ComponentModel.DataAnnotations;

namespace AuthoritySTS.Models.AccountViewModels
{
    public class VerifyPhoneNumberAdViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Subject { get; set; }
    }
}
