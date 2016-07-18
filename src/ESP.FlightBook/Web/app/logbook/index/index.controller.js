(function () {
    'use strict';

    // Define index controller
    angular
        .module('app.logbook')
        .controller('IndexController', IndexController);

    // Inject dependencies
    IndexController.$inject = ['$location', '$rootScope', 'authService', 'logbookService', 'LOGBOOK_CONSTANT'];

    function IndexController($location, $rootScope, authService, logbookService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.activeLogbook = logbookService.activeLogbook;
        vm.logbooks = logbookService.logbooksResource.logbooks;
        vm.message = '';

        // Available functions
        vm.activate = activate;
        vm.closeSlideBar = closeSlideBar;
        vm.isActivePage = isActivePage;
        vm.isLoggedIn = isLoggedIn;
        vm.setActiveLogbook = setActiveLogbook;
        vm.signout = signout;
        vm.toggleSlideBar = toggleSlideBar;

        // Trigger controller activation
        activate();

        // Activate the controller
        function activate() {
            queryLogbooks();
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_LOGBOOKS_UPDATED, reloadLogbooks);
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_SIGNIN_SUCCESSFUL, reloadLogbooks);
        }

        // Close slidebar
        function closeSlideBar() {
            if (angular.isDefined(mySlidebars)) {
                mySlidebars.close();
            }
        }

        // Determines if the specified page is active
        function isActivePage(page) {
            return (page === $location.path());
        }

        // Determine if a user is signed in
        function isLoggedIn() {
            return (authService.isLoggedIn());
        }

        // Query logbooks
        function queryLogbooks() {
            vm.working = true;
            logbookService.logbooksResource.queryLogbooks()
                .then(queryLogbooksSucceeded, queryLogbooksFailed);
        }

        // Handle failed query (GET)
        function queryLogbooksFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_LOGBOOK_QUERY_ERROR;
            vm.working = false;
        }

        // Handle successful query logbooks
        function queryLogbooksSucceeded(response) {
            vm.working = false;
        }

        // Reload logbooks
        function reloadLogbooks() {
            logbookService.flushCache();
            queryLogbooks();
        }

        // Set active logbook
        function setActiveLogbook(title, logbookId) {
            logbookService.setActiveLogbook(title, logbookId);
        }

        // Sign out current user
        function signout() {
            authService.signout();
            logbookService.flushCache();
            logbookService.clearPreferences();
            closeSlideBar();
            $location.path(LOGBOOK_CONSTANT.PATH_DEFAULT_ANONYMOUS);
        }

        // Toggle slidebar
        function toggleSlideBar(side) {
            if (angular.isDefined(mySlidebars)) {
                mySlidebars.toggle(side);
            }
        }
    }
})();