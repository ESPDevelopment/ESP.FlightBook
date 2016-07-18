(function () {
    'use strict';

    // Add the controller to the module
    angular
        .module('app.account')
        .controller('AccountController', AccountController);

    // Inject dependencies
    AccountController.$inject = ['$location', 'authService', 'accountService', 'ACCOUNT_CONSTANT'];

    // Define the controller
    function AccountController($location, authService, accountService, ACCOUNT_CONSTANT) {

        var vm = this;

        // Attributes
        vm.changePasswordRequest = {
            OldPassword: '',
            NewPassword: '',
            ConfirmPassword: ''
        };
        vm.errors = [];
        vm.forgotPasswordMessage = '';
        vm.forgotPasswordRequest = {
            EmailAddress: '',
            ReturnUrl: '',
        };
        vm.message = '';
        vm.registerRequest = {
            EmailAddress: '',
            Password: '',
            ConfirmPassword: '',
            ConfirmUrl: '',
            RedirectUrl: ''
        };
        vm.resetPasswordRequest = {
            EmailAddress: '',
            Password: '',
            ConfirmPassword: '',
            Code: ''
        };
        vm.signinRequest = {
            EmailAddress: '',
            Password: '',
            RememberMe: false
        };
        vm.success = true;
        vm.working = false;

        // Functions
        vm.changePassword = changePassword;
        vm.forgotPassword = forgotPassword;
        vm.isActiveTab = isActiveTab;
        vm.register = register;
        vm.resetPassword = resetPassword;
        vm.signin = signin;

        // Activate controller
        activate();

        // Define controller startup actions
        function activate() {
        }

        // Change password
        function changePassword() {
            vm.working = true;
            vm.message = '';
            var promise = accountService.changePassword(vm.changePasswordRequest);
            promise.then(changePasswordSucceeded, changePasswordFailed);
        }

        // Process successful change password request
        function changePasswordSucceeded(response) {
            vm.success = response.succeeded;
            vm.working = false;
            if (vm.success) {
                vm.message = ACCOUNT_CONSTANT.CHANGE_PASSWORD_SUCCESS_MESSAGE;
                authService.signout();
                accountService.redirectWithDelay(ACCOUNT_CONSTANT.PATH_SIGNIN, 3000);
            } else {
                vm.message = response.message;
                vm.errors = response.errors;
            }
        }

        // Process failed change password request
        function changePasswordFailed(error) {
            vm.working = false;
            vm.success = false;
            vm.message = (error != null && angular.isDefined(error.message)) ? error.message : ACCOUNT_CONSTANT.UNEXPECTED_ERROR_MESSAGE;
            vm.errors = (error != null && angular.isDefined(error.errors)) ? error.errors : [];
        }

        // Forgot password
        function forgotPassword() {
            vm.working = true;
            vm.forgotPasswordMessage = '';
            var promise = accountService.forgotPassword(vm.forgotPasswordRequest);
            promise.then(forgotPasswordSucceeded, forgotPasswordFailed);
        }

        // Process successful forgot password request
        function forgotPasswordSucceeded(response) {
            vm.success = response.succeeded;
            vm.working = false;
            if (vm.success) {
                vm.forgotPasswordMessage = ACCOUNT_CONSTANT.FORGOT_PASSWORD_SUCCESS_MESSAGE;
                accountService.redirectWithDelay(ACCOUNT_CONSTANT.PATH_SIGNIN, 3000);
            } else {
                vm.forgotPasswordMessage = response.message;
                vm.errors = response.errors;
            }
        }

        // Process failed forgot password request
        function forgotPasswordFailed(error) {
            vm.working = false;
            vm.success = false;
            vm.forgotPasswordMessage = (error != null && angular.isDefined(error.message)) ? error.message : ACCOUNT_CONSTANT.UNEXPECTED_ERROR_MESSAGE;
            vm.errors = (error != null && angular.isDefined(error.errors)) ? error.errors : [];
        }

        // Determines if the tab is active
        function isActiveTab(viewLocation) {
            return viewLocation == $location.path();
        }

        // Register a new account
        function register() {
            vm.working = true;
            var promise = accountService.register(vm.registerRequest);
            promise.then(registerSucceeded, registerFailed);
        }

        // Process successful registration
        function registerSucceeded(response) {
            vm.success = response.succeeded;
            vm.working = false;
            if (vm.success) {
                vm.message = ACCOUNT_CONSTANT.REGISTER_SUCCESS_MESSAGE;
                accountService.redirectWithDelay(ACCOUNT_CONSTANT.PATH_ON_REGISTRATION, 3000);
            } else {
                vm.message = response.Message;
                vm.errors = response.Errors;
            }
        }

        // Process failed registration
        function registerFailed(error) {
            vm.working = false;
            vm.success = false;
            vm.message = (error != null && angular.isDefined(error.eessage)) ? error.message : ACCOUNT_CONSTANT.UNEXPECTED_ERROR_MESSAGE;
            vm.errors = (error != null && angular.isDefined(error.errors)) ? error.errors : [];
        }

        // Reset password
        function resetPassword() {
            vm.working = true;
            vm.message = 'Resetting password...';
            var params = $location.search();
            vm.resetPasswordRequest.Code = params.code;
            var promise = accountService.resetPassword(vm.resetPasswordRequest);
            promise.then(resetPasswordSucceeded, resetPasswordFailed);
        }

        // Process successful reset password
        function resetPasswordSucceeded(response) {
            vm.success = response.succeeded;
            vm.working = false;
            if (vm.success) {
                vm.message = ACCOUNT_CONSTANT.RESET_PASSWORD_SUCCESS_MESSAGE;
                accountService.redirectWithDelay(ACCOUNT_CONSTANT.PATH_SIGNIN, 3000);
            } else {
                vm.message = response.message;
                vm.errors = response.errors;
            }
        }

        // Process failed reset password
        function resetPasswordFailed(error) {
            vm.working = false;
            vm.success = false;
            vm.message = (error != null && angular.isDefined(error.message)) ? error.message : ACCOUNT_CONSTANT.UNEXPECTED_ERROR_MESSAGE;
            vm.errors = (error != null && angular.isDefined(error.errors)) ? error.errors : [];
        }

        // Sign in an existing user
        function signin() {
            vm.working = true;
            var promise = accountService.signin(vm.signinRequest);
            promise.then(signinSucceeded, signinFailed);
        }

        // Process successful signin
        function signinSucceeded(response) {
            vm.working = false;
            vm.success = true;
            $location.path(ACCOUNT_CONSTANT.PATH_ON_SIGNIN);
        }

        // Process failed signin
        function signinFailed(error) {
            vm.working = false;
            vm.success = false;
            vm.message = (error != null && angular.isDefined(error.message)) ? error.message : ACCOUNT_CONSTANT.UNEXPECTED_ERROR_MESSAGE;
        }
    }
})();