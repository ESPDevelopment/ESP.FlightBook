(function () {
    'use strict';

    // Add the resource to the module
    angular
        .module('app.account')
        .factory('accountResource', accountResource);

    // Inject dependencies
    accountResource.$inject = ['$resource', 'ENVIRONMENT_CONFIG'];

    // Define the resource
    function accountResource($resource, ENVIRONMENT_CONFIG) {
        return $resource(ENVIRONMENT_CONFIG.SIGNIN_URI, null,
            {
                'changePassword': { method: 'POST', url: ENVIRONMENT_CONFIG.CHANGE_PASSWORD_URI, headers: { 'Content-Type': 'application/json; charset=utf-8' } },
                'confirmEmail': { method: 'POST', url: ENVIRONMENT_CONFIG.CONFIRM_EMAIL_URI },
                'forgotPassword': { method: 'POST', url: ENVIRONMENT_CONFIG.FORGOT_PASSWORD_URI },
                'resendConfirmationEmail': { method: 'POST', url: ENVIRONMENT_CONFIG.RESEND_CONFIRMATION_EMAIL_URI },
                'register': { method: 'POST', url: ENVIRONMENT_CONFIG.REGISTER_URI },
                'resetPassword': { method: 'POST', url: ENVIRONMENT_CONFIG.RESET_PASSWORD_URI },
                'signin': { method: 'POST', url: ENVIRONMENT_CONFIG.SIGNIN_URI }
            });
    }
})();