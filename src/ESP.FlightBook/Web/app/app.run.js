(function () {
    'use strict';

    // Add run block to the module
    angular
        .module('app')
        .run(runBlock);

    // Inject dependencies
    runBlock.$inject = ['authService', 'logbookService'];

    // Define the run block
    function runBlock(authService, logbookService) {

        // Initialize the auth service
        authService.initialize();

        // Initialize the logbook service
        logbookService.initialize();
    }
})();