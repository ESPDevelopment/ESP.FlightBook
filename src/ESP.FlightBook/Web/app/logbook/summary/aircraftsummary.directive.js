(function () {
    'use strict';

    // Define aircraft summary directive
    angular
        .module('app.logbook')
        .directive('espAircraftSummary', espAircraftSummary);

    // Inject dependencies
    espAircraftSummary.$inject = ['LOGBOOK_CONSTANT'];

    function espAircraftSummary(LOGBOOK_CONSTANT) {
        return {
            templateUrl: LOGBOOK_CONSTANT.TEMPLATE_URL_AIRCRAFT_SUMMARY,
            restrict: 'E'
        }
    };
})();
