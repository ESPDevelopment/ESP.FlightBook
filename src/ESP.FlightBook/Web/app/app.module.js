(function () {
    'use strict';

    // Create the module
    angular
        .module('app', [
            'app.config',
            'app.auth',
            'app.account',
            'app.utils',
            'app.logbook',
            'ngRoute'
        ]);
})();
