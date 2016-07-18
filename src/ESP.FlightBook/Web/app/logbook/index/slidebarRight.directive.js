(function () {
    'use strict';

    // Add directive to module
    angular
        .module('app.logbook')
        .directive('espSlidebarRight', espSlidebarRight);

    // Inject dependencies
    espSlidebarRight.$inject = [];

    // Define directive
    function espSlidebarRight() {
        return {
            templateUrl: '/app/logbook/index/_slidebarRight.html',
            restrict: 'E'
        }
    }
})();