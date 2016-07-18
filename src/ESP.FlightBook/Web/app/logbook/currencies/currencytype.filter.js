(function () {
    'use strict';

    // Add filter to module
    angular
        .module('app.logbook')
        .filter('currencyType', currencyType);

    // Inject dependencies
    currencyType.$inject = [];

    // Define filter
    function currencyType() {

        var filter = filter;
        return filter;

        function filter(input, isNightCurrency) {
            var label = '';
            switch (input.calculationType) {
                case 0:
                    label = 'Flight Review';
                    break;
                case 1:
                    label = isNightCurrency ? 'Night ' : 'General ';
                    label = label + input.label;
                    break;
                case 2:
                    label = 'IFR ' + input.aircraftCategory;
                    break;
            }
            return label;
        }
    }
})();