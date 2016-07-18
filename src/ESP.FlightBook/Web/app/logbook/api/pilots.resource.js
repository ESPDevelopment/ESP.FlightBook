(function () {
    'use strict';

    // Add resource (service) to module
    angular
        .module('app.logbook')
        .factory('pilotsResource', pilotsResource);

    // Inject dependencies
    pilotsResource.$inject = ['cacheService', 'ENVIRONMENT_CONFIG'];

    // Define resource (service)
    function pilotsResource(cacheService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var resource;

        // Define the resource (service)
        var service = {
            addPilot: addPilot,
            deletePilot: deletePilot,
            getPilot: getPilot,
            queryPilots: queryPilots,
            updatePilot: updatePilot,
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_PILOTS, { logbookId: '@logbookId', pilotId: '@pilotId' });
        }

        // Add a pilot
        function addPilot(logbookId, pilot) {
            var promise = resource.add({ logbookId: logbookId }, pilot).$promise
                .then(flushCacheEntries(logbookId));
            return promise;
        }

        // Delete a pilot
        function deletePilot(logbookId, pilotId) {
            var promise = resource.delete({ logbookId: logbookId, pilotId: pilotId }).$promise
                .then(flushCacheEntries(logbookId, pilotId));
            return promise;
        }

        // Flush appropriate cache entries
        function flushCacheEntries(logbookId, pilotId) {
            if (angular.isDefined(logbookId)) {

                // Flush pilots entries
                var pilotsUri = ENVIRONMENT_CONFIG.RESOURCE_URL_PILOTS.replace(':logbookId', logbookId);
                if (angular.isDefined(pilotId)) {
                    var getPilotKey = pilotsUri.replace(':pilotId', pilotId);
                    cacheService.flushCacheEntry(getPilotKey);
                }
                var queryPilotsKey = pilotsUri.replace('/:pilotId', '');
                cacheService.flushCacheEntry(queryPilotsKey);
            }
        }

        // Get a pilot
        function getPilot(logbookId, pilotId) {
            var promise = resource.get({ logbookId: logbookId, pilotId: pilotId }).$promise;
            return promise;
        }

        // Query pilots
        function queryPilots(logbookId) {
            var promise = resource.query({ logbookId: logbookId }).$promise;
            return promise;
        }

        // Update a pilot
        function updatePilot(logbookId, pilotId, pilot) {
            var promise = resource.update({ logbookId: logbookId, pilotId: pilotId }, pilot).$promise
                .then(flushCacheEntries(logbookId, pilotId));
            return promise;
        }
    }
})();