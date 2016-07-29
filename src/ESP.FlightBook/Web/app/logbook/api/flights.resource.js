(function () {
    'use strict';

    // Add resource (service) to module
    angular
        .module('app.logbook')
        .factory('flightsResource', flightsResource);

    // Inject dependencies
    flightsResource.$inject = ['cacheService', 'utilService', 'ENVIRONMENT_CONFIG'];

    // Define the resource (service)
    function flightsResource(cacheService, utilService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var flights = [];
        var isDataLoaded = false;
        var resource;

        // Define the resource (service)
        var service = {

            flights: flights,
            isDataLoaded: isDataLoaded,

            addFlight: addFlight,
            deleteFlight: deleteFlight,
            getFlight: getFlight,
            queryFlights: queryFlights,
            updateFlight: updateFlight,
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_FLIGHTS, {
                logbookId: '@logbookId',
                flightId: '@flightId',
                page: '@page',
                itemsPerPage: '@itemsPerPage',
                flightDateStart: '@flightDateStart',
                flightDateEnd: '@flightDateEnd',
                aircraftIdentifier: '@aircraftIdentifier',
                aircraftType: '@aircraftType',
                isComplex: '@isComplex',
                isRetractable: '@isRetractable'
            });
        }

        // Add a flight
        function addFlight(logbookId, flight) {
            var promise = resource.add({ logbookId: logbookId }, flight).$promise
                .then(flushCacheEntries(logbookId));
            return promise;
        }

        // Delete a flight
        function deleteFlight(logbookId, flightId) {
            var promise = resource.delete({ logbookId: logbookId, flightId: flightId }).$promise
                .then(flushCacheEntries(logbookId, flightId));
            return promise;
        }

        // Flush appropriate cache entries
        function flushCacheEntries(logbookId, flightId) {
            if (angular.isDefined(logbookId)) {

                // Flush flights entries
                var flightsUri = ENVIRONMENT_CONFIG.RESOURCE_URL_FLIGHTS.replace(':logbookId', logbookId);
                if (angular.isDefined(flightId)) {
                    var getFlightKey = flightsUri.replace(':flightId', flightId);
                    cacheService.flushCacheEntry(getFlightKey);
                }
                var queryFlightsKey = flightsUri.replace('/:flightId', '');
                cacheService.flushCacheMatches(queryFlightsKey);

                // Flush summary entries
                var aircraftSummaryKey = ENVIRONMENT_CONFIG.RESOURCE_URL_AIRCRAFT_SUMMARY.replace(':logbookId', logbookId);
                cacheService.flushCacheEntry(aircraftSummaryKey);
                var hoursSummaryKey = ENVIRONMENT_CONFIG.RESOURCE_URL_HOURS_SUMMARY.replace(':logbookId', logbookId);
                cacheService.flushCacheEntry(hoursSummaryKey);
                var landingsSummaryKey = ENVIRONMENT_CONFIG.RESOURCE_URL_LANDINGS_SUMMARY.replace(':logbookId', logbookId);
                cacheService.flushCacheEntry(landingsSummaryKey);

                // Flush currency entries
                var currenciesUri = ENVIRONMENT_CONFIG.RESOURCE_URL_CURRENCIES.replace(':logbookId', logbookId);
                var queryCurrenciesKey = currenciesUri.replace('/:currencyId', '');
                cacheService.flushCacheMatches(queryCurrenciesKey);
            }
        }

        // Get a flight
        function getFlight(logbookId, flightId) {
            var promise = resource.get({ logbookId: logbookId, flightId: flightId }).$promise;
            return promise;
        }

        // Query flights
        function queryFlights(logbookId, page, itemsPerPage, filter) {
            var query = '';
            if (angular.isDefined(filter) && angular.isDefined(filter.active) && filter.active == true) {
                query = {
                    logbookId: logbookId,
                    page: page,
                    itemsPerPage: itemsPerPage,
                    flightDateStart: filter.flightDateStart,
                    flightDateEnd: filter.flightDateEnd,
                    aircraftIdentifier: filter.aircraftIdentifier,
                    aircraftType: filter.aircraftType,
                    isComplex: filter.isComplex,
                    isRetractable: filter.isRetractable
                };
            } else {
                query = {
                    logbookId: logbookId,
                    page: page,
                    itemsPerPage: itemsPerPage
                };
            }
            var promise = resource.query(query).$promise.then(queryFlightsSucceeded, queryFlightsFailed);
            return promise;
        }

        // Handle failed query flights
        function queryFlightsFailed(err) {
            utilService.clearArray(flights);
            isDataLoaded = false;
            return err;
        }

        // Handle successful query aircraft
        function queryFlightsSucceeded(response) {
            utilService.clearArray(flights);
            utilService.addToArray(flights, response.data);
            isDataLoaded = true;
            return response;
        }

        // Update a flight
        function updateFlight(logbookId, flightId, flight) {
            var promise = resource.update({ logbookId: logbookId, flightId: flightId }, flight).$promise
                .then(flushCacheEntries(logbookId, flightId));
            return promise;
        }
    }
})();