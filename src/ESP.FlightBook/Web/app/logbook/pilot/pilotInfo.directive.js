(function () {
    'use strict';

    // Define pilot information directive
    angular
        .module('app.logbook')
        .directive('espPilotInfo', espPilotInfo);

    // Inject dependencies
    espPilotInfo.$inject = ['LOGBOOK_CONSTANT'];

    function espPilotInfo(LOGBOOK_CONSTANT) {
        return {
            templateUrl: LOGBOOK_CONSTANT.TEMPLATE_URL_PILOT,
            controller: 'PilotInfoController',
            controllerAs: 'pilotInfoVM',
            restrict: 'E'
        }
    };
})();
