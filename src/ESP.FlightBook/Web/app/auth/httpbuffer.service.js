(function () {
    'use strict';

    // Add the service to the module
    angular
        .module('app.auth')
        .factory('httpBuffer', httpBuffer);

    // Inject dependencies
    httpBuffer.$inject = ['$injector'];

    // Define the service
    function httpBuffer($injector) {

        // Define service
        var buffer = [];
        var $http; // Service initialized later because of circular dependency problem
        var service = {
            append: append,
            rejectAll: rejectAll,
            retryAll: retryAll
        }
        return service;

        // Append request to the buffer
        function append(config, deferred) {
            buffer.push({
                config: config,
                deferred: deferred
            });
        }

        // Reject all requests in the buffer
        function rejectAll(reason) {
            if (reason) {
                for (var i = 0; i < buffer.length; ++i) {
                    buffer[i].deferred.reject(reason);
                }
            }
            buffer = [];
        }

        // Retry all requests in the buffer
        function retryAll() {
            for (var i = 0; i < buffer.length; ++i) {
                retryHttpRequest(buffer[i].config, buffer[i].deferred);
            }
            buffer = [];
        }

        // Retry an http request
        function retryHttpRequest(config, deferred) {
            function successCallback(response) {
                deferred.resolve(response);
            }
            function errorCallback(response) {
                deferred.reject(response);
            }
            $http = $http || $injector.get('$http');
            $http(config).then(successCallback, errorCallback);
        }
    }
})();