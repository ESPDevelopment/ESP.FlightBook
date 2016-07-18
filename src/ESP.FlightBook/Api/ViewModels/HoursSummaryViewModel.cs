using System;
using System.Collections.Generic;

namespace ESP.FlightBook.Api.ViewModels
{
    public class HoursSummaryViewModel
    {
        // Summary Date
        public DateTime SummaryDate { get; set; }

        // Total Hours by Year
        public Dictionary<int, decimal> TotalHoursByYear { get; set; }

        // Total Hours by Type
        public Dictionary<string, decimal> TotalHoursByType { get; set; }

        // Total Hours
        public decimal HoursTotalTotal { get; set; }
        public decimal HoursTotalLast7Days { get; set; }
        public decimal HoursTotalLast30Days { get; set; }
        public decimal HoursTotalLast60Days { get; set; }
        public decimal HoursTotalLast90Days { get; set; }
        public decimal HoursTotalLast6Months { get; set; }
        public decimal HoursTotalLast12Months { get; set; }

        // PIC Hours
        public decimal HoursPICTotal { get; set; }
        public decimal HoursPICLast7Days { get; set; }
        public decimal HoursPICLast30Days { get; set; }
        public decimal HoursPICLast60Days { get; set; }
        public decimal HoursPICLast90Days { get; set; }
        public decimal HoursPICLast6Months { get; set; }
        public decimal HoursPICLast12Months { get; set; }

        // Dual Hours
        public decimal HoursDualTotal { get; set; }
        public decimal HoursDualLast7Days { get; set; }
        public decimal HoursDualLast30Days { get; set; }
        public decimal HoursDualLast60Days { get; set; }
        public decimal HoursDualLast90Days { get; set; }
        public decimal HoursDualLast6Months { get; set; }
        public decimal HoursDualLast12Months { get; set; }

        // FixedGear Hours
        public decimal HoursSoloTotal { get; set; }
        public decimal HoursSoloLast7Days { get; set; }
        public decimal HoursSoloLast30Days { get; set; }
        public decimal HoursSoloLast60Days { get; set; }
        public decimal HoursSoloLast90Days { get; set; }
        public decimal HoursSoloLast6Months { get; set; }
        public decimal HoursSoloLast12Months { get; set; }

        // Cross-country Hours
        public decimal HoursXCTotal { get; set; }
        public decimal HoursXCLast7Days { get; set; }
        public decimal HoursXCLast30Days { get; set; }
        public decimal HoursXCLast60Days { get; set; }
        public decimal HoursXCLast90Days { get; set; }
        public decimal HoursXCLast6Months { get; set; }
        public decimal HoursXCLast12Months { get; set; }

        // Day Hours
        public decimal HoursDayTotal { get; set; }
        public decimal HoursDayLast7Days { get; set; }
        public decimal HoursDayLast30Days { get; set; }
        public decimal HoursDayLast60Days { get; set; }
        public decimal HoursDayLast90Days { get; set; }
        public decimal HoursDayLast6Months { get; set; }
        public decimal HoursDayLast12Months { get; set; }

        // Night Hours
        public decimal HoursNightTotal { get; set; }
        public decimal HoursNightLast7Days { get; set; }
        public decimal HoursNightLast30Days { get; set; }
        public decimal HoursNightLast60Days { get; set; }
        public decimal HoursNightLast90Days { get; set; }
        public decimal HoursNightLast6Months { get; set; }
        public decimal HoursNightLast12Months { get; set; }

        // Actual Instrument Hours
        public decimal HoursActualInstrumentTotal { get; set; }
        public decimal HoursActualInstrumentLast7Days { get; set; }
        public decimal HoursActualInstrumentLast30Days { get; set; }
        public decimal HoursActualInstrumentLast60Days { get; set; }
        public decimal HoursActualInstrumentLast90Days { get; set; }
        public decimal HoursActualInstrumentLast6Months { get; set; }
        public decimal HoursActualInstrumentLast12Months { get; set; }

        // Simulated Instrument Hours
        public decimal HoursSimulatedInstrumentTotal { get; set; }
        public decimal HoursSimulatedInstrumentLast7Days { get; set; }
        public decimal HoursSimulatedInstrumentLast30Days { get; set; }
        public decimal HoursSimulatedInstrumentLast60Days { get; set; }
        public decimal HoursSimulatedInstrumentLast90Days { get; set; }
        public decimal HoursSimulatedInstrumentLast6Months { get; set; }
        public decimal HoursSimulatedInstrumentLast12Months { get; set; }

        // AATD Hours
        public decimal HoursAATDTotal { get; set; }
        public decimal HoursAATDLast7Days { get; set; }
        public decimal HoursAATDLast30Days { get; set; }
        public decimal HoursAATDLast60Days { get; set; }
        public decimal HoursAATDLast90Days { get; set; }
        public decimal HoursAATDLast6Months { get; set; }
        public decimal HoursAATDLast12Months { get; set; }

        // BATD Hours
        public decimal HoursBATDTotal { get; set; }
        public decimal HoursBATDLast7Days { get; set; }
        public decimal HoursBATDLast30Days { get; set; }
        public decimal HoursBATDLast60Days { get; set; }
        public decimal HoursBATDLast90Days { get; set; }
        public decimal HoursBATDLast6Months { get; set; }
        public decimal HoursBATDLast12Months { get; set; }

        // Complex Hours
        public decimal HoursComplexTotal { get; set; }
        public decimal HoursComplexLast7Days { get; set; }
        public decimal HoursComplexLast30Days { get; set; }
        public decimal HoursComplexLast60Days { get; set; }
        public decimal HoursComplexLast90Days { get; set; }
        public decimal HoursComplexLast6Months { get; set; }
        public decimal HoursComplexLast12Months { get; set; }

        // High Performance Hours
        public decimal HoursHPTotal { get; set; }
        public decimal HoursHPLast7Days { get; set; }
        public decimal HoursHPLast30Days { get; set; }
        public decimal HoursHPLast60Days { get; set; }
        public decimal HoursHPLast90Days { get; set; }
        public decimal HoursHPLast6Months { get; set; }
        public decimal HoursHPLast12Months { get; set; }

        // Pressurized Hours
        public decimal HoursPressurizedTotal { get; set; }
        public decimal HoursPressurizedLast7Days { get; set; }
        public decimal HoursPressurizedLast30Days { get; set; }
        public decimal HoursPressurizedLast60Days { get; set; }
        public decimal HoursPressurizedLast90Days { get; set; }
        public decimal HoursPressurizedLast6Months { get; set; }
        public decimal HoursPressurizedLast12Months { get; set; }

        // Fixed Gear Hours
        public decimal HoursFixedGearTotal { get; set; }
        public decimal HoursFixedGearLast7Days { get; set; }
        public decimal HoursFixedGearLast30Days { get; set; }
        public decimal HoursFixedGearLast60Days { get; set; }
        public decimal HoursFixedGearLast90Days { get; set; }
        public decimal HoursFixedGearLast6Months { get; set; }
        public decimal HoursFixedGearLast12Months { get; set; }

        // Retractable Gear Hours
        public decimal HoursRetractableGearTotal { get; set; }
        public decimal HoursRetractableGearLast7Days { get; set; }
        public decimal HoursRetractableGearLast30Days { get; set; }
        public decimal HoursRetractableGearLast60Days { get; set; }
        public decimal HoursRetractableGearLast90Days { get; set; }
        public decimal HoursRetractableGearLast6Months { get; set; }
        public decimal HoursRetractableGearLast12Months { get; set; }
    }
}
