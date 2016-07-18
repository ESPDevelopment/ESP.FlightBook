(function () {
    'use strict';

    // Add directive to module
    angular
        .module('app.logbook')
        .directive('espSlidebarLeft', espSlidebarLeft);

    // Inject dependencies
    espSlidebarLeft.$inject = [];

    // Define directive
    function espSlidebarLeft() {
        return {
            templateUrl: '/app/logbook/index/_slidebarLeft.html',
            restrict: 'E'
        }
    }
})();