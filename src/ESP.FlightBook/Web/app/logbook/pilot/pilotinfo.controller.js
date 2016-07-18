(function () {
    'use strict';

    // Add controller to the module
    angular
        .module('app.logbook')
        .controller('PilotInfoController', PilotInfoController);

    // Inject dependencies
    PilotInfoController.$inject = ['$rootScope', 'logbookService', 'LOGBOOK_CONSTANT'];

    // Define the controller
    function PilotInfoController($rootScope, logbookService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.activeLogbook = logbookService.activeLogbook;
        vm.message = '';
        vm.pilot = {};
        vm.showAddButton = true;
        vm.showUpdateButton = false;
        vm.tempPilot = {};
        vm.working = false;

        // Available methods
        vm.addPilot = addPilot;
        vm.getAddressLine = getAddressLine;
        vm.getCityStateZip = getCityStateZip;
        vm.getPilot = getPilot;
        vm.initUpdateModal = initUpdateModal;
        vm.updatePilot = updatePilot;

        // Activate the controller
        activate();

        // Define controller activation
        function activate() {
            vm.getPilot();
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_LOGBOOK_SELECTED, getPilot);
        }

        // Add pilot information
        function addPilot() {
            vm.working = true;
            logbookService.pilotsResource.addPilot(vm.activeLogbook.logbookId, vm.tempPilot)
                .then(addPilotSucceeded, addPilotFailed);
        }

        // Handle failed add pilot
        function addPilotFailed(err) {
            vm.working = false;
            vm.showAddButton = true;
            vm.showUpdateButton = false;
            vm.message = LOGBOOK_CONSTANT.MSG_PILOT_UPDATE_ERROR;
        }

        // Handle successful add pilot
        function addPilotSucceeded(response) {
            vm.getPilot();
            vm.pilotForm.$setPristine();
            vm.pilotForm.$setUntouched();
            vm.showAddButton = false;
            vm.showUpdateButton = true;
            vm.working = false;
            vm.message = LOGBOOK_CONSTANT.MSG_PILOT_UPDATED;
        }

        // Constructs formatted address line
        function getAddressLine() {
            var newData = "";
            if (vm.pilot != undefined && vm.pilot != null) {
                if (vm.pilot.addressLine1 != null && vm.pilot.addressLine1.length > 0) newData = newData + vm.pilot.addressLine1;
                if (newData.length > 0 && vm.pilot.addressLine2 != null && vm.pilot.addressLine2.length > 0) newData = newData + "<br />";
                if (vm.pilot.addressLine2 != null && vm.pilot.addressLine2.length > 0) newData = newData + vm.pilot.addressLine2;
            }
            return newData;
        }

        // Constructs formatted city, state, postal code line
        function getCityStateZip() {
            var newData = "";
            if (vm.pilot != undefined && vm.pilot != null) {
                if (vm.pilot.city != null && vm.pilot.city.length > 0) newData = newData + vm.pilot.city;
                if (newData.length > 0 && vm.pilot.stateOrProvince != null && vm.pilot.stateOrProvince.length > 0) newData = newData + ",&nbsp;";
                if (vm.pilot.stateOrProvince != null && vm.pilot.stateOrProvince.length > 0) newData = newData + vm.pilot.stateOrProvince;
                if (newData.length > 0 && vm.pilot.postalCode != null && vm.pilot.postalCode.length > 0) newData = newData + "&nbsp;";
                if (vm.pilot.postalCode != null && vm.pilot.postalCode.length > 0) newData = newData + vm.pilot.postalCode;
            }
            return newData;
        }

        // Loads pilot information
        function getPilot() {
            vm.working = true;
            vm.pilot = {};
            vm.tempPilot = {};
            logbookService.pilotsResource.getPilot(vm.activeLogbook.logbookId)
                .then(getPilotSucceeded, getPilotFailed);
        }

        // Handle failed get pilot
        function getPilotFailed(error) {
            vm.showAddButton = true;
            vm.showUpdateButton = false;
            vm.working = false;
            vm.message = LOGBOOK_CONSTANT.MSG_PILOT_GET_ERROR;
        }

        // Handle successful get pilot
        function getPilotSucceeded(response) {
            angular.copy(response, vm.pilot);
            angular.copy(response, vm.tempPilot);
            vm.showAddButton = false;
            vm.showUpdateButton = true;
            vm.working = false;
        }

        // Initializes update modal window
        function initUpdateModal() {
            if (vm.pilot == null || angular.isUndefined(vm.pilot.pilotId)) {
                vm.tempPilot = {};
                vm.showAddButton = true;
                vm.showUpdateButton = false;
            } else {
                angular.copy(vm.pilot, vm.tempPilot);
                vm.showAddButton = false;
                vm.showUpdateButton = true;
            }
            vm.pilotForm.$setPristine();
            vm.pilotForm.$setUntouched();
            vm.message = '';
        }

        // Update pilot information
        function updatePilot() {
            vm.working = true;
            logbookService.pilotsResource.updatePilot(vm.tempPilot.logbookId, vm.tempPilot.pilotId, vm.tempPilot)
                .then(updatePilotSucceeded, updatePilotFailed);
        }

        // Handle failed update pilot
        function updatePilotFailed(err) {
            vm.working = false;
            vm.message = LOGBOOK_CONSTANT.MSG_PILOT_UPDATE_ERROR;
        }

        // Handle successful update pilot
        function updatePilotSucceeded(response) {
            vm.getPilot();
            vm.pilotForm.$setPristine();
            vm.pilotForm.$setUntouched();
            vm.working = false;
            vm.message = LOGBOOK_CONSTANT.MSG_PILOT_UPDATED;
        }
    }
})();

