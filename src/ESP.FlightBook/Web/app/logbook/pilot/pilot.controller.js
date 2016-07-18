(function () {
    'use strict';

    // Define pilot controller
    angular
        .module('app.logbook')
        .controller('PilotController', PilotController);

    // Inject dependencies
    PilotController.$inject = ['logbookService'];

    function PilotController(logbookService) {

        var vm = this;

        // Available attributes
        vm.message = '';

        // Available functions
        vm.activate = activate;

        // Activate the controller
        activate();

        // Initializes the controller
        function activate() {
        }
    };
})();

