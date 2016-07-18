(function () {
    'use strict';

    // Define hours by condition directive
    angular
        .module('app.logbook')
        .directive('espHoursByCondition', espHoursByCondition);

    // Inject dependencies
    espHoursByCondition.$inject = ['LOGBOOK_CONSTANT'];

    function espHoursByCondition(LOGBOOK_CONSTANT) {
        return {
            templateUrl: LOGBOOK_CONSTANT.TEMPLATE_URL_HOURS_BY_CONDITION,
            restrict: 'E'
        }
    };
})();
