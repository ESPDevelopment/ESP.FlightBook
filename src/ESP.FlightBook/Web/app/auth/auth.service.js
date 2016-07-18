(function () {
    'use strict';

    // Add the service to the module
    angular
        .module('app.auth')
        .factory('authService', authService);

    // Inject dependencies
    authService.$inject = ['$rootScope', '$location', 'httpBuffer', 'localStorageService', 'authResource', 'tokenService', 'AUTH_CONSTANT'];

    // Define the service
    function authService($rootScope, $location, httpBuffer, localStorageService, authResource, tokenService, AUTH_CONSTANT) {

        // Define the service
        var authData = {
            accessToken: '',
            isAuth: false,
            rememberMe: false,
        }
        var isValidating = false;
        var service = {
            authData: authData,
            initialize: initialize,
            isLoggedIn: isLoggedIn,
            isProtected: isProtected,
            isTokenExpired: isTokenExpired,
            loginConfirmed: loginConfirmed,
            loginCancelled: loginCancelled,
            refreshAccessToken: refreshAccessToken,
            saveAuthData: saveAuthData,
            signout: signout
        };
        return service;

        // Initialize authentication data
        function initialize() {
            loadAuthData();
            isValidating = false;
            $rootScope.$on('$routeChangeStart', isProtected);
            $rootScope.$on(AUTH_CONSTANT.LOGIN_REQUIRED_EVENT, refreshAccessToken);
        }

        // Determines whether the user is logged in or not
        function isLoggedIn() {
            return (angular.isDefined(authData) &&
                    angular.isDefined(authData.isAuth) &&
                    angular.isDefined(authData.accessToken) &&
                    authData.accessToken.length > 0 &&
                    authData.isAuth == true);
        }

        // Redirects protected routes to the sigin page
        function isProtected(event, next, current) {
            if (!isLoggedIn()) {
                if ($.inArray($location.path(), AUTH_CONSTANT.PATH_PUBLIC_PATHS) < 0) {
                    return $location.path(AUTH_CONSTANT.PATH_SIGNIN);
                }
            }
        }

        // Determines whether the token has expired
        function isTokenExpired() {
            return (angular.isUndefined(authData) ||
                    angular.isUndefined(authData.accessToken) ||
                    authData.accessToken.length <= 0 ||
                    tokenService.isTokenExpired(authData.accessToken) == true);
        }

        // Loads authentication data
        function loadAuthData() {
            var savedAuthData = localStorageService.get(AUTH_CONSTANT.DATA_KEY);
            if (savedAuthData) {
                if (savedAuthData.rememberMe) {
                    authData.accessToken = savedAuthData.accessToken;
                    authData.isAuth = true;
                    authData.rememberMe = savedAuthData.rememberMe;
                } else {
                    localStorageService.remove(AUTH_CONSTANT.DATA_KEY);
                }
            }
        }

        // Login confirmed
        function loginConfirmed(data) {
            $rootScope.$broadcast(AUTH_CONSTANT.LOGIN_CONFIRMED_EVENT, data);
            httpBuffer.retryAll();
        }

        // Login cancelled
        function loginCancelled(data, reason) {
            $rootScope.$broadcast(AUTH_CONSTANT.LOGIN_CANCELLED_EVENT, data);
            httpBuffer.rejectAll(reason);
        }

        // Refresh access token
        function refreshAccessToken() {
            if (isValidating) {
                return;
            }
            if (isLoggedIn()) {
                isValidating = true;
                var request = { accessToken: authData.accessToken };
                authResource.token(null, request).$promise.then(
                    function (response) {
                        updateAccessToken(response.newAccessToken);
                        loginConfirmed(response);
                        isValidating = false;
                    },
                    function (err) {
                        loginCancelled(null, AUTH_CONSTANT.NOT_AUTHORIZED_MESSAGE);
                        isValidating = false;
                    }
                );
            } else {
                loginCancelled(null, AUTH_CONSTANT.NOT_LOGGED_IN_MESSAGE);
                isValidating = false;
            }
        }

        // Reset authentication data
        function resetAuthData() {
            authData.accessToken = '';
            authData.isAuth = false;
            authData.rememberMe = false;
        }

        // Save authentication data
        function saveAuthData(accessToken, rememberMe) {
            var decodedToken = tokenService.decodeAccessToken(accessToken);
            authData.accessToken = accessToken;
            authData.isAuth = !tokenService.isTokenExpired(accessToken);
            authData.rememberMe = rememberMe;
            localStorageService.set(AUTH_CONSTANT.DATA_KEY, authData);
        }

        // Signout (clear auth data)
        function signout() {
            resetAuthData();
            localStorageService.remove(AUTH_CONSTANT.DATA_KEY);
        }

        // Update access token
        function updateAccessToken(accessToken) {
            saveAuthData(accessToken, authData.rememberMe);
        }
    }
})();