using ESP.FlightBook.Api.Models;
using ESP.FlightBook.Data;
using ESP.FlightBook.Extensions;
using System.Linq;

namespace ESP.FlightBook.Api.Extensions
{
    public static class ApplicationDbContextExtensions
    {
        /// <summary>
        /// Ensure that application data has been seeded
        /// </summary>
        /// <param name="context"></param>
        public static void EnsureSeedData(this ApplicationDbContext context)
        {
            // Seed data if all migrations have been applied
            if (context.AllMigrationsApplied())
            {
                SeedApproachTypes(context);
                SeedCategoriesAndClasses(context);
                SeedCertificateTypes(context);
                SeedCurrencyTypes(context);
                SeedEndorsementTypes(context);
                SeedEngineTypes(context);
                SeedGearTypes(context);
                SeedRatingTypes(context);
            }
        }

        /// <summary>
        /// Seed data for the ApproachTypes table
        /// </summary>
        /// <param name="context">Database context</param>
        private static void SeedApproachTypes(ApplicationDbContext context)
        {
            if (!context.ApproachTypes.Any())
            {
                context.ApproachTypes.AddRange(
                    new ApproachType { Label = "ASR/SRA", SortOrder = 1 },
                    new ApproachType { Label = "GCA", SortOrder = 2 },
                    new ApproachType { Label = "GLS", SortOrder = 3 },
                    new ApproachType { Label = "ILS", SortOrder = 4 },
                    new ApproachType { Label = "ILS CAT II", SortOrder = 5 },
                    new ApproachType { Label = "ILS CAT III", SortOrder = 6 },
                    new ApproachType { Label = "LDA", SortOrder = 7 },
                    new ApproachType { Label = "LOC", SortOrder = 8 },
                    new ApproachType { Label = "LOC BC", SortOrder = 9 },
                    new ApproachType { Label = "LOC DME", SortOrder = 10 }, 
                    new ApproachType { Label = "MLS", SortOrder = 11 },
                    new ApproachType { Label = "NDB", SortOrder = 12 },
                    new ApproachType { Label = "PAR", SortOrder = 13 },
                    new ApproachType { Label = "RNAV (GPS)", SortOrder = 14 },
                    new ApproachType { Label = "RNAV (RNP)", SortOrder = 15 },
                    new ApproachType { Label = "SDF", SortOrder = 16 },
                    new ApproachType { Label = "TACAN", SortOrder = 17 },
                    new ApproachType { Label = "VOR", SortOrder = 18 },
                    new ApproachType { Label = "VOR/DME", SortOrder = 19 }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seed data for the CategoriesAndClasses table
        /// </summary>
        /// <param name="context">Database context</param>
        private static void SeedCategoriesAndClasses(ApplicationDbContext context)
        {
            if (!context.CategoriesAndClasses.Any())
            {
                context.CategoriesAndClasses.AddRange(
                    new CategoryAndClass { Label = "Airplane Single Engine Land", Category = "Airplane", Class = "Single Engine Land", Abbreviation = "ASEL" },
                    new CategoryAndClass { Label = "Airplane Single Engine Sea", Category = "Airplane", Class = "Single Engine Sea", Abbreviation = "ASES" },
                    new CategoryAndClass { Label = "Airplane Multi Engine Land", Category = "Airplane", Class = "Multi Engine Land", Abbreviation = "AMEL" },
                    new CategoryAndClass { Label = "Airplane Multi Engine Sea", Category = "Airplane", Class = "Multi Engine Sea", Abbreviation = "AMES" },
                    new CategoryAndClass { Label = "Rotorcraft Helicopter", Category = "Rotorcraft", Class = "Helicopter", Abbreviation = "RH" },
                    new CategoryAndClass { Label = "Rotorcraft Gyroplane", Category = "Rotorcraft", Class = "Gyroplane", Abbreviation = "RG" },
                    new CategoryAndClass { Label = "Glider", Category = "Glider", Class = "Glider", Abbreviation = "" },
                    new CategoryAndClass { Label = "Lighter Than Air Airship", Category = "Lighter Than Air", Class = "Airship", Abbreviation = "LA" },
                    new CategoryAndClass { Label = "Lighter Than Air Balloon", Category = "Lighter Than Air", Class = "Balloon", Abbreviation = "LB" },
                    new CategoryAndClass { Label = "Powered Lift", Category = "Powered Lift", Class = "Powered Lift", Abbreviation = "" },
                    new CategoryAndClass { Label = "Powered Parachute Land", Category = "Powered Parachute", Class = "Powered Parachute Land", Abbreviation = "PL" },
                    new CategoryAndClass { Label = "Powered Parachute Sea", Category = "Powered Parachute", Class = "Powered Parachute Sea", Abbreviation = "PS" },
                    new CategoryAndClass { Label = "Weight Shift Control Land", Category = "Weight Shift Control", Class = "Weight Shift Control Land", Abbreviation = "WL" },
                    new CategoryAndClass { Label = "Weight Shift Control Sea", Category = "Weight Shift Control", Class = "Weight Shift Control Sea", Abbreviation = "WS" },
                    new CategoryAndClass { Label = "Full Flight Simulator", Category = "Full Flight Simulator", Class = "Full Flight Simulator", Abbreviation = "FFS" },
                    new CategoryAndClass { Label = "Flight Training Device", Category = "Flight Training Device", Class = "Flight Training Device", Abbreviation = "FTD" },
                    new CategoryAndClass { Label = "Aviation Training Device", Category = "Aviation Training Device", Class = "Aviation Training Device", Abbreviation = "ATD" }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seed data for the CertificateTypes table
        /// </summary>
        /// <param name="context">Database context</param>
        private static void SeedCertificateTypes(ApplicationDbContext context)
        {
            if (!context.CertificateTypes.Any())
            {
                context.CertificateTypes.AddRange(
                    new CertificateType { SortOrder = 1, Label = "Student Pilot" },
                    new CertificateType { SortOrder = 2, Label = "Sport Pilot" },
                    new CertificateType { SortOrder = 3, Label = "Recreational Pilot" },
                    new CertificateType { SortOrder = 4, Label = "Private Pilot" },
                    new CertificateType { SortOrder = 5, Label = "Commercial Pilot" },
                    new CertificateType { SortOrder = 6, Label = "Airline Transport Pilot" },
                    new CertificateType { SortOrder = 7, Label = "Flight Instructor" },
                    new CertificateType { SortOrder = 8, Label = "Ground Instructor" }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seed data for the CurrencyTypes table
        /// </summary>
        /// <param name="context">Database context</param>
        private static void SeedCurrencyTypes(ApplicationDbContext context)
        {
            if (!context.CurrencyTypes.Any())
            {
                context.CurrencyTypes.AddRange(
                    new CurrencyType { SortOrder = 1, Category = "Flight Review", Label = "Flight Review", AircraftCategory = "", AircraftClass = "", Abbreviation = "", CalculationType = CurrencyType.CalculationTypes.FlightReview, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 2, Category = "Airplane", Label = "Airplane Single Engine Land", AircraftCategory = "Airplane", AircraftClass = "Single Engine Land", Abbreviation = "ASEL", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 3, Category = "Airplane", Label = "Airplane Single Engine Land Tailwheel", AircraftCategory = "Airplane", AircraftClass = "Single Engine Land", Abbreviation = "ASELT", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = true },
                    new CurrencyType { SortOrder = 4, Category = "Airplane", Label = "Airplane Multi Engine Land", AircraftCategory = "Airplane", AircraftClass = "Multi Engine Land", Abbreviation = "AMEL", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 5, Category = "Airplane", Label = "Airplane Multi Engine Land Tailwheel", AircraftCategory = "Airplane", AircraftClass = "Multi Engine Land", Abbreviation = "AMELT", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = true },
                    new CurrencyType { SortOrder = 6, Category = "Airplane", Label = "Airplane Single Engine Sea", AircraftCategory = "Airplane", AircraftClass = "Single Engine Sea", Abbreviation = "ASES", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 7, Category = "Airplane", Label = "Airplane Single Engine Sea Tailwheel", AircraftCategory = "Airplane", AircraftClass = "Single Engine Sea", Abbreviation = "ASEST", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = true },
                    new CurrencyType { SortOrder = 8, Category = "Airplane", Label = "Airplane Multi Engine Sea", AircraftCategory = "Airplane", AircraftClass = "Multi Engine Sea", Abbreviation = "AMES", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 9, Category = "Airplane", Label = "Airplane Multi Engine Sea Tailwheel", AircraftCategory = "Airplane", AircraftClass = "Multi Engine Sea", Abbreviation = "AMEST", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 10, Category = "Rotorcraft", Label = "Rotorcraft Helicopter", AircraftCategory = "Rotorcraft", AircraftClass = "Helicopter", Abbreviation = "RH", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 11, Category = "Rotorcraft", Label = "Rotorcraft Gyroplane", AircraftCategory = "Rotorcraft", AircraftClass = "Gyroplane", Abbreviation = "RG", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 12, Category = "Glider", Label = "Glider", AircraftCategory = "Glider", AircraftClass = "Glider", Abbreviation = "", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 13, Category = "Lighter Than Air", Label = "Lighter Than Air Airship", AircraftCategory = "Lighter Than Air", Abbreviation = "LA", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 14, Category = "Lighter Than Air", Label = "Lighter Than Air Balloon", AircraftCategory = "Lighter Than Air", AircraftClass = "Balloon", Abbreviation = "LB", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 15, Category = "Powered Lift", Label = "Powered Lift", AircraftCategory = "Powered Lift", AircraftClass = "Powered Lift", Abbreviation = "", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 16, Category = "Powered Parachute", Label = "Powered Parachute Land", AircraftCategory = "Powered Parachute", AircraftClass = "Powered Parachute Land", Abbreviation = "PL", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 17, Category = "Powered Parachute", Label = "Powered Parachute Sea", AircraftCategory = "Powered Parachute", AircraftClass = "Powered Parachute Sea", Abbreviation = "PS", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 18, Category = "Weight Shift Control", Label = "Weight Shift Control Land", AircraftCategory = "Weight Shift Control", AircraftClass = "Weight Shift Control Land", Abbreviation = "WL", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 19, Category = "Weight Shift Control", Label = "Weight Shift Control Sea", AircraftCategory = "Weight Shift Control", AircraftClass = "Weight Shift Control Sea", Abbreviation = "WS", CalculationType = CurrencyType.CalculationTypes.General, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 20, Category = "IFR", Label = "Airplane", AircraftCategory = "Airplane", AircraftClass = "", Abbreviation = "", CalculationType = CurrencyType.CalculationTypes.IFR, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 21, Category = "IFR", Label = "Powered Lift", AircraftCategory = "Powered Lift", AircraftClass = "", Abbreviation = "", CalculationType = CurrencyType.CalculationTypes.IFR, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 22, Category = "IFR", Label = "Rotorcraft", AircraftCategory = "Rotorcraft", AircraftClass = "", Abbreviation = "", CalculationType = CurrencyType.CalculationTypes.IFR, RequiresTailwheel = false },
                    new CurrencyType { SortOrder = 23, Category = "IFR", Label = "Lighter Than Air", AircraftCategory = "Lighter Than Air", AircraftClass = "", Abbreviation = "", CalculationType = CurrencyType.CalculationTypes.IFR, RequiresTailwheel = false }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seed data for the EndorsementTypes table
        /// </summary>
        /// <param name="context">Database context</param>
        private static void SeedEndorsementTypes(ApplicationDbContext context)
        {
            if (!context.EndorsementTypes.Any())
            {
                context.EndorsementTypes.AddRange(
                    new EndorsementType { SortOrder = 1, Category = "Student Pilot", Label = "Solo Flight", Template = "I certify that [NAME] has satisfactorily completed the presolo knowledge exam of §61.87(b), received the required presolo training required by §61.87(c), and has demonstrated the proficiency of §61.87(d) and is proficient to make solo flights in [MAKE and MODEL AIRCRAFT]." },
                    new EndorsementType { SortOrder = 2, Category = "Student Pilot", Label = "Solo Flight – Night", Template = "I certify that [NAME] has received the required presolo training in a [MAKE and MODEL AIRCRAFT] and determined that he/she has demonstrated the proficiency of §61.87(o) and is proficient to make solo flights at night in a [MAKE and MODEL AIRCRAFT]." },
                    new EndorsementType { SortOrder = 3, Category = "Student Pilot", Label = "Solo Flight – Additional Airports", Template = "I certify that [NAME] has received the required training of §61.93(b)(1). I have determined that he/she is proficient to practice solo takeoffs and landings at [AIRPORT NAME] subject to the following conditions and limitations: [LIST CONDITIONS/LIMITATIONS]." },
                    new EndorsementType { SortOrder = 4, Category = "Student Pilot", Label = "Solo Flight – Additional 90 Days", Template = "I certify that [NAME] has received the required training to qualify for solo flying. I have determined he/she meets the applicable requirements of §61.87(n) and is proficient to make solo flights in a [MAKE and MODEL AIRCRAFT]." },
                    new EndorsementType { SortOrder = 5, Category = "Student Pilot", Label = "Solo Flight – Repeat Cross Country", Template = "I certify that [NAME] has received the required training in both directions between and at both [AIRPORT NAMES] and determined that he/she is proficient in §61.93(b)(2) to conduct repeated solo cross-country flights over that route, which is not more than 50NM from the point of departure, subject to the following conditions and limitations: [LIST CONDITIONS/LIMITATIONS]." },
                    new EndorsementType { SortOrder = 6, Category = "Student Pilot", Label = "Solo Flight – Class B", Template = "I certify that [NAME] has received the required training of §61.95(a) and determined he/she is proficient to conduct solo flights in [NAME OF CLASS B] Class B airspace, subject to the following conditions and limitations: [LIST CONDITIONS/LIMITATIONS]." },
                    new EndorsementType { SortOrder = 7, Category = "Student Pilot", Label = "Solo Flight – Class B Airport", Template = "I certify that [NAME] has received the required training of §61.95(a)(1) and determined that he/she is proficient to conduct solo flight operations at [NAME OF AIRPORT] in Class B airspace, subject to the following conditions and limitations: [LIST OF CONDITIONS/LIMITATIONS]." },
                    new EndorsementType { SortOrder = 8, Category = "Student Pilot", Label = "Solo Flight – Cross Country", Template = "I certify that [NAME] has received the required solo cross-country training and find he/she has met the applicable requirements of §61.93 and is proficient to make solo cross-country flights in a [MAKE and MODEL AIRCRAFT]." },
                    new EndorsementType { SortOrder = 9, Category = "Student Pilot", Label = "Solo Flight – Cross Country Planning", Template = "I have reviewed the cross-country planning of [NAME] and find the planning and preparation to be correct to make the solo flight from [LOCATION AIRPORT] to [DESTINATION AIRPORT] via [ROUTE OF FLIGHT] with landings at [AIRPORT NAMES] in a [MAKE and MODEL AIRCRAFT], subject to the following conditions and limitations: [LIST OF CONDITIONS/LIMITATIONS]." },
                    new EndorsementType { SortOrder = 10, Category = "Private Pilot", Label = "Private Pilot – Knowledge Exam", Template = "I certify that [NAME] has received the required training of §61.105 and/or I have reviewed the home study curriculum and have determined he/she is prepared for the Private Pilot knowledge exam." },
                    new EndorsementType { SortOrder = 11, Category = "Private Pilot", Label = "Private Pilot – Practical Test", Template = "I certify that [NAME] has received the required training of §61.105, §61.107 and §61.109; has been given flight instruction in preparation for the Private Pilot practical test within the proceeding 60 days; and has been given additional instruction in areas found deficient on his/her Private Pilot knowledge exam. I have determined he/she is prepared for the Private Pilot practical test." },
                    new EndorsementType { SortOrder = 12, Category = "Recreational Pilot", Label = "Recreational Pilot – Knowledge Exam", Template = "I certify that [NAME] has received the required training of §61.97(b) and/or I have reviewed the home study curriculum and have determined that he/she is prepared for the [PRACTICAL TEST]." },
                    new EndorsementType { SortOrder = 13, Category = "Recreational Pilot", Label = "Recreational Pilot – Practical Test", Template = "I certify that [NAME] has received the required training of §61.98(b) and §61.99 and have determined that he/she is prepared for the [PRACTICAL TEST]." },
                    new EndorsementType { SortOrder = 14, Category = "Recreational Pilot", Label = "Recreational Pilot – Additional Airport", Template = "I certify that [NAME] has received the reqiured training of §61.101(b) and have determined he/she is competent to operate at the [AIRPORT NAME]." },
                    new EndorsementType { SortOrder = 15, Category = "Recreational Pilot", Label = "Recreational Pilot – Cross Country", Template = "I certify that [NAME] has received the required cross-country training of §61.101(c) and have determined that he/she is proficient in cross-country flying of part 61, subpart E." },
                    new EndorsementType { SortOrder = 16, Category = "Recreational Pilot", Label = "Recreational Pilot – Recurrent Training", Template = "I certify that [NAME] has received the required 180-day recurrent training of 61.101(f) in a [MAKE and MODEL AIRCRAFT]. I have determined him/her to be proficient to act as PIC of that aircraft." },
                    new EndorsementType { SortOrder = 17, Category = "Instrument Rating", Label = "Instrument Rating – Knowledge Exam", Template = "I certify that [NAME] has received the required training of §61.65(b) and/or I have reviewed the home study curriculum and have determined that he/she is prepared for the [KNOWLEDGE EXAM]." },
                    new EndorsementType { SortOrder = 18, Category = "Instrument Rating", Label = "Instrument Rating – Practical Test", Template = "I certify that [NAME] has received the required training of §61.65(c) and (d) and have determined he/she is prepared for the [PRACTICAL TEST]." },
                    new EndorsementType { SortOrder = 19, Category = "Commercial Pilot", Label = "Commercial Pilot – Knowledge Exam", Template = "I certify that [NAME] has received the required training of §61.125 and/or I have reviewed the home study curriculum and have determined that he/she is prepared for the [KNOWLEDGE EXAM]." },
                    new EndorsementType { SortOrder = 20, Category = "Commercial Pilot", Label = "Commercial Pilot – Practical Test", Template = "I certify that [NAME] has received the required training of §61.127 and §61.129 and have determined he/she is prepared for the [PRACTICAL TEST]." },
                    new EndorsementType { SortOrder = 21, Category = "Other", Label = "Category/Class/Type Rating", Template = "I certify that [NAME] has received the training as required by 61.31(d)(3) to serve as PIC ina [CATEGORY and CLASS AIRCRAFT]. I have determined that he/she is prepared to serve as PIC in that [CATEGORY and CLASS AIRCRAFT]." },
                    new EndorsementType { SortOrder = 22, Category = "Other", Label = "Category/Class Rating – Practical Test", Template = "I certify tnat [NAME] [PILOT CERTIFICATE] [CERTIFICATE NUMBER] has received the required training for an additional [CATEGORY CLASS RATING]. I have determined that he/she is prepared for the [PRACTICAL TEST] for the addition of a [CATEGORY CLASS RATING]." },
                    new EndorsementType { SortOrder = 23, Category = "Other", Label = "Flight Review", Template = "I certify that [NAME], [PILOT CERTIFICATE], [CERTIFICATE NUMBER] has satisfactorily completed a flight review of §61.56(a)." },
                    new EndorsementType { SortOrder = 24, Category = "Other", Label = "Instrument Proficiency Check", Template = "I certify that [NAME], [PILOT CERTIFICATE], [CERTIFICATE NUMBER] has satisfactorily completed the instrument proficiency check of §61.57(d) in a [MAKE and MODEL AIRCRAFT]." },
                    new EndorsementType { SortOrder = 25, Category = "Other", Label = "Complex Airplane", Template = "I certify that [NAME], [PILOT CERTIFICATE], [CERTIFICATE NUMBER] has received the required training of §61.31(e) in a [MAKE and MODEL COMPLEX AIRCRAFT]. I have determined that he/she is proficient in the operation and systems of a complex airplane." },
                    new EndorsementType { SortOrder = 26, Category = "Other", Label = "High Performance Airplane", Template = "I certify that [NAME], [PILOT CERTIFICATE], [CERTIFICATE NUMBER] has received the required training of §61.31(f) in a [MAKE and MODEL HIGH PERFORMANCE AIRCRAFT]. I have determined that he/she is proficient in the operation and systems of a high performance airplane." },
                    new EndorsementType { SortOrder = 27, Category = "Other", Label = "Pressurized Aircraft", Template = "I certify that [NAME], [PILOT CERTIFICATE], [CERTIFICATE NUMBER] has received the required training of §61.31(g) in a [MAKE and MODEL PRESSURIZED AIRCRAFT]. I have determined that he/she is proficient in the operation and systems of a pressurized aircraft." },
                    new EndorsementType { SortOrder = 28, Category = "Other", Label = "Tailwheel Airplane", Template = "I certify that [NAME], [PILOT CERTIFICATE], [CERTIFICATE NUMBER] has received the required training of §61.31(i) in a [MAKE and MODEL TAILWHEEL AIRPLANE]. I have determined that he/she is proficient in the operation of a tailwheel airplane." },
                    new EndorsementType { SortOrder = 29, Category = "Other", Label = "Retesting After Failure", Template = "I certify that [NAME] has received the additional [FLIGHT AND/OR GROUND] training as required by §61.49 and determined that he/she is prepared for the [KNOWLEDGE/PRACTICAL TEST]." }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seed data for the EngineTypes table
        /// </summary>
        /// <param name="context">Database context</param>
        private static void SeedEngineTypes(ApplicationDbContext context)
        {
            if (!context.EngineTypes.Any())
            {
                context.EngineTypes.AddRange(
                    new EngineType { SortOrder = 1, Label = "Diesel" },
                    new EngineType { SortOrder = 2, Label = "Electric" },
                    new EngineType { SortOrder = 3, Label = "Non-Powered" },
                    new EngineType { SortOrder = 4, Label = "Piston" },
                    new EngineType { SortOrder = 5, Label = "Radial" },
                    new EngineType { SortOrder = 6, Label = "Turbofan" },
                    new EngineType { SortOrder = 7, Label = "Turbojet" },
                    new EngineType { SortOrder = 8, Label = "Turboprop" }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seed data for the GearTypes table
        /// </summary>
        /// <param name="context">Database context</param>
        private static void SeedGearTypes(ApplicationDbContext context)
        {
            if (!context.GearTypes.Any())
            {
                context.GearTypes.AddRange(
                    new GearType { SortOrder = 1, Label = "Amphibian", Abbreviation = "AM" },
                    new GearType { SortOrder = 2, Label = "Fixed Tailwheel", Abbreviation = "FC" },
                    new GearType { SortOrder = 3, Label = "Fixed Tricycle", Abbreviation = "FT" },
                    new GearType { SortOrder = 4, Label = "Floats", Abbreviation = "FL" },
                    new GearType { SortOrder = 5, Label = "Retractable Tailwheel", Abbreviation = "RC" },
                    new GearType { SortOrder = 6, Label = "Retractable Tricycle", Abbreviation = "RT" },
                    new GearType { SortOrder = 7, Label = "Skids", Abbreviation = "" },
                    new GearType { SortOrder = 8, Label = "Skis", Abbreviation = "" }
                );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Seed data for the RatingTypes table
        /// </summary>
        /// <param name="context">Database context</param>
        private static void SeedRatingTypes(ApplicationDbContext context)
        {
            if (!context.RatingTypes.Any())
            {
                context.RatingTypes.AddRange(
                    new RatingType { SortOrder = 1, Label = "Instrument - Airplane" },
                    new RatingType { SortOrder = 2, Label = "Instrument - Helicopter" },
                    new RatingType { SortOrder = 3, Label = "Instrument - Powered-Lift" },
                    new RatingType { SortOrder = 4, Label = "Airplane Single Engine Land" },
                    new RatingType { SortOrder = 5, Label = "Airplane Single Engine Sea" },
                    new RatingType { SortOrder = 6, Label = "Airplane Multi Engine Land" },
                    new RatingType { SortOrder = 7, Label = "Airplane Multi Engine Sea" },
                    new RatingType { SortOrder = 8, Label = "Rotorcraft Helicopter" },
                    new RatingType { SortOrder = 9, Label = "Rotorcraft Gyroplane" },
                    new RatingType { SortOrder = 10, Label = "Glider" },
                    new RatingType { SortOrder = 11, Label = "Lighter Than Air Airship" },
                    new RatingType { SortOrder = 12, Label = "Lighter Than Air Balloon" }
                );
                context.SaveChanges();
            }
        }
    }
}
