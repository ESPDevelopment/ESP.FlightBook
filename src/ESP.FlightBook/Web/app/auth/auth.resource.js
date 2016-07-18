(function () {
    'use strict';

    // Add the resource
    angular
        .module('app.auth')
        .factory('authResource', authResource);

    // Inject dependencies
    authResource.$inject = ['$resource', 'ENVIRONMENT_CONFIG'];

    // Define the resource
    function authResource($resource, ENVIRONMENT_CONFIG) {
        return $resource(ENVIRONMENT_CONFIG.TOKEN_URI, null,
            {
                'token': { method: 'POST', url: ENVIRONMENT_CONFIG.TOKEN_URI, cache: false, ignoreAuth: true }
            });
    }
})();