(function () {
    'use strict';

    // Add the controller to the module
    angular
        .module('app.account')
        .controller('ConfirmController', ConfirmController);

    // Inject dependencies
    ConfirmController.$inject = ['$location', 'accountService', 'ACCOUNT_CONSTANT'];

    // Define the controller
    function ConfirmController($location, accountService, ACCOUNT_CONSTANT) {

        var vm = this;

        // Attributes
        vm.confirmationFailed = false;
        vm.confirmRequest = {
            userId: '',
            code: ''
        };
        vm.errors = [];
        vm.message = '';
        vm.resendRequest = {
            ConfirmUrl: '',
            EmailAddress: ''
        }
        vm.success = false;
        vm.working = false;

        // Functions
        vm.confirmEmail = confirmEmail;
        vm.resendConfirmationEmail = resendConfirmationEmail;

        // Activate the controller
        activate();

        // Define controller statup actions
        function activate() {
            parseParams();
            confirmEmail();
        }

        // Confirm email address
        function confirmEmail() {
            vm.working = true;
            vm.message = ACCOUNT_CONSTANT.CONFIRM_EMAIL_WORKING;
            var promise = accountService.confirmEmail(vm.confirmRequest);
            promise.then(confirmEmailSucceeded, confirmEmailFailed);
        }

        // Process successful email confirmation
        function confirmEmailSucceeded(response) {
            vm.confirmationFailed = false;
            vm.success = response.succeeded;
            vm.working = false;
            if (vm.success) {
                vm.message = ACCOUNT_CONSTANT.CONFIRM_EMAIL_SUCCESS_MESSAGE;
                accountService.redirectWithDelay(ACCOUNT_CONSTANT.PATH_SIGNIN, 3000);
            } else {
                vm.message = response.message;
                vm.errors = response.errors;
            }
        }

        // Process failed email confirmation
        function confirmEmailFailed(error) {
            vm.working = false;
            vm.success = false;
            vm.confirmationFailed = true;
            vm.message = (error != null && angular.isDefined(error.message)) ? error.message : ACCOUNT_CONSTANT.UNEXPECTED_ERROR_MESSAGE;
            vm.errors = (error != null && angular.isDefined(error.errors)) ? error.errors : [];
        }

        // Parse query parameters
        function parseParams() {
            var params = $location.search();
            vm.confirmRequest.userId = params.userId;
            vm.confirmRequest.code = params.code;
        }

        // Resend email confirmation message
        function resendConfirmationEmail() {
            vm.working = true;
            vm.message = ACCOUNT_CONSTANT.RESENDING_CONFIRMATION_EMAIL_MESSAGE;
            var promise = accountService.resendConfirmationEmail(vm.resendRequest);
            promise.then(resendConfirmationEmailSucceeded, resendConfirmationEmailFailed);
        }

        // Process successful confirmation email resend
        function resendConfirmationEmailSucceeded(response) {
            vm.confirmationFailed = false;
            vm.success = response.succeeded;
            vm.working = false;
            if (vm.success) {
                vm.message = ACCOUNT_CONSTANT.RESEND_CONFIRMATION_EMAIL_SUCCESS_MESSAGE;
                accountService.redirectWithDelay(ACCOUNT_CONSTANT.PATH_ON_REGISTRATION, 3000);
            } else {
                vm.message = response.message;
                vm.errors = response.errors;
            }
        }

        // Process failed confirmation email resend
        function resendConfirmationEmailFailed(error) {
            vm.working = false;
            vm.success = false;
            vm.confirmationFailed = true;
            vm.message = (error != null && angular.isDefined(error.message)) ? error.message : ACCOUNT_CONSTANT.UNEXPECTED_ERROR_MESSAGE;
            vm.errors = (error != null && angular.isDefined(error.errors)) ? error.errors : [];
        }
    }
})();