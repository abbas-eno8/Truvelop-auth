using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthoritySTS.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class        
    public class ApplicationUser : IdentityUser
    {
        //[MaxLength(64)]
        //public override string PhoneNumber { get; set; }

        //public string UserClient { get; set; }

        //[MaxLength(30)]
        //public string FullName { get; set; }

        //[MaxLength(450)]
        //public string UserStoreId { get; set; }

    }
}
