(function () {
    'use strict';

    // Add resource (service) to the module
    angular
        .module('app.logbook')
        .factory('approachesResource', approachesResource);

    // Inject dependencies
    approachesResource.$inject = ['cacheService', 'utilService', 'ENVIRONMENT_CONFIG'];

    // Define resource (service)
    function approachesResource(cacheService, utilService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var approaches = [];
        var isDataLoaded = false;
        var resource;

        // Define the resource (service)
        var service = {

            approaches: approaches,
            isDataLoaded: isDataLoaded,

            addApproach: addApproach,
            deleteApproach: deleteApproach,
            getApproach: getApproach,
            queryApproaches: queryApproaches,
            updateApproach: updateApproach,
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_APPROACHES, { logbookId: '@logbookId', flightId: '@flightId', approachId: '@approachId' });
        }

        // Add an approach
        function addApproach(logbookId, flightId, approach) {
            var promise = resource.add({ logbookId: logbookId, flightId: flightId }, approach).$promise
                .then(flushCacheEntries(logbookId, flightId));
            return promise;
        }

        // Delete an approach
        function deleteApproach(logbookId, flightId, approachId) {
            var promise = resource.delete({ logbookId: logbookId, flightId: flightId, approachId: approachId }).$promise
                .then(flushCacheEntries(logbookId, flightId, approachId));
            return promise;
        }

        // Flush appropriate cache entries
        function flushCacheEntries(logbookId, flightId, approachId) {
            if (angular.isDefined(logbookId)) {

                // Flush flight entries
                var flightsUri = ENVIRONMENT_CONFIG.RESOURCE_URL_FLIGHTS.replace(':logbookId', logbookId);
                if (angular.isDefined(flightId)) {
                    var getFlightKey = flightsUri.replace(':flightId', flightId);
                    cacheService.flushCacheEntry(getFlightKey);
                }
                var queryFlightsKey = flightsUri.replace('/:flightId', '');
                cacheService.flushCacheMatches(queryFlightsKey);

                // Flush approach entries
                if (angular.isDefined(approachId)) {
                    var approachesUri = ENVIRONMENT_CONFIG.RESOURCE_URL_APPROACHES.replace(':logbookId', logbookId);
                    approachesUri = approachesUri.replace(':flightId', flightId);
                    var getApproachKey = approachesUri.replace(':approachId', approachId);
                    cacheService.flushCacheEntry(getApproachKey);
                    var queryApproachesKey = approachesUri.replace('/:approachId', '');
                    cacheService.flushCacheEntry(queryApproachesKey);
                }
            }
        }

        // Get an approach
        function getApproach(logbookId, flightId, approachId) {
            var promise = resource.get({ logbookId: logbookId, flightId: flightId, approachId: approachId }).$promise;
            return promise;
        }

        // Query approach
        function queryApproaches(logbookId, flightId) {
            var promise = resource.query({ logbookId: logbookId, flightId: flightId }).$promise
                .then(queryApproachesSucceeded, queryApproachesFailed);
            return promise;
        }

        // Handle failed query approaches
        function queryApproachesFailed(err) {
            utilService.clearArray(approaches);
            isDataLoaded = false;
            return err;
        }

        // Handle successful query approaches
        function queryApproachesSucceeded(response) {
            utilService.clearArray(approaches);
            utilService.addToArray(approaches, response.data);
            isDataLoaded = true;
            return response;
        }

        // Update an approach
        function updateApproach(logbookId, flightId, approachId, approach) {
            var promise = resource.update({ logbookId: logbookId, flightId: flightId, approachId: approachId }, approach).$promise
                .then(flushCacheEntries(logbookId, flightId, approachId));
            return promise;
        }
    }
})();