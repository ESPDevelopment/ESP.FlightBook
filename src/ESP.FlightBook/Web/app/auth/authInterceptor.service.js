(function () {
    'use strict';

    // Add the service to the module
    angular
        .module('app.auth')
        .factory('authInterceptorService', authInterceptorService);

    // Inject dependencies
    authInterceptorService.$inject = ['$injector', '$rootScope', '$q', 'httpBuffer', 'AUTH_CONSTANT'];

    // Define the service
    function authInterceptorService($injector, $rootScope, $q, httpBuffer, AUTH_CONSTANT) {

        // Define the service
        var authService;
        var service = {
            request: request,
            responseError: responseError
        };
        return service;

        // Add authorization header on requests
        function request(config) {
            if (angular.isDefined(config) && angular.isDefined(config.url)) {
                var url = config.url.substring(0, 1);
                if (url != '/') {
                    authService = authService || $injector.get('authService');
                    console.log('Calling api: ' + config.url);
                    var authData = authService.authData;
                    if (authData.isAuth == true) {
                        config.headers = config.headers || {};
                        config.headers.Authorization = AUTH_CONSTANT.HEADER + authData.accessToken;
                    }
                }
            }
            return config;
        }

        // Redirect to signin on 401 responses
        function responseError(rejection) {
            console.log('Call failed: ' + rejection.config.url);
            var config = rejection.config || {};
            switch (rejection.status) {
                case 401:
                    console.log('Deferring call: ' + rejection.config.url);
                    var deferred = $q.defer();
                    httpBuffer.append(config, deferred);
                    $rootScope.$broadcast(AUTH_CONSTANT.LOGIN_REQUIRED_EVENT, rejection);
                    return deferred.promise;
                case 403:
                    $rootScope.$broadcast(AUTH_CONSTANT.LOGIN_FORBIDDEN_EVENT, rejection);
                    break;
            }
            return $q.reject(rejection);
        }
    }
})();