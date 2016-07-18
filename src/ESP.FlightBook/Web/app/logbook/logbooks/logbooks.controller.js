(function () {
    'use strict';

    // Add controller to the module
    angular
        .module('app.logbook')
        .controller('LogbooksController', LogbooksController);

    // Inject dependencies
    LogbooksController.$inject = ['$rootScope', '$location', 'logbookService', 'authService', 'utilService', 'LOGBOOK_CONSTANT'];

    // Define the controller
    function LogbooksController($rootScope, $location, logbookService, authService, utilService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.action = { type: 'none', title: '' };
        vm.logbooks = logbookService.logbooksResource.logbooks;
        vm.message = '';
        vm.showAddButton = false;
        vm.showDeleteButton = false;
        vm.showUpdateButton = false;
        vm.tempLogbook = {};
        vm.working = false;

        // Available functions
        vm.addLogbook = addLogbook;
        vm.deleteLogbook = deleteLogbook;
        vm.getLogbook = getLogbook;
        vm.initAddModal = initAddModal;
        vm.initUpdateModal = initUpdateModal;
        vm.setActiveLogbook = setActiveLogbook;
        vm.updateLogbook = updateLogbook;

        // Activate the controller
        activate();

        // Controller activation
        function activate() {
            queryLogbooks();
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_SIGNIN_SUCCESSFUL, reloadLogbooks);
        }

        // Add a logbook
        function addLogbook() {
            vm.working = true;
            logbookService.logbooksResource.addLogbook(vm.tempLogbook)
                .then(addLogbookSucceeded, addLogbookFailed);
        }

        // Handle failed add
        function addLogbookFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_LOGBOOK_ADD_ERROR;
            vm.working = false;
        }

        // Handle successful add
        function addLogbookSucceeded() {
            queryLogbooks();
            vm.action.type = 'add';
            vm.action.title = vm.tempLogbook.title;
            vm.tempLogbook = {};
            vm.logbookForm.$setPristine();
            vm.logbookForm.$setUntouched();
            vm.message = LOGBOOK_CONSTANT.MSG_LOGBOOK_ADDED;
            vm.working = false;
        }

        // Delete a logbook
        function deleteLogbook() {
            vm.working = true;
            logbookService.logbooksResource.deleteLogbook(vm.tempLogbook.logbookId)
                .then(deleteLogbookSucceeded, deleteLogbookFailed);
        }

        // Handle failed delete
        function deleteLogbookFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_LOGBOOK_DELETE_ERROR;
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.working = false;
        }

        // Handle successful delete
        function deleteLogbookSucceeded() {
            queryLogbooks();
            vm.action.type = 'delete';
            vm.action.title = vm.tempLogbook.title;
            vm.tempLogbook = {};
            vm.showDeleteButton = false;
            vm.showUpdateButton = false;
            vm.message = LOGBOOK_CONSTANT.MSG_LOGBOOK_DELETED;
            vm.working = false;
        }

        // Get a logbook
        function getLogbook(logbookId) {
            vm.working = true;
            logbookService.logbooksResource.getLogbook(logbookId)
                .then(getLogbookSucceeded, getLogbookFailed);
        }

        // Handle failed get
        function getLogbookFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_LOGBOOK_GET_ERROR;
            vm.showDeleteButton = false;
            vm.showUpdateButton = false;
            vm.working = false;
        }

        // Handle successful get
        function getLogbookSucceeded(response) {
            angular.copy(response, vm.tempLogbook);
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.working = false;
        }

        // Initialize add modal
        function initAddModal() {
            vm.message = '';
            vm.showAddButton = true;
            vm.showDeleteButton = false;
            vm.showTemplate = false;
            vm.showUpdateButton = false;
            vm.logbookForm.$setPristine();
            vm.logbookForm.$setUntouched();
            vm.tempLogbook = {};
        }

        // Function init update modal
        function initUpdateModal(logbookId) {
            vm.message = '';
            vm.showAddButton = false;
            vm.showDeleteButton = false;
            vm.showTemplate = false;
            vm.showUpdateButton = false;
            vm.logbookForm.$setPristine();
            vm.logbookForm.$setUntouched();
            vm.getLogbook(logbookId);
        }

        // Query logbooks
        function queryLogbooks() {
            vm.working = true;
            logbookService.logbooksResource.queryLogbooks()
                .then(queryLogbooksSucceeded, queryLogbooksFailed);
        }

        // Handle failed query
        function queryLogbooksFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_LOGBOOK_QUERY_ERROR;
            vm.working = false;
        }

        // Handle successful query
        function queryLogbooksSucceeded() {
            resetActiveLogbook();
            vm.working = false;
        }

        // Reload logbooks
        function reloadLogbooks() {
            logbookService.flushCache();
            queryLogbooks();
        }

        // Resets active logbook after a change
        function resetActiveLogbook() {
            switch (vm.action.type) {
                case 'add':
                case 'update':
                    var logbook = utilService.getElementByProperty('Title', vm.action.title, vm.logbooks);
                    if (logbook != null) {
                        logbookService.setActiveLogbook(logbook.title, logbook.logbookId);
                    }
                    break;
                case 'delete':
                    if (vm.action.title == logbookService.activeLogbook.title) {
                        logbookService.setActiveLogbook('(Select Logbook)', 0);
                    }
                    break;
            }
            vm.action.type = 'none';
            vm.action.title = '';
        }

        // Sets the active logbook
        function setActiveLogbook(title, logbookId) {
            logbookService.setActiveLogbook(title, logbookId);
            $location.path(LOGBOOK_CONSTANT.PATH_DEFAULT_WHEN_SELECTED);
        }

        // Update a logbook
        function updateLogbook() {
            vm.working = true;
            logbookService.logbooksResource.updateLogbook(vm.tempLogbook.logbookId, vm.tempLogbook)
                .then(updateLogbookSucceeded, updateLogbookFailed);
        }

        // Handle failed update
        function updateLogbookFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_LOGBOOK_UDPATE_ERROR;
            vm.working = false;
        }

        // Handle successful update
        function updateLogbookSucceeded() {
            queryLogbooks();
            vm.action.type = 'update';
            vm.action.title = vm.tempLogbook.title;
            vm.logbookForm.$setPristine();
            vm.logbookForm.$setUntouched();
            vm.showTemplate = false;
            vm.message = LOGBOOK_CONSTANT.MSG_LOGBOOK_UPDATED;
            vm.working = false;
        }
    }
})();