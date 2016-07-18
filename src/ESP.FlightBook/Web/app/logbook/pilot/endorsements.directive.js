(function () {
    'use strict';

    // Define endorsements directive
    angular
        .module('app.logbook')
        .directive('espEndorsements', espEndorsements);

    // Inject dependencies
    espEndorsements.$inject = ['LOGBOOK_CONSTANT'];

    function espEndorsements(LOGBOOK_CONSTANT) {
        return {
            templateUrl: LOGBOOK_CONSTANT.TEMPLATE_URL_ENDORSEMENTS,
            controller: 'EndorsementsController',
            controllerAs: 'endorsementsVM',
            restrict: 'E'
        }
    };
})();
