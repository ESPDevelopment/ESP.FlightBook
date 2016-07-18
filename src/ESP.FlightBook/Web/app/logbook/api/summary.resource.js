(function () {
    'use strict';

    // Add resource (service) to the module
    angular
        .module('app.logbook')
        .factory('summaryResource', summaryResource);

    // Inject dependencies
    summaryResource.$inject = ['cacheService', 'ENVIRONMENT_CONFIG'];

    // Define resource (service)
    function summaryResource(cacheService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var aircraftSummaryResource;
        var hoursSummaryResource;
        var landingsSummaryResource;

        // Define the resource (service)
        var service = {
            getAircraftSummary: getAircraftSummary,
            getHoursSummary: getHoursSummary,
            getLandingsSummary: getLandingsSummary
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            aircraftSummaryResource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_AIRCRAFT_SUMMARY, { logbookId: '@logbookId' });
            hoursSummaryResource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_HOURS_SUMMARY, { logbookId: '@logbookId' });
            landingsSummaryResource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_LANDINGS_SUMMARY, { logbookId: '@logbookId' });
        }

        // Get an aircraft summary
        function getAircraftSummary(logbookId) {
            return aircraftSummaryResource.get({ logbookId: logbookId }).$promise;
        }

        // Get hours summary
        function getHoursSummary(logbookId) {
            return hoursSummaryResource.get({ logbookId: logbookId }).$promise;
        }

        // Get landings summary
        function getLandingsSummary(logbookId) {
            return landingsSummaryResource.get({ logbookId: logbookId }).$promise;
        }
    }
})();