using ESP.FlightBook.Api.Models;
using System.Collections.Generic;

namespace ESP.FlightBook.Api.ViewModels
{
    public class ConstantsViewModel
    {
        public List<ApproachType> ApproachTypes { get; set; }
        public List<CategoryAndClass> CategoriesAndClasses { get; set; }
        public List<CertificateType> CertificateTypes { get; set; }
        public List<CurrencyType> CurrencyTypes { get; set; }
        public List<EndorsementType> EndorsementTypes { get; set; }
        public List<EngineType> EngineTypes { get; set; }
        public List<GearType> GearTypes { get; set; }
        public List<RatingType> RatingTypes { get; set; }
    }
}
