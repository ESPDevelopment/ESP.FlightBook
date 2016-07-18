(function () {
    'use strict';

    // Add resource (service) to the module
    angular
        .module('app.logbook')
        .factory('logbooksResource', logbooksResource);

    // Inject dependencies
    logbooksResource.$inject = ['utilService', 'cacheService', 'ENVIRONMENT_CONFIG'];

    // Define resource (service)
    function logbooksResource(utilService, cacheService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var isDataLoaded = false;
        var logbooks = [];
        var resource;

        // Define the resource (service)
        var service = {

            isDataLoaded: isDataLoaded,
            logbooks: logbooks,

            addLogbook: addLogbook,
            deleteLogbook: deleteLogbook,
            getLogbook: getLogbook,
            queryLogbooks: queryLogbooks,
            updateLogbook: updateLogbook,
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_LOGBOOKS, { logbookId: '@logbookId' });
        }

        // Add a logbook
        function addLogbook(logbook) {
            var promise = resource.add({}, logbook).$promise
                .then(flushCacheEntries());
            return promise;
        }

        // Delete a logbook
        function deleteLogbook(logbookId) {
            var promise = resource.delete({ logbookId: logbookId }).$promise
                .then(flushCacheEntries(logbookId));
            return promise;
        }

        // Flush appropriate cache entries
        function flushCacheEntries(logbookId) {
            if (angular.isDefined(logbookId)) {

                // Flush logbook entries
                var logbookUri = ENVIRONMENT_CONFIG.RESOURCE_URL_LOGBOOKS.replace(':logbookId', logbookId);
                var getLogbookKey = logbookUri.replace('/:logbookId', '');
                cacheService.flushCacheMatches(getLogbookKey);
            }
            var queryLogbooksKey = ENVIRONMENT_CONFIG.RESOURCE_URL_LOGBOOKS.replace('/:logbookId', '');
            cacheService.flushCacheEntry(queryLogbooksKey);
        }

        // Get a logbook
        function getLogbook(logbookId) {
            var promise = resource.get({ logbookId: logbookId }).$promise;
            return promise;
        }

        // Query logbooks
        function queryLogbooks() {
            var promise = resource.query().$promise
                .then(queryLogbooksSucceeded, queryLogbooksFailed);
            return promise;
        }

        // Handle failed query logbooks
        function queryLogbooksFailed(err) {
            utilService.clearArray(logbooks);
            isDataLoaded = false;
            return err;
        }

        // Handle successful query logbooks
        function queryLogbooksSucceeded(response) {
            utilService.clearArray(logbooks);
            utilService.addToArray(logbooks, response.data);
            isDataLoaded = true;
            return response;
        }

        // Update a logbook
        function updateLogbook(logbookId, logbook) {
            var promise = resource.update({ logbookId: logbookId }, logbook).$promise
                .then(flushCacheEntries(logbookId));
            return promise;
        }
    }
})();