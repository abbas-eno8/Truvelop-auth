using System.ComponentModel.DataAnnotations;

namespace AuthoritySTS.Models.AccountViewModels
{
    public class AddPhoneNumberAdViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Subject { get; set; }
    }
}
