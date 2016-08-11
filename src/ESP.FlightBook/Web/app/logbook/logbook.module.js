(function () {
    'use strict';

    // Create the module
    angular
        .module('app.logbook', [
            'ngRoute',
            'ngResource',
            'ngSanitize',
            'app.utils',
            'angular-cache',
            'ngFileSaver',
            'googlechart'
        ]);
})();
