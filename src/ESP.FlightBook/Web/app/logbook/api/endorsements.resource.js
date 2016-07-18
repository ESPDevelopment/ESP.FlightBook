(function () {
    'use strict';

    // Add resource (service) to the module
    angular
        .module('app.logbook')
        .factory('endorsementsResource', endorsementsResource);

    // Inject dependencies
    endorsementsResource.$inject = ['cacheService', 'utilService', 'ENVIRONMENT_CONFIG'];

    // Define resource (service)
    function endorsementsResource(cacheService, utilService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var endorsements = [];
        var isDataLoaded = false;
        var resource;

        // Define the resource (service)
        var service = {

            endorsements: endorsements,
            isDataLoaded: isDataLoaded,

            addEndorsement: addEndorsement,
            deleteEndorsement: deleteEndorsement,
            getEndorsement: getEndorsement,
            queryEndorsements: queryEndorsements,
            updateEndorsement: updateEndorsement,
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_ENDORSEMENTS, { logbookId: '@logbookId', endorsementId: '@endorsementId' });
        }

        // Add an endorsement
        function addEndorsement(logbookId, endorsement) {
            var promise = resource.add({ logbookId: logbookId }, endorsement).$promise
                .then(flushCacheEntries(logbookId));
            return promise;
        }

        // Delete an endorsement
        function deleteEndorsement(logbookId, endorsementId) {
            var promise = resource.delete({ logbookId: logbookId, endorsementId: endorsementId }).$promise
                .then(flushCacheEntries(logbookId, endorsementId));
            return promise;
        }

        // Flush appropriate cache entries
        function flushCacheEntries(logbookId, endorsementId) {
            if (angular.isDefined(logbookId)) {

                // Flush endorsements entries
                var endorsementsUri = ENVIRONMENT_CONFIG.RESOURCE_URL_ENDORSEMENTS.replace(':logbookId', logbookId);
                if (angular.isDefined(endorsementId)) {
                    var getEndorsementKey = endorsementsUri.replace(':endorsementId', endorsementId);
                    cacheService.flushCacheEntry(getEndorsementKey);
                }
                var queryEndorsementsKey = endorsementsUri.replace('/:endorsementId', '');
                cacheService.flushCacheEntry(queryEndorsementsKey);

                // Flush currency entries
                var currenciesUri = ENVIRONMENT_CONFIG.RESOURCE_URL_CURRENCIES.replace(':logbookId', logbookId);
                var queryCurrenciesKey = currenciesUri.replace('/:currencyId', '');
                cacheService.flushCacheMatches(queryCurrenciesKey);
            }
        }

        // Get an endorsement
        function getEndorsement(logbookId, endorsementId) {
            var promise = resource.get({ logbookId: logbookId, endorsementId: endorsementId }).$promise;
            return promise;
        }

        // Query endorsements
        function queryEndorsements(logbookId) {
            var promise = resource.query({ logbookId: logbookId }).$promise
                .then(queryEndorsementsSucceeded, queryEndorsementsFailed);
            return promise;
        }

        // Handle failed query endorsements
        function queryEndorsementsFailed(err) {
            utilService.clearArray(endorsements);
            isDataLoaded = false;
            return err;
        }

        // Handle successful query endorsements
        function queryEndorsementsSucceeded(response) {
            utilService.clearArray(endorsements);
            utilService.addToArray(endorsements, response.data);
            isDataLoaded = true;
            return response;
        }

        // Update an endorsement
        function updateEndorsement(logbookId, endorsementId, endorsement) {
            var promise = resource.update({ logbookId: logbookId, endorsementId: endorsementId }, endorsement).$promise
                .then(flushCacheEntries(logbookId, endorsementId));
            return promise;
        }
    }
})();