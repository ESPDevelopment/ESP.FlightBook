(function () {
    'use strict';

    // Define landings and approaches directive
    angular
        .module('app.logbook')
        .directive('espLandingsAndApproaches', espLandingsAndApproaches);

    // Inject dependencies
    espLandingsAndApproaches.$inject = ['LOGBOOK_CONSTANT'];

    function espLandingsAndApproaches(LOGBOOK_CONSTANT) {
        return {
            templateUrl: LOGBOOK_CONSTANT.TEMPLATE_URL_LANDINGS_AND_APPROACHES,
            restrict: 'E'
        }
    };
})();
