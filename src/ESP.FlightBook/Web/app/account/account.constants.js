(function () {
    'use strict';

    // Add constants to the module
    angular
        .module('app.account')

        // Define constants for the module
        .constant('ACCOUNT_CONSTANT', {
            CHANGE_PASSWORD_SUCCESS_MESSAGE: 'Password change successful. You will now be redirected to signin...',
            CONFIRM_EMAIL_SUCCESS_MESSAGE: 'Email confirmation successful. You will now be redirected to signin...',
            CONFIRM_EMAIL_FAILED_MESSAGE: 'Unable to confirm your email address',
            CONFIRM_EMAIL_WORKING: 'Confirming your email address',
            FORGOT_PASSWORD_SUCCESS_MESSAGE: 'Password reset instructions have been sent to your email address.',
            EVENT_SIGNIN_SUCCESSFUL: 'event:account-signinSuccessful',
            EVENT_SIGNOUT_SUCCESSFUL: 'event:account-signoutSuccessful',
            PATH_PASSWORD_RECOVERY: '/account/resetPassword',
            PATH_EMAIL_CONFIRM: '/account/confirm',
            PATH_ON_REGISTRATION: '/account/pending',
            PATH_ON_SIGNIN: '/logbooks',
            PATH_SIGNIN: '/signin',
            REGISTER_SUCCESS_MESSAGE: 'Registration successful.  You will now be redirected...',
            RESEND_CONFIRMATION_EMAIL_SUCCESS_MESSAGE: 'A new email confirmation message has been sent.',
            RESENDING_CONFIRMATION_EMAIL_MESSAGE: 'Resending email confirmation message...',
            RESET_PASSWORD_SUCCESS_MESSAGE: 'Password reset successful. You will now be redirected to signin...',
            UNEXPECTED_ERROR_MESSAGE: 'An unexpected error occurred. Please try again later.'
        });
})();