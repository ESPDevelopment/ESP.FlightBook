(function () {
    'use strict';

    // Add resource (service) to the module
    angular
        .module('app.logbook')
        .factory('ratingsResource', ratingsResource);

    // Inject dependencies
    ratingsResource.$inject = ['cacheService', 'utilService', 'ENVIRONMENT_CONFIG'];

    // Define the resource
    function ratingsResource(cacheService, utilService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var isDataLoaded = false;
        var ratings = [];
        var resource;

        // Define the resource (service)
        var service = {

            isDataLoaded: isDataLoaded,
            ratings: ratings,

            addRating: addRating,
            deleteRating: deleteRating,
            getRating: getRating,
            queryRatings: queryRatings,
            updateRating: updateRating
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_RATINGS, { logbookId: '@logbookId', certificateId: '@certificateId', ratingId: '@ratingId' });
        }

        // Add a rating
        function addRating(logbookId, certificateId, rating) {
            var promise = resource.add({ logbookId: logbookId, certificateId: certificateId }, rating).$promise
                .then(flushCacheEntries(logbookId, certificateId));
            return promise;
        }

        // Delete a rating
        function deleteRating(logbookId, certificateId, ratingId) {
            var promise = resource.delete({ logbookId: logbookId, certificateId: certificateId, ratingId: ratingId }).$promise
                .then(flushCacheEntries(logbookId, certificateId, ratingId));
            return promise;
        }

        // Flush appropriate cache entries
        function flushCacheEntries(logbookId, certificateId, ratingId) {
            if (angular.isDefined(logbookId)) {

                // Flush certificates entries
                var certificatesUri = ENVIRONMENT_CONFIG.RESOURCE_URL_CERTIFICATES.replace(':logbookId', logbookId);
                if (angular.isDefined(certificateId)) {
                    var getCertificateKey = certificatesUri.replace(':certificateId', certificateId);
                    cacheService.flushCacheEntry(getCertificateKey);
                }
                var queryCertificatesKey = certificatesUri.replace('/:certificateId', '');
                cacheService.flushCacheEntry(queryCertificatesKey);

                // Flush ratings entries
                if (angular.isDefined(ratingId)) {
                    var ratingsUri = ENVIRONMENT_CONFIG.RESOURCE_URL_RATINGS.replace(':logbookId', logbookId);
                    ratingsUri = ratingsUri.replace(':certificateId', certificateId);
                    var getRatingKey = ratingsUri.replace(':ratingId', ratingId);
                    cacheService.flushCacheEntry(getRatingKey);
                    var queryRatingsKey = ratingsUri.replace('/:ratingId', '');
                    cacheService.flushCacheEntry(queryRatingsKey);
                }
            }
        }

        // Get a rating
        function getRating(logbookId, certificateId, ratingId) {
            var promise = resource.get({ logbookId: logbookId, certificateId: certificateId, ratingId: ratingId }).$promise;
            return promise;
        }

        // Query ratings
        function queryRatings(logbookId, certificateId) {
            var promise = resource.query({ logbookId: logbookId, certificateId: certificateId }).$promise
                .then(queryRatingsSucceeded, queryRatingsFailed);
            return promise;
        }

        // Handle failed query ratings
        function queryRatingsFailed(err) {
            utilService.clearArray(ratings);
            isDataLoaded = false;
            return err;
        }

        // Handle successful query ratings
        function queryRatingsSucceeded(response) {
            utilService.clearArray(ratings);
            utilService.addToArray(ratings, response.data);
            isDataLoaded = true;
            return response;
        }

        // Update a rating
        function updateRating(logbookId, certificateId, ratingId, rating) {
            var promise = resource.update({ logbookId: logbookId, certificateId: certificateId, ratingId: ratingId }, rating).$promise
                .then(flushCacheEntries(logbookId, certificateId, ratingId));
            return promise;
        }
    }
})();