(function () {
    'use strict';

    // Add directive to the module
    angular
        .module('app.utils')
        .directive('espFocus', espFocus);

    // Inject dependencies
    espFocus.$inject = [];

    // Define the service
    function espFocus() {
        return function (scope, elem, attr) {
            scope.$on('espFocus', function (e, name) {
                if (name === attr.espFocus) {
                    elem[0].focus();
                }
            });
        };
    }
})();
