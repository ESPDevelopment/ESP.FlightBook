(function () {
    'use strict';

    // Define aircraft controller
    angular
        .module('app.logbook')
        .controller('AircraftController', AircraftController);

    // Inject dependencies
    AircraftController.$inject = ['$rootScope', 'logbookService', 'LOGBOOK_CONSTANT'];

    function AircraftController($rootScope, logbookService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.activeLogbook = logbookService.activeLogbook;
        vm.aircraft = logbookService.aircraftResource.aircraft;
        vm.constants = logbookService.constantsResource.constants;
        vm.disableNext = false;
        vm.disablePrev = false;
        vm.message = '';
        vm.pageCurrent = 0;
        vm.pageSize = LOGBOOK_CONSTANT.AIRCRAFT_PER_PAGE;
        vm.pageTotal = 0;
        vm.rowCount = 0;
        vm.selectedCategoryAndClass = '';
        vm.showAddButton = false;
        vm.showDeleteButton = false;
        vm.showPagination = false;
        vm.showUpdateButton = false;
        vm.sortPredicate = 'aircraftIdentifier';
        vm.sortReverse = false;
        vm.tempAircraft = {};
        vm.working = false;

        // Available functions
        vm.addAircraft = addAircraft;
        vm.deleteAircraft = deleteAircraft;
        vm.getAircraft = getAircraft;
        vm.initAddModal = initAddModal;
        vm.initUpdateModal = initUpdateModal;
        vm.nullsToTop = nullsToTop;
        vm.order = order;
        vm.setCurrentPage = setCurrentPage;
        vm.updateAircraft = updateAircraft;

        // Activate the controller
        activate();

        // Initializes the controller
        function activate() {
            queryAircraft(0);
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_LOGBOOK_SELECTED, queryAircraft);
        }

        // Add an aircraft
        function addAircraft() {
            vm.working = true;
            if (angular.isDefined(vm.selectedCategoryAndClass)) {
                vm.tempAircraft.aircraftCategory = vm.selectedCategoryAndClass.category;
                vm.tempAircraft.aircraftClass = vm.selectedCategoryAndClass.class;
            } else {
                vm.tempAircraft.aircraftCategory = "";
                vm.tempAircraft.aircraftClass = "";
            }
            logbookService.aircraftResource.addAircraft(vm.activeLogbook.logbookId, vm.tempAircraft)
                .then(addAircraftSucceeded, addAircraftFailed);
        }

        // Handle failed add aircraft
        function addAircraftFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_ADD_ERROR;
            vm.working = false;
        }

        // Handle successful add aircraft
        function addAircraftSucceeded(response) {
            queryAircraft(0);
            vm.aircraftForm.$setPristine();
            vm.aircraftForm.$setUntouched();
            vm.tempAircraft = {};
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_ADDED;
            vm.working = false;
        }

        // Delete an aircraft
        function deleteAircraft() {
            vm.working = true;
            logbookService.aircraftResource.deleteAircraft(vm.activeLogbook.logbookId, vm.tempAircraft.aircraftId)
                .then(deleteAircraftSucceeded, deleteAircraftFailed);
        }

        // Handle failed delete aircraft
        function deleteAircraftFailed(err) {
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_DELETE_ERROR;
            vm.working = false;
        }

        // Handle successful delete aircraft
        function deleteAircraftSucceeded(response) {
            queryAircraft(0);
            vm.tempAircraft = {};
            vm.aircraftForm.$setPristine();
            vm.aircraftForm.$setUntouched();
            vm.showDeleteButton = false;
            vm.showUpdateButton = false;
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_DELETED;
            vm.working = false;
        }

        // Get an aircraft
        function getAircraft(aircraftId) {
            vm.working = true;
            logbookService.aircraftResource.getAircraft(vm.activeLogbook.logbookId, aircraftId)
                .then(getAircraftSucceeded, getAircraftFailed);
        }

        // Handle failed get aircraft
        function getAircraftFailed(err) {
            vm.showAddButton = false;
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_GET_ERROR;
            vm.working = false;
        }

        // Handle successful get aircraft
        function getAircraftSucceeded(response) {
            angular.copy(response, vm.tempAircraft);
            vm.selectedCategoryAndClass = selectCategoryAndClass(vm.tempAircraft);
            vm.showAddButton = false;
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.working = false;
        }

        // Initialize the "add" modal
        function initAddModal() {
            vm.message = '';
            vm.showAddButton = true;
            vm.showDeleteButton = false;
            vm.showUpdateButton = false;
            vm.aircraftForm.$setPristine();
            vm.aircraftForm.$setUntouched();
            vm.tempAircraft = {};
            vm.selectCategoryAndClass = '';
        }

        // Initialize the "update" modal
        function initUpdateModal(aircraftId) {
            vm.message = '';
            vm.showAddButton = false;
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.aircraftForm.$setPristine();
            vm.aircraftForm.$setUntouched();
            vm.getAircraft(aircraftId);
        }

        // Helper function for sorting nulls to top
        function nullsToTop(obj) {
            var value = obj[vm.sortPredicate];
            return (value == null ? -1 : 0);
        }

        // Sort function
        function order(predicate) {
            vm.sortReverse = (vm.sortPredicate === predicate) ? !vm.sortReverse : false;
            vm.sortPredicate = predicate;
            vm.pageCurrent = 0;
        }

        // Load aircraft information
        function queryAircraft(page) {
            vm.working = true;
            vm.pageCurrent = page;
            logbookService.aircraftResource.queryAircraft(vm.activeLogbook.logbookId, 0, 0)
                .then(queryAircraftComplete, queryAircraftFailed);
        }

        // Handle successful query (GET)
        function queryAircraftComplete(response) {
            vm.rowCount = parseInt(response.headers['x-eflightbook-pagination-total']);
            vm.pageTotal = Math.ceil(vm.rowCount / vm.pageSize);
            vm.pageCurrent = 0;
            resetPagination();
            vm.working = false;
        }

        // Handle failed query (GET)
        function queryAircraftFailed(err) {
            resetPagination();
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_QUERY_ERROR;
            vm.working = false;
        }

        // Reset pagination
        function resetPagination() {
            vm.showPagination = (vm.pageTotal > 1);
            vm.disablePrev = (vm.pageCurrent == 0);
            vm.disableNext = (vm.pageCurrent == vm.pageTotal - 1);
        }

        // Select appropriate category and class
        function selectCategoryAndClass(aircraft) {
            var selectedItem = "";
            if (angular.isDefined(aircraft)) {
                angular.forEach(vm.constants.categoriesAndClasses, function (row) {
                    if (row.category == aircraft.aircraftCategory && row.class == aircraft.aircraftClass) {
                        selectedItem = row;
                    }
                });
            }
            return selectedItem;
        }

        // Set current page
        function setCurrentPage(page) {
            vm.pageCurrent = page;
            resetPagination();
        }

        // Update aircraft
        function updateAircraft() {
            vm.working = true;
            if (angular.isDefined(vm.selectedCategoryAndClass)) {
                vm.tempAircraft.aircraftCategory = vm.selectedCategoryAndClass.category;
                vm.tempAircraft.aircraftClass = vm.selectedCategoryAndClass.class;
            } else {
                vm.tempAircraft.aircraftCategory = "";
                vm.tempAircraft.aircraftClass = "";
            }
            logbookService.aircraftResource.updateAircraft(vm.activeLogbook.logbookId, vm.tempAircraft.aircraftId, vm.tempAircraft)
                .then(updateAircraftSucceeded, updateAircraftFailed);
        }

        // Handle failed update (PUT)
        function updateAircraftFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_UPDATE_ERROR;
            vm.working = false;
        }

        // Handle successful update
        function updateAircraftSucceeded(response) {
            queryAircraft(0);
            vm.aircraftForm.$setPristine();
            vm.aircraftForm.$setUntouched();
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_UPDATED;
            vm.working = false;
        }
    }
})();

