using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthoritySTS.Models
{
    /// <summary>
    /// Models of ExternalProvider
    /// </summary>
    /// [ExternalProvider]
    [Table("ExternalProvider")]
    public class AzureAdModel
    {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string TenantId { get; set; }

        public string Name { get; set; }

        public string ClientId { get; set; }

        public string WebCallBack { get; set; }
        //public string AndroidCallBack { get; set; }
        //public string IosCallBack { get; set; }

    }
}
