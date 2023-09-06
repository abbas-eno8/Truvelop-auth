using AuthoritySTS.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthoritySTS.Data
{
    public class TriggerDbContext : DbContext
    {

        public virtual DbSet<AzureAdModel> ExternalProvider { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<CompanyDetails> CompanyDetails { get; set; }

        public TriggerDbContext(DbContextOptions<TriggerDbContext> options)
           : base(options)
        {

        }

    }
}
