(function () {
    'use strict';

    // Add resource (service) to module
    angular
        .module('app.logbook')
        .factory('aircraftResource', aircraftResource);

    // Inject dependencies
    aircraftResource.$inject = ['utilService', 'cacheService', 'ENVIRONMENT_CONFIG'];

    // Define resource (service)
    function aircraftResource(utilService, cacheService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var aircraft = [];
        var isDataLoaded = false;
        var resource;

        // Define the resource (service)
        var service = {

            aircraft: aircraft,
            isDataLoaded: isDataLoaded,

            addAircraft: addAircraft,
            deleteAircraft: deleteAircraft,
            getAircraft: getAircraft,
            queryAircraft: queryAircraft,
            updateAircraft: updateAircraft,
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_AIRCRAFT, { logbookId: '@logbookId', aircraftId: '@aircraftId', page: '@page', itemsPerPage: '@itemsPerPage' });
        }

        // Add an aircraft
        function addAircraft(logbookId, aircraft) {
            var promise = resource.add({ logbookId: logbookId }, aircraft).$promise
                .then(flushCacheEntries(logbookId));
            return promise;
        }

        // Delete an aircraft
        function deleteAircraft(logbookId, aircraftId) {
            var promise = resource.delete({ logbookId: logbookId, aircraftId: aircraftId }).$promise
                .then(flushCacheEntries(logbookId, aircraftId));
            return promise;
        }

        // Flush appropriate cache entries
        function flushCacheEntries(logbookId, aircraftId) {
            if (angular.isDefined(logbookId)) {

                // Flush aircraft entries
                var aircraftUri = ENVIRONMENT_CONFIG.RESOURCE_URL_AIRCRAFT.replace(':logbookId', logbookId);
                if (angular.isDefined(aircraftId)) {
                    var getAircraftKey = aircraftUri.replace(':aircraftId', aircraftId);
                    cacheService.flushCacheEntry(getAircraftKey);
                }
                var queryAircraftKey = aircraftUri.replace('/:aircraftId', '');
                cacheService.flushCacheMatches(queryAircraftKey);

                // Flush summary entries
                var aircraftSummaryKey = ENVIRONMENT_CONFIG.RESOURCE_URL_AIRCRAFT_SUMMARY.replace(':logbookId', logbookId);
                cacheService.flushCacheEntry(aircraftSummaryKey);
                var hoursSummaryKey = ENVIRONMENT_CONFIG.RESOURCE_URL_HOURS_SUMMARY.replace(':logbookId', logbookId);
                cacheService.flushCacheEntry(hoursSummaryKey);

                // Flush currency entries
                var currenciesUri = ENVIRONMENT_CONFIG.RESOURCE_URL_CURRENCIES.replace(':logbookId', logbookId);
                var queryCurrenciesKey = currenciesUri.replace('/:currencyId', '');
                cacheService.flushCacheMatches(queryCurrenciesKey);
            }
        }

        // Get an aircraft
        function getAircraft(logbookId, aircraftId) {
            var promise = resource.get({ logbookId: logbookId, aircraftId: aircraftId }).$promise;
            return promise;
        }

        // Query aircraft
        function queryAircraft(logbookId, page, itemsPerPage) {
            var promise = resource.query({ logbookId: logbookId, page: page, itemsPerPage: itemsPerPage }).$promise
                .then(queryAircraftSucceeded, queryAircraftFailed);
            return promise;
        }

        // Handle failed query aircraft
        function queryAircraftFailed(err) {
            utilService.clearArray(aircraft);
            isDataLoaded = false;
            return err;
        }

        // Handle successful query aircraft
        function queryAircraftSucceeded(response) {
            utilService.clearArray(aircraft);
            utilService.addToArray(aircraft, response.data);
            isDataLoaded = true;
            return response;
        }

        // Update an aircraft
        function updateAircraft(logbookId, aircraftId, aircraft) {
            var promise = resource.update({ logbookId: logbookId, aircraftId: aircraftId }, aircraft).$promise
                .then(flushCacheEntries(logbookId, aircraftId));
            return promise;
        }
    }
})();