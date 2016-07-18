(function () {
    'use strict';

    // Add resource (service) to module
    angular
        .module('app.logbook')
        .factory('constantsResource', constantsResource);

    // Inject dependencies
    constantsResource.$inject = ['cacheService', 'ENVIRONMENT_CONFIG'];

    // Define resource (service)
    function constantsResource(cacheService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var constants = {};
        var isDataLoaded = false;
        var resource;

        // Define the resource (service)
        var service = {
            constants: constants,
            isDataLoaded: isDataLoaded,

            getConstants: getConstants
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_CONSTANTS, {});
        }

        // Get constants
        function getConstants() {
            var promise = resource.get().$promise
                .then(getConstantsSucceeded, getConstantsFailed);
            return promise;
        }

        // Handle failed get
        function getConstantsFailed(err) {
            isDataLoaded = false;
            return err;
        }

        // Handle successful get
        function getConstantsSucceeded(response) {
            angular.copy(response, constants);
            isDataLoaded = true;
            return response;
        }
    }
})();