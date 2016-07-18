(function () {
    'use strict';

    // Add resource (service) to the module
    angular
        .module('app.logbook')
        .factory('certificatesResource', certificatesResource);

    // Inject dependencies
    certificatesResource.$inject = ['cacheService', 'utilService', 'ENVIRONMENT_CONFIG'];

    // Define the resource (service)
    function certificatesResource(cacheService, utilService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var certificates = [];
        var isDataLoaded = false;
        var resource;

        // Define the resource (service)
        var service = {

            certificates: certificates,
            isDataLoaded: isDataLoaded,

            addCertificate: addCertificate,
            deleteCertificate: deleteCertificate,
            getCertificate: getCertificate,
            queryCertificates: queryCertificates,
            updateCertificate: updateCertificate
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_CERTIFICATES, { logbookId: '@logbookId', certificateId: '@certificateId' });
        }

        // Add a certificate
        function addCertificate(logbookId, certificate) {
            var promise = resource.add({ logbookId: logbookId }, certificate).$promise
                .then(flushCacheEntries(logbookId));
            return promise;
        }

        // Delete a certificate
        function deleteCertificate(logbookId, certificateId) {
            var promise = resource.delete({ logbookId: logbookId, certificateId: certificateId }).$promise
                .then(flushCacheEntries(logbookId, certificateId));
            return promise;
        }

        // Flush appropriate cache entries
        function flushCacheEntries(logbookId, certificateId) {
            if (angular.isDefined(logbookId)) {

                // Flush certificates entries
                var certificatesUri = ENVIRONMENT_CONFIG.RESOURCE_URL_CERTIFICATES.replace(':logbookId', logbookId);
                if (angular.isDefined(certificateId)) {
                    var getCertificateKey = certificatesUri.replace(':certificateId', certificateId);
                    cacheService.flushCacheEntry(getCertificateKey);
                }
                var queryCertificatesKey = certificatesUri.replace('/:certificateId', '');
                cacheService.flushCacheEntry(queryCertificatesKey);
            }
        }

        // Get a certificate
        function getCertificate(logbookId, certificateId) {
            var promise = resource.get({ logbookId: logbookId, certificateId: certificateId }).$promise;
            return promise;
        }

        // Query certificates
        function queryCertificates(logbookId) {
            var promise = resource.query({ logbookId: logbookId }).$promise
                .then(queryCertificatesSucceeded, queryCertificatesFailed);
            return promise;
        }

        // Handle failed query certificates
        function queryCertificatesFailed(err) {
            utilService.clearArray(certificates);
            isDataLoaded = false;
            return err;
        }

        // Handle successful query certificates
        function queryCertificatesSucceeded(response) {
            utilService.clearArray(certificates);
            utilService.addToArray(certificates, response.data);
            isDataLoaded = true;
            return response;
        }

        // Update a certificate
        function updateCertificate(logbookId, certificateId, certificate) {
            var promise = resource.update({ logbookId: logbookId, certificateId: certificateId }, certificate).$promise
                .then(flushCacheEntries(logbookId, certificateId));
            return promise;
        }
    }
})();