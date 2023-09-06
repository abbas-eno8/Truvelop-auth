using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthoritySTS.Models
{
    [Table("AspNetUsers")]
    public class AspNetUsers
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }

    [Table("AspNetUserClaims")]
    public class AspNetUserClaims
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string UserId { get; set; }
    }


    [Table("CompanyDetails")]
    public class CompanyDetails
    {
        [Key]
        public int CompId { get; set; }
        public int? ExternalProviderType { get; set; }

    }

}
