(function () {
    'use strict';

    // Define certificates directive
    angular
        .module('app.logbook')
        .directive('espCertificates', espCertificates);

    // Inject dependencies
    espCertificates.$inject = ['LOGBOOK_CONSTANT'];

    function espCertificates(LOGBOOK_CONSTANT) {
        return {
            templateUrl: LOGBOOK_CONSTANT.TEMPLATE_URL_CERTIFICATES,
            controller: 'CertificatesController',
            controllerAs: "certificatesVM",
            restrict: 'E'
        }
    };
})();
