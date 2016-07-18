(function () {
    'use strict';

    // Add service to the module
    angular
        .module('app.utils')
        .factory('testService', testService);

    // Inject dependencies
    testService.$inject = ['$timeout', '$q'];

    // Implement the service
    function testService($timeout, $q) {

        // Define attributes
        var testData = {
            attribute1: 'This is an attribute',
            attribute2: [ { Name: 'name value 1'}, { Name: 'name value 2'}, { Name: 'name value 3'} ]
        };

        // Define the service
        var service = {
            testResolve: testResolve
        };
        return service;

        // Function that tests promise resolution
        function testResolve() {
            var deferred = $q.defer();
            var timer = $timeout(function () {
                $timeout.cancel(timer);
                deferred.resolve(testData);
            }, 5000);
            return deferred.promise;
        }
    }
})();