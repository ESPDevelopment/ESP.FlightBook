(function () {
    'use strict';

    // Define hours by piloting time directive
    angular
        .module('app.logbook')
        .directive('espHoursByPilotingTime', espHoursByPilotingTime);

    // Inject dependencies
    espHoursByPilotingTime.$inject = ['LOGBOOK_CONSTANT'];

    function espHoursByPilotingTime(LOGBOOK_CONSTANT) {
        return {
            templateUrl: LOGBOOK_CONSTANT.TEMPLATE_URL_HOURS_BY_PILOTING_TIME,
            restrict: 'E'
        }
    };
})();
