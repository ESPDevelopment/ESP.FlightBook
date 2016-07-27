(function () {
    'use strict';

    // Add filter to the module
    angular
        .module('app.logbook')
        .filter('filterFlights', filterFlights);

    // Inject dependencies
    filterFlights.$inject = ['utilService'];

    // Define the filter
    function filterFlights(utilService) {

        var filter = filter;
        return filter;

        // Define the filter function
        function filter(items, filterCriteria) {

            // Validate item collection
            if (!angular.isArray(items)) {
                return items;
            }

            // Initialize new array
            var filteredFlights = [];

            // Initialize filter criteria
            var filterFlightDateStart = '';
            var filterFlightDateEnd = '';
            var filterAircraftIdentifier = '';
            var filterAircraftType = '';

            // Save validated filter criteria
            if (angular.isDefined(filterCriteria)) {
                if (angular.isDate(filterCriteria.flightDateStart)) {
                    filterFlightDateStart = utilService.stringToDate(filterCriteria.flightDateStart);
                }
                if (angular.isDate(filterCriteria.flightDateEnd)) {
                    filterFlightDateEnd = utilService.stringToDate(filterCriteria.flightDateEnd);
                }
                if (angular.isString(filterCriteria.aircraftIdentifier)) {
                    filterAircraftIdentifier = filterCriteria.aircraftIdentifier;
                }
                if (angular.isString(filterCriteria.aircraftType)) {
                    filterAircraftType = filterCriteria.aircraftType;
                }
            }

            // Iterate items in the original collection
            for (var i = 0; i < items.length; i++) {

                // Initialize match flag
                var match = true;

                // Initialize data upon which to filter
                var item = items[i];
                var flightDate = (angular.isDefined(item.flightDate)) ? utilService.stringToDate(item.flightDate) : '';
                var aircraftIdentifier = (angular.isDefined(item.aircraft) && angular.isDefined(item.aircraft.aircraftIdentifier)) ? item.aircraft.aircraftIdentifier : '';
                var aircraftType = (angular.isDefined(item.aircraft) && angular.isDefined(item.aircraft.aircraftType)) ? item.aircraft.aircraftType : '';

                // Filter by flight date start
                if (angular.isDate(filterFlightDateStart)) {
                    if (flightDate < filterFlightDateStart) match = false;
                }

                // Filter by flight date end
                if (angular.isDate(filterFlightDateEnd)) {
                    if (flightDate > filterFlightDateEnd) match = false;
                }

                // Filter by aircraft identifier
                if (filterAircraftIdentifier.length > 0 && aircraftIdentifier != filterAircraftIdentifier) {
                    match = false;
                }

                // Filter by aircraft type
                if (filterAircraftType.length > 0 && aircraftType != filterAircraftType) {
                    match = false;
                }

                // Add matching items to the filtered array
                if (match) {
                    filteredFlights.push(item);
                }
            }
            return filteredFlights;
        }
    }
})();