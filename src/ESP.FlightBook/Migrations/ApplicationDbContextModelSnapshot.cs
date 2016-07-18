using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ESP.FlightBook.Data;

namespace ESP.FlightBook.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Aircraft", b =>
                {
                    b.Property<int>("AircraftId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AircraftCategory");

                    b.Property<string>("AircraftClass");

                    b.Property<string>("AircraftIdentifier")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 10);

                    b.Property<string>("AircraftMake");

                    b.Property<string>("AircraftModel");

                    b.Property<string>("AircraftType")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 10);

                    b.Property<int>("AircraftYear");

                    b.Property<DateTime>("ChangedOn");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("EngineType");

                    b.Property<string>("GearType");

                    b.Property<bool>("IsComplex");

                    b.Property<bool>("IsHighPerformance");

                    b.Property<bool>("IsPressurized");

                    b.Property<int>("LogbookId");

                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 36);

                    b.HasKey("AircraftId");

                    b.HasIndex("LogbookId");

                    b.ToTable("Aircraft");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Approach", b =>
                {
                    b.Property<int>("ApproachId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AirportCode")
                        .IsRequired();

                    b.Property<string>("ApproachType")
                        .IsRequired();

                    b.Property<DateTime>("ChangedOn");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("FlightId");

                    b.Property<bool>("IsCircleToLand");

                    b.Property<string>("Remarks");

                    b.Property<string>("Runway")
                        .IsRequired();

                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 36);

                    b.HasKey("ApproachId");

                    b.HasIndex("FlightId");

                    b.ToTable("Approaches");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.ApproachType", b =>
                {
                    b.Property<int>("ApproachTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Label")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.HasKey("ApproachTypeId");

                    b.ToTable("ApproachTypes");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.CategoryAndClass", b =>
                {
                    b.Property<int>("CategoryAndClassId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Abbreviation");

                    b.Property<string>("Category")
                        .IsRequired();

                    b.Property<string>("Class")
                        .IsRequired();

                    b.Property<string>("Label")
                        .IsRequired();

                    b.HasKey("CategoryAndClassId");

                    b.ToTable("CategoriesAndClasses");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Certificate", b =>
                {
                    b.Property<int>("CertificateId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CertificateDate");

                    b.Property<string>("CertificateNumber")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("CertificateType")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("ChangedOn");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("ExpirationDate");

                    b.Property<int>("LogbookId");

                    b.Property<string>("Remarks");

                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 36);

                    b.HasKey("CertificateId");

                    b.HasIndex("LogbookId");

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.CertificateType", b =>
                {
                    b.Property<int>("CertificateTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Label")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.HasKey("CertificateTypeId");

                    b.ToTable("CertificateTypes");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Currency", b =>
                {
                    b.Property<int>("CurrencyId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangedOn");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("CurrencyTypeId");

                    b.Property<int>("DaysRemaining");

                    b.Property<bool>("IsCurrent");

                    b.Property<bool>("IsNightCurrency");

                    b.Property<int>("LogbookId");

                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 36);

                    b.HasKey("CurrencyId");

                    b.HasIndex("CurrencyTypeId");

                    b.HasIndex("LogbookId");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.CurrencyType", b =>
                {
                    b.Property<int>("CurrencyTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Abbreviation");

                    b.Property<string>("AircraftCategory");

                    b.Property<string>("AircraftClass");

                    b.Property<int>("CalculationType");

                    b.Property<string>("Category")
                        .IsRequired();

                    b.Property<string>("Label")
                        .IsRequired();

                    b.Property<bool>("RequiresTailwheel");

                    b.Property<int>("SortOrder");

                    b.HasKey("CurrencyTypeId");

                    b.ToTable("CurrencyTypes");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Endorsement", b =>
                {
                    b.Property<int>("EndorsementId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CFIExpiration");

                    b.Property<string>("CFIName");

                    b.Property<string>("CFINumber");

                    b.Property<DateTime>("ChangedOn");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime>("EndorsementDate");

                    b.Property<int>("LogbookId");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 36);

                    b.HasKey("EndorsementId");

                    b.HasIndex("LogbookId");

                    b.ToTable("Endorsements");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.EndorsementType", b =>
                {
                    b.Property<int>("EndorsementTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Category")
                        .IsRequired();

                    b.Property<string>("Label")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.Property<string>("Template")
                        .IsRequired();

                    b.HasKey("EndorsementTypeId");

                    b.ToTable("EndorsementTypes");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.EngineType", b =>
                {
                    b.Property<int>("EngineTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Label")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.HasKey("EngineTypeId");

                    b.ToTable("EngineTypes");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Flight", b =>
                {
                    b.Property<int>("FlightId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AircraftId");

                    b.Property<DateTime>("ChangedOn");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("DepartureCode")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 5);

                    b.Property<string>("DestinationCode")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 5);

                    b.Property<DateTime>("FlightDate");

                    b.Property<decimal>("FlightTimeActualInstrument");

                    b.Property<decimal>("FlightTimeCrossCountry");

                    b.Property<decimal>("FlightTimeDay");

                    b.Property<decimal>("FlightTimeDual");

                    b.Property<decimal>("FlightTimeNight");

                    b.Property<decimal>("FlightTimePIC");

                    b.Property<decimal>("FlightTimeSimulatedInstrument");

                    b.Property<decimal>("FlightTimeSolo");

                    b.Property<decimal>("FlightTimeTotal");

                    b.Property<bool>("IsCheckRide");

                    b.Property<bool>("IsFlightReview");

                    b.Property<bool>("IsInstrumentProficiencyCheck");

                    b.Property<int>("LogbookId");

                    b.Property<int>("NumberOfHolds");

                    b.Property<int>("NumberOfLandingsDay");

                    b.Property<int>("NumberOfLandingsNight");

                    b.Property<string>("Remarks");

                    b.Property<string>("Route");

                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 36);

                    b.HasKey("FlightId");

                    b.HasIndex("AircraftId");

                    b.HasIndex("LogbookId");

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.GearType", b =>
                {
                    b.Property<int>("GearTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Abbreviation");

                    b.Property<string>("Label")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.HasKey("GearTypeId");

                    b.ToTable("GearTypes");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Logbook", b =>
                {
                    b.Property<int>("LogbookId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangedOn");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Remarks");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 36);

                    b.HasKey("LogbookId");

                    b.HasIndex("UserId");

                    b.ToTable("Logbooks");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Pilot", b =>
                {
                    b.Property<int>("PilotId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressLine1");

                    b.Property<string>("AddressLine2");

                    b.Property<string>("CellPhoneNumber");

                    b.Property<DateTime>("ChangedOn");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("EmailAddress");

                    b.Property<string>("FirstName");

                    b.Property<string>("HomePhoneNumber");

                    b.Property<string>("LastName");

                    b.Property<int>("LogbookId");

                    b.Property<string>("PostalCode");

                    b.Property<string>("StateOrProvince");

                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 36);

                    b.HasKey("PilotId");

                    b.HasIndex("LogbookId")
                        .IsUnique();

                    b.ToTable("Pilots");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Rating", b =>
                {
                    b.Property<int>("RatingId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CertificateId");

                    b.Property<DateTime>("ChangedOn");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime>("RatingDate");

                    b.Property<string>("RatingType")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Remarks");

                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 36);

                    b.HasKey("RatingId");

                    b.HasIndex("CertificateId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.RatingType", b =>
                {
                    b.Property<int>("RatingTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Label")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.HasKey("RatingTypeId");

                    b.ToTable("RatingTypes");
                });

            modelBuilder.Entity("ESP.FlightBook.Identity.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Aircraft", b =>
                {
                    b.HasOne("ESP.FlightBook.Api.Models.Logbook", "Logbook")
                        .WithMany("Aircraft")
                        .HasForeignKey("LogbookId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Approach", b =>
                {
                    b.HasOne("ESP.FlightBook.Api.Models.Flight", "Flight")
                        .WithMany("Approaches")
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Certificate", b =>
                {
                    b.HasOne("ESP.FlightBook.Api.Models.Logbook", "Logbook")
                        .WithMany("Certificates")
                        .HasForeignKey("LogbookId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Currency", b =>
                {
                    b.HasOne("ESP.FlightBook.Api.Models.CurrencyType", "CurrencyType")
                        .WithMany()
                        .HasForeignKey("CurrencyTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ESP.FlightBook.Api.Models.Logbook", "Logbook")
                        .WithMany("Currencies")
                        .HasForeignKey("LogbookId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Endorsement", b =>
                {
                    b.HasOne("ESP.FlightBook.Api.Models.Logbook", "Logbook")
                        .WithMany("Endorsements")
                        .HasForeignKey("LogbookId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Flight", b =>
                {
                    b.HasOne("ESP.FlightBook.Api.Models.Aircraft", "Aircraft")
                        .WithMany("Flights")
                        .HasForeignKey("AircraftId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ESP.FlightBook.Api.Models.Logbook")
                        .WithMany("Flights")
                        .HasForeignKey("LogbookId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Pilot", b =>
                {
                    b.HasOne("ESP.FlightBook.Api.Models.Logbook", "Logbook")
                        .WithOne("Pilot")
                        .HasForeignKey("ESP.FlightBook.Api.Models.Pilot", "LogbookId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ESP.FlightBook.Api.Models.Rating", b =>
                {
                    b.HasOne("ESP.FlightBook.Api.Models.Certificate", "Certificate")
                        .WithMany("Ratings")
                        .HasForeignKey("CertificateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ESP.FlightBook.Identity.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ESP.FlightBook.Identity.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ESP.FlightBook.Identity.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
