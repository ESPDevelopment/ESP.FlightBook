(function () {
    'use strict';

    // Add directive to the module
    angular
        .module('app.logbook')
        .directive('espNavigation', espNavigation);

    // Inject dependencies
    espNavigation.$inject = [];

    // Define the directive
    function espNavigation() {
        return {
            templateUrl: '/app/logbook/index/_navigation.html',
            restrict: 'E'
        }
    }
})();


