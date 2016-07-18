(function () {
    'use strict';

    // Add constants to the module
    angular
        .module('app.auth')

        // Define constants for the module
        .constant('AUTH_CONSTANT', {
            DATA_KEY: 'authData',
            HEADER: 'Bearer ',
            LOGIN_CONFIRMED_EVENT: 'event:auth-loginConfirmed',
            LOGIN_CANCELLED_EVENT: 'event:auth-loginCancelled',
            LOGIN_REQUIRED_EVENT: 'event:auth-loginRequired',
            LOGIN_FORBIDDEN_EVENT: 'event:auth-forbidden',
            NOT_AUTHORIZED_MESSAGE: 'User not authorized',
            NOT_LOGGED_IN_MESSAGE: 'User is not logged in',
            PATH_PUBLIC_PATHS: ['', '/home', '/contact', '/about', '/signin', '/register', '/account/pending', '/account/confirm', '/account/resetPassword'],
            PATH_SIGNIN: '/signin'
        });
})();