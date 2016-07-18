using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ESP.FlightBook.Migrations
{
    public partial class InitialModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApproachTypes",
                columns: table => new
                {
                    ApproachTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Label = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproachTypes", x => x.ApproachTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesAndClasses",
                columns: table => new
                {
                    CategoryAndClassId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Abbreviation = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: false),
                    Class = table.Column<string>(nullable: false),
                    Label = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesAndClasses", x => x.CategoryAndClassId);
                });

            migrationBuilder.CreateTable(
                name: "CertificateTypes",
                columns: table => new
                {
                    CertificateTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Label = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTypes", x => x.CertificateTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyTypes",
                columns: table => new
                {
                    CurrencyTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Abbreviation = table.Column<string>(nullable: true),
                    AircraftCategory = table.Column<string>(nullable: true),
                    AircraftClass = table.Column<string>(nullable: true),
                    CalculationType = table.Column<int>(nullable: false),
                    Category = table.Column<string>(nullable: false),
                    Label = table.Column<string>(nullable: false),
                    RequiresTailwheel = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyTypes", x => x.CurrencyTypeId);
                });

            migrationBuilder.CreateTable(
                name: "EndorsementTypes",
                columns: table => new
                {
                    EndorsementTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<string>(nullable: false),
                    Label = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Template = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndorsementTypes", x => x.EndorsementTypeId);
                });

            migrationBuilder.CreateTable(
                name: "EngineTypes",
                columns: table => new
                {
                    EngineTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Label = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineTypes", x => x.EngineTypeId);
                });

            migrationBuilder.CreateTable(
                name: "GearTypes",
                columns: table => new
                {
                    GearTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Abbreviation = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearTypes", x => x.GearTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Logbooks",
                columns: table => new
                {
                    LogbookId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChangedOn = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logbooks", x => x.LogbookId);
                });

            migrationBuilder.CreateTable(
                name: "RatingTypes",
                columns: table => new
                {
                    RatingTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Label = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingTypes", x => x.RatingTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Aircraft",
                columns: table => new
                {
                    AircraftId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AircraftCategory = table.Column<string>(nullable: true),
                    AircraftClass = table.Column<string>(nullable: true),
                    AircraftIdentifier = table.Column<string>(maxLength: 10, nullable: false),
                    AircraftMake = table.Column<string>(nullable: true),
                    AircraftModel = table.Column<string>(nullable: true),
                    AircraftType = table.Column<string>(maxLength: 10, nullable: false),
                    AircraftYear = table.Column<int>(nullable: false),
                    ChangedOn = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EngineType = table.Column<string>(nullable: true),
                    GearType = table.Column<string>(nullable: true),
                    IsComplex = table.Column<bool>(nullable: false),
                    IsHighPerformance = table.Column<bool>(nullable: false),
                    IsPressurized = table.Column<bool>(nullable: false),
                    LogbookId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.AircraftId);
                    table.ForeignKey(
                        name: "FK_Aircraft_Logbooks_LogbookId",
                        column: x => x.LogbookId,
                        principalTable: "Logbooks",
                        principalColumn: "LogbookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CertificateDate = table.Column<DateTime>(nullable: false),
                    CertificateNumber = table.Column<string>(maxLength: 20, nullable: false),
                    CertificateType = table.Column<string>(maxLength: 50, nullable: false),
                    ChangedOn = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    LogbookId = table.Column<int>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK_Certificates_Logbooks_LogbookId",
                        column: x => x.LogbookId,
                        principalTable: "Logbooks",
                        principalColumn: "LogbookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChangedOn = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CurrencyTypeId = table.Column<int>(nullable: false),
                    DaysRemaining = table.Column<int>(nullable: false),
                    IsCurrent = table.Column<bool>(nullable: false),
                    IsNightCurrency = table.Column<bool>(nullable: false),
                    LogbookId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                    table.ForeignKey(
                        name: "FK_Currencies_CurrencyTypes_CurrencyTypeId",
                        column: x => x.CurrencyTypeId,
                        principalTable: "CurrencyTypes",
                        principalColumn: "CurrencyTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Currencies_Logbooks_LogbookId",
                        column: x => x.LogbookId,
                        principalTable: "Logbooks",
                        principalColumn: "LogbookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Endorsements",
                columns: table => new
                {
                    EndorsementId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CFIExpiration = table.Column<string>(nullable: true),
                    CFIName = table.Column<string>(nullable: true),
                    CFINumber = table.Column<string>(nullable: true),
                    ChangedOn = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EndorsementDate = table.Column<DateTime>(nullable: false),
                    LogbookId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endorsements", x => x.EndorsementId);
                    table.ForeignKey(
                        name: "FK_Endorsements_Logbooks_LogbookId",
                        column: x => x.LogbookId,
                        principalTable: "Logbooks",
                        principalColumn: "LogbookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pilots",
                columns: table => new
                {
                    PilotId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressLine1 = table.Column<string>(nullable: true),
                    AddressLine2 = table.Column<string>(nullable: true),
                    CellPhoneNumber = table.Column<string>(nullable: true),
                    ChangedOn = table.Column<DateTime>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    HomePhoneNumber = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    LogbookId = table.Column<int>(nullable: false),
                    PostalCode = table.Column<string>(nullable: true),
                    StateOrProvince = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.PilotId);
                    table.ForeignKey(
                        name: "FK_Pilots_Logbooks_LogbookId",
                        column: x => x.LogbookId,
                        principalTable: "Logbooks",
                        principalColumn: "LogbookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    FlightId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AircraftId = table.Column<int>(nullable: false),
                    ChangedOn = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    DepartureCode = table.Column<string>(maxLength: 5, nullable: false),
                    DestinationCode = table.Column<string>(maxLength: 5, nullable: false),
                    FlightDate = table.Column<DateTime>(nullable: false),
                    FlightTimeActualInstrument = table.Column<decimal>(nullable: false),
                    FlightTimeCrossCountry = table.Column<decimal>(nullable: false),
                    FlightTimeDay = table.Column<decimal>(nullable: false),
                    FlightTimeDual = table.Column<decimal>(nullable: false),
                    FlightTimeNight = table.Column<decimal>(nullable: false),
                    FlightTimePIC = table.Column<decimal>(nullable: false),
                    FlightTimeSimulatedInstrument = table.Column<decimal>(nullable: false),
                    FlightTimeSolo = table.Column<decimal>(nullable: false),
                    FlightTimeTotal = table.Column<decimal>(nullable: false),
                    IsCheckRide = table.Column<bool>(nullable: false),
                    IsFlightReview = table.Column<bool>(nullable: false),
                    IsInstrumentProficiencyCheck = table.Column<bool>(nullable: false),
                    LogbookId = table.Column<int>(nullable: false),
                    NumberOfHolds = table.Column<int>(nullable: false),
                    NumberOfLandingsDay = table.Column<int>(nullable: false),
                    NumberOfLandingsNight = table.Column<int>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    Route = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.FlightId);
                    table.ForeignKey(
                        name: "FK_Flights_Aircraft_AircraftId",
                        column: x => x.AircraftId,
                        principalTable: "Aircraft",
                        principalColumn: "AircraftId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Flights_Logbooks_LogbookId",
                        column: x => x.LogbookId,
                        principalTable: "Logbooks",
                        principalColumn: "LogbookId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CertificateId = table.Column<int>(nullable: false),
                    ChangedOn = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    RatingDate = table.Column<DateTime>(nullable: false),
                    RatingType = table.Column<string>(maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_Ratings_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "CertificateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Approaches",
                columns: table => new
                {
                    ApproachId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AirportCode = table.Column<string>(nullable: false),
                    ApproachType = table.Column<string>(nullable: false),
                    ChangedOn = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    FlightId = table.Column<int>(nullable: false),
                    IsCircleToLand = table.Column<bool>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    Runway = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approaches", x => x.ApproachId);
                    table.ForeignKey(
                        name: "FK_Approaches_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "FlightId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aircraft_LogbookId",
                table: "Aircraft",
                column: "LogbookId");

            migrationBuilder.CreateIndex(
                name: "IX_Approaches_FlightId",
                table: "Approaches",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_LogbookId",
                table: "Certificates",
                column: "LogbookId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CurrencyTypeId",
                table: "Currencies",
                column: "CurrencyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_LogbookId",
                table: "Currencies",
                column: "LogbookId");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_LogbookId",
                table: "Endorsements",
                column: "LogbookId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AircraftId",
                table: "Flights",
                column: "AircraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_LogbookId",
                table: "Flights",
                column: "LogbookId");

            migrationBuilder.CreateIndex(
                name: "IX_Logbooks_UserId",
                table: "Logbooks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pilots_LogbookId",
                table: "Pilots",
                column: "LogbookId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_CertificateId",
                table: "Ratings",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Approaches");

            migrationBuilder.DropTable(
                name: "ApproachTypes");

            migrationBuilder.DropTable(
                name: "CategoriesAndClasses");

            migrationBuilder.DropTable(
                name: "CertificateTypes");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Endorsements");

            migrationBuilder.DropTable(
                name: "EndorsementTypes");

            migrationBuilder.DropTable(
                name: "EngineTypes");

            migrationBuilder.DropTable(
                name: "GearTypes");

            migrationBuilder.DropTable(
                name: "Pilots");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "RatingTypes");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "CurrencyTypes");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Aircraft");

            migrationBuilder.DropTable(
                name: "Logbooks");
        }
    }
}
