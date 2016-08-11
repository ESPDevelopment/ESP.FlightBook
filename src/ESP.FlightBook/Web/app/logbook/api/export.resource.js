(function () {
    'use strict';

    // Add resource to the module
    angular
        .module('app.logbook')
        .factory('exportResource', exportResource);

    // Inject dependencies
    exportResource.$inject = ['$resource', 'ENVIRONMENT_CONFIG'];

    // Define the resource
    function exportResource($resource, ENVIRONMENT_CONFIG) {

        // Define attributes
        var resource;

        // Define the resource (service)
        var service = {
            exportLogbook: exportLogbook,
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = $resource(ENVIRONMENT_CONFIG.EXPORT_LOGBOOK_URI, null,
                {
                    'export': { method: 'GET', url: ENVIRONMENT_CONFIG.EXPORT_LOGBOOK_URI },
                });
        }

        // Export a logbook
        function exportLogbook(logbookId) {
            var promise = resource.export({ logbookId: logbookId }).$promise;
            return promise;
        }
    }
})();