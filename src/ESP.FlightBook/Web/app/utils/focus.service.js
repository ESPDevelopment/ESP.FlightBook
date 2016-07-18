(function () {
    'use strict';

    // Define focus service
    angular
        .module('app.utils')
        .factory('focusService', focusService);

    // Inject dependencies
    focusService.$inject = ['$rootScope', '$timeout'];

    // Define the focus service
    function focusService($rootScope, $timeout) {

        // Define the service
        var setFocus = setFocus;
        var service = {
            setFocus: setFocus
        }
        return service;

        // Set focus on named element
        function setFocus(name) {
            $timeout(function () {
                $rootScope.$broadcast('espFocus', name);
            });
        }
    }
})();
