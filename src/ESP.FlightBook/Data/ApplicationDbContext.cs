using ESP.FlightBook.Api.Models;
using ESP.FlightBook.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ESP.FlightBook.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Overrides the OnModelCreating method to apply custom model
        /// configuration
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Create index on UserId column in the Pilot table
            builder.Entity<Logbook>().HasIndex(b => b.UserId).IsUnique(false);
        }

        // Core entities
        public DbSet<Aircraft> Aircraft { get; set; }
        public DbSet<Approach> Approaches { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Endorsement> Endorsements { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Logbook> Logbooks { get; set; }
        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        // Lookup tables
        public DbSet<ApproachType> ApproachTypes { get; set; }
        public DbSet<CategoryAndClass> CategoriesAndClasses { get; set; }
        public DbSet<CertificateType> CertificateTypes { get; set; }
        public DbSet<CurrencyType> CurrencyTypes { get; set; }
        public DbSet<EndorsementType> EndorsementTypes { get; set; }
        public DbSet<EngineType> EngineTypes { get; set; }
        public DbSet<GearType> GearTypes { get; set; }
        public DbSet<RatingType> RatingTypes { get; set; }
    }
}
