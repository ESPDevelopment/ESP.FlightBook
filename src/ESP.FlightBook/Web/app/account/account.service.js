(function () {
    'use strict';

    // Add the service to the module
    angular
        .module('app.account')
        .factory('accountService', accountService);

    // Inject dependencies
    accountService.$inject = ['$q', '$rootScope', '$location', '$timeout', 'accountResource', 'authService', 'ACCOUNT_CONSTANT', 'ENVIRONMENT_CONFIG'];

    // Define the service
    function accountService($q, $rootScope, $location, $timeout, accountResource, authService, ACCOUNT_CONSTANT, ENVIRONMENT_CONFIG) {

        // Define the service
        var service = {
            changePassword: changePassword,
            confirmEmail: confirmEmail,
            forgotPassword: forgotPassword,
            redirectWithDelay: redirectWithDelay,
            register: register,
            resendConfirmationEmail: resendConfirmationEmail,
            resetPassword: resetPassword,
            signin: signin
        }
        return service;

        // Change password
        function changePassword(request) {
            var deferred = $q.defer();
            accountResource.changePassword(null, request).$promise.then(
                function (response) {
                    deferred.resolve(response);
                },
                function (error) {
                    deferred.reject(error.data);
                }
            );
            return deferred.promise;
        }

        // Confirm email address
        function confirmEmail(confirmRequest) {
            var deferred = $q.defer();
            accountResource.confirmEmail(null, confirmRequest).$promise.then(
                function (response) {
                    deferred.resolve(response);
                },
                function (error) {
                    deferred.reject(error.data);
                }
            );
            return deferred.promise;
        }

        // Forgot password
        function forgotPassword(forgotPasswordRequest) {
            var deferred = $q.defer();
            forgotPasswordRequest.ReturnUrl = ENVIRONMENT_CONFIG.HOST_CLIENT + "/#" + ACCOUNT_CONSTANT.PATH_PASSWORD_RECOVERY;
            accountResource.forgotPassword(null, forgotPasswordRequest).$promise.then(
                function (response) {
                    deferred.resolve(response);
                },
                function (error) {
                    deferred.reject(error.data);
                }
            );
            return deferred.promise;
        }

        // Redirect
        function redirectWithDelay(path, delay) {
            var timer = $timeout(function () {
                $timeout.cancel(timer);
                $location.url(path);
            }, delay);
        }

        // Register a new user
        function register(registerRequest) {
            authService.signout();
            var deferred = $q.defer();
            registerRequest.ConfirmUrl = ENVIRONMENT_CONFIG.HOST_CLIENT + "/#" + ACCOUNT_CONSTANT.PATH_EMAIL_CONFIRM;
            accountResource.register(null, registerRequest).$promise.then(
                function (response) {
                    deferred.resolve(response);
                },
                function (error) {
                    deferred.reject(error.data);
                }
            );
            return deferred.promise;
        }

        // Resend confirmation email
        function resendConfirmationEmail(resendRequest) {
            var deferred = $q.defer();
            resendRequest.ConfirmUrl = ENVIRONMENT_CONFIG.HOST_CLIENT + "/#" + ACCOUNT_CONSTANT.PATH_EMAIL_CONFIRM;
            accountResource.resendConfirmationEmail(null, resendRequest).$promise.then(
                function (response) {
                    deferred.resolve(response);
                },
                function (error) {
                    deferred.reject(error.data);
                }
            );
            return deferred.promise;
        }

        // Reset password
        function resetPassword(resetPasswordRequest) {
            var deferred = $q.defer();
            accountResource.resetPassword(null, resetPasswordRequest).$promise.then(
                function (response) {
                    deferred.resolve(response);
                },
                function (error) {
                    deferred.reject(error.data);
                }
            );
            return deferred.promise;
        }

        // Signin in an existing user
        function signin(signinRequest) {
            authService.signout();
            var deferred = $q.defer();
            accountResource.signin(null, signinRequest).$promise.then(
                function (response) {
                    authService.saveAuthData(response.accessToken, signinRequest.RememberMe);
                    $rootScope.$broadcast(ACCOUNT_CONSTANT.EVENT_SIGNIN_SUCCESSFUL);
                    deferred.resolve(response);
                },
                function (error) {
                    deferred.reject(error.data);
                }
            );
            return deferred.promise;
        }
    }
})();