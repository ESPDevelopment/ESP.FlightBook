(function () {
    'use strict';

    // Add configuration to the module
    angular
        .module('app.auth')
        .config(config);

    // Inject dependencies
    config.$inject = ['$httpProvider'];

    // Define the configuration
    function config($httpProvider) {

        // Configure the auth interceptor
        $httpProvider.interceptors.push('authInterceptorService');
    }
})();