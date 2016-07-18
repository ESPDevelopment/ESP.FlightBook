(function () {
    'use strict';

    // Add configuration to the module
    angular
        .module('app.logbook')
        .config(config);

    // Inject dependencies
    config.$inject = ['$routeProvider', 'CacheFactoryProvider'];

    // Define the configuration
    function config($routeProvider, CacheFactoryProvider) {

        // Configure angular cache
        angular.extend(CacheFactoryProvider.defaults, { maxAge: 15 * 60 * 1000 });

        // Configure application routes
        $routeProvider.when('/logbooks', {
            templateUrl: '/app/logbook/logbooks/_logbooks.html',
            controller: 'LogbooksController',
            controllerAs: 'logbooksVm'
        });
        $routeProvider.when('/pilot', {
            templateUrl: '/app/logbook/pilot/_pilot.html',
            controller: 'PilotController',
            controllerAs: 'pilotVm',
            bindToController: true
        });
        $routeProvider.when('/aircraft', {
            templateUrl: '/app/logbook/aircraft/_aircraft.html',
            controller: 'AircraftController',
            controllerAs: 'aircraftVm'
        });
        $routeProvider.when('/flights', {
            templateUrl: '/app/logbook/flights/_flights.html',
            controller: 'FlightsController',
            controllerAs: 'flightsVm'
        });
        $routeProvider.when('/summary', {
            templateUrl: '/app/logbook/summary/_summary.html',
            controller: 'SummaryController',
            controllerAs: 'summaryVm'
        });
        $routeProvider.when('/currency', {
            templateUrl: '/app/logbook/currencies/_currencies.html',
            controller: 'CurrenciesController',
            controllerAs: 'currenciesVm'
        });
    }
})();