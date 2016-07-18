(function () {
    'use strict';

    // Add configuration to the module
    angular
        .module('app')
        .config(config);

    // Inject dependencies
    config.$inject = ['$httpProvider', '$routeProvider'];

    // Define the configuration
    function config($httpProvider, $routeProvider) {

        // Disable IE ajax request caching
        $httpProvider.defaults.cache = true;
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }
        $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';

        // Configure application routes
        $routeProvider.when('/home', {
            templateUrl: '/app/logbook/index/_home.html',
            controller: 'IndexController',
            controllerAs: 'indexVm',
        });
        $routeProvider.when('/about', {
            templateUrl: '/app/logbook/index/_about.html',
            controller: 'IndexController',
            controllerAs: 'indexVm'
        });
        $routeProvider.when('/contact', {
            templateUrl: '/app/logbook/index/_contact.html',
            controller: 'IndexController',
            controllerAs: 'indexVm'
        });
        $routeProvider.when('/signin', {
            templateUrl: '/app/account/_signin.html',
            controller: 'AccountController',
            controllerAs: 'accountVm'
        });
        $routeProvider.when('/register', {
            templateUrl: '/app/account/_signin.html',
            controller: 'AccountController',
            controllerAs: 'accountVm'
        });
        $routeProvider.when('/account/pending', {
            templateUrl: '/app/account/_pending.html',
            controller: 'AccountController',
            controllerAs: 'accountVm'
        });
        $routeProvider.when('/account/resetPassword', {
            templateUrl: '/app/account/_resetPassword.html',
            controller: 'AccountController',
            controllerAs: 'accountVm'
        });
        $routeProvider.when('/account/changePassword', {
            templateUrl: '/app/account/_changePassword.html',
            controller: 'AccountController',
            controllerAs: 'accountVm'
        });
        $routeProvider.when('/account/confirm', {
            templateUrl: '/app/account/_confirm.html',
            controller: 'ConfirmController',
            controllerAs: 'confirmVm'
        });
        $routeProvider.otherwise({ redirectTo: '/home' });
    }
})();