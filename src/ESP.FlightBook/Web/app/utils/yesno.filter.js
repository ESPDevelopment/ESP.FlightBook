(function () {
    'use strict';

    // Add filter to the module
    angular
        .module('app.utils')
        .filter('yesNo', yesNo);

    // Inject dependencies
    yesNo.$inject = [];

    // Define the filter
    function yesNo() {
        return function (input) {
            return input ? 'Yes' : 'No';
        };
    }
})();