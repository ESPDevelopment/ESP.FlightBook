(function () {
    'use strict';

    // Add controller to module
    angular
        .module('app.logbook')
        .controller('EndorsementsController', EndorsementsController);

    // Inject dependencies
    EndorsementsController.$inject = ['$rootScope', 'focusService', 'logbookService', 'utilService', 'LOGBOOK_CONSTANT'];

    // Define the controller
    function EndorsementsController($rootScope, focusService, logbookService, utilService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.activeLogbook = logbookService.activeLogbook;
        vm.constants = logbookService.constantsResource.constants;
        vm.endorsements = logbookService.endorsementsResource.endorsements;
        vm.message = '';
        vm.selectedEndorsementType = {};
        vm.showAddButton = true;
        vm.showDeleteButton = false;
        vm.showTemplate = false;
        vm.showUpdateButton = false;
        vm.tempEndorsement = {};
        vm.tempEndorsementDateAsDate = undefined;
        vm.working = false;

        // Available methods
        vm.addEndorsement = addEndorsement;
        vm.deleteEndorsement = deleteEndorsement;
        vm.initAddModal = initAddModal;
        vm.initEditModal = initEditModal;
        vm.selectTemplate = selectTemplate;
        vm.setTemplate = setTemplate;
        vm.updateEndorsement = updateEndorsement;

        // Trigger controller activation
        activate();

        // Activate the controller
        function activate() {
           queryEndorsements();
           $rootScope.$on(LOGBOOK_CONSTANT.EVENT_LOGBOOK_SELECTED, queryEndorsements);
       }

        // Add endorsement
        function addEndorsement() {
            vm.working = true;
            vm.tempEndorsement.endorsementDate = vm.tempEndorsementDateAsDate.toISOString();
            logbookService.endorsementsResource.addEndorsement(vm.activeLogbook.logbookId, vm.tempEndorsement)
                .then(addEndorsementSucceeded, addEndorsementFailed);
        }

        // Handle failed add endorsement
        function addEndorsementFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_ENDORSEMENT_ADD_ERROR;
            vm.working = false;
        }

        // Handle successful add endorsement
        function addEndorsementSucceeded(response) {
            queryEndorsements();
            vm.tempEndorsement = {};
            vm.tempEndorsementDateAsDate = utilService.getCurrentDate();
            vm.endorsementForm.$setPristine();
            vm.endorsementForm.$setUntouched();
            vm.showTemplate = false;
            vm.message = LOGBOOK_CONSTANT.MSG_ENDORSEMENT_ADDED;
            vm.working = false;
        }

        // Delete an endorsement
        function deleteEndorsement() {
            vm.working = true;
            logbookService.endorsementsResource.deleteEndorsement(vm.activeLogbook.logbookId, vm.tempEndorsement.endorsementId)
                .then(deleteEndorsementSucceeded, deleteEndorsementFailed);
        }

        // Handle failed delete endorsement
        function deleteEndorsementFailed() {
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.message = LOGBOOK_CONSTANT.MSG_ENDORSEMENT_DELETE_ERROR;
            vm.working = false;
        }

        // Handle successful delete endorsement
        function deleteEndorsementSucceeded() {
            queryEndorsements();
            vm.selectedEndorsementType = {};
            vm.tempEndorsement = {};
            vm.tempEndorsementDateAsDate = undefined;
            vm.showDeleteButton = false;
            vm.showTemplate = false;
            vm.showUpdateButton = false;
            vm.message = LOGBOOK_CONSTANT.MSG_ENDORSEMENT_DELETED;
            vm.working = false;
        }

        // Get existing endorsement
        function getEndorsement(endorsementId) {
            vm.working = true;
            logbookService.endorsementsResource.getEndorsement(vm.activeLogbook.logbookId, endorsementId)
                .then(getEndorsementSucceeded, getEndorsementFailed);
        }

        // Handle failed get endorsement
        function getEndorsementFailed(err) {
            vm.showDeleteButton = false;
            vm.showUpdateButton = false;
            vm.message = LOGBOOK_CONSTANT.MSG_ENDORSEMENT_GET_ERROR;
            vm.working = false;
        }

        // Handle successful get endorsement
        function getEndorsementSucceeded(response) {
            angular.copy(response, vm.tempEndorsement);
            vm.tempEndorsementDateAsDate = utilService.stringToDate(vm.tempEndorsement.endorsementDate);
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.working = false;
        }

        // Initializes the add modal window
        function initAddModal() {
            vm.message = '';
            vm.selectedEndorsementType = {};
            vm.showAddButton = true;
            vm.showDeleteButton = false;
            vm.showTemplate = false;
            vm.showUpdateButton = false;
            vm.endorsementForm.$setPristine();
            vm.endorsementForm.$setUntouched();
            vm.tempEndorsement = {};
            vm.tempEndorsementDateAsDate = utilService.getCurrentDate();
        }

        // Initializes the edit modal window
        function initEditModal(endorsementId) {
            vm.message = '';
            vm.selectedEndorsementType = {};
            vm.showAddButton = false;
            vm.showDeleteButton = false;
            vm.showTemplate = false;
            vm.showUpdateButton = false;
            vm.endorsementForm.$setPristine();
            vm.endorsementForm.$setUntouched();
            getEndorsement(endorsementId);
        }

        // Query endorsements
        function queryEndorsements() {
            vm.working = true;
            logbookService.endorsementsResource.queryEndorsements(vm.activeLogbook.logbookId)
                .then(queryEndorsementsSucceeded, queryEndorsementsFailed);
        }

        // Handle failed query endorsements
        function queryEndorsementsFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_ENDORSEMENT_QUERY_ERROR;
            vm.working = false;
        }

        // Handle successful query endorsements
        function queryEndorsementsSucceeded(response) {
            vm.working = false;
        }

        // Enable template selection
        function selectTemplate() {
            focusService.setFocus('inputEndorsementType');
            vm.showTemplate = true;
        }

        // Update temp endorsement with endorsement type template
        function setTemplate() {
            if (angular.isDefined(vm.selectedEndorsementType)) {
                vm.tempEndorsement.title = vm.selectedEndorsementType.label;
                vm.tempEndorsement.text = vm.selectedEndorsementType.template;
            }
            vm.showTemplate = false;
        }

        // Update endorsement
        function updateEndorsement() {
            vm.working = true;
            vm.tempEndorsement.endorsementDate = vm.tempEndorsementDateAsDate.toISOString();
            logbookService.endorsementsResource.updateEndorsement(vm.activeLogbook.logbookId, vm.tempEndorsement.endorsementId, vm.tempEndorsement)
                .then(updateEndorsementSucceeded, updateEndorsementFailed);
        }

        // Handle failed update endorsement
        function updateEndorsementFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_ENDORSEMENT_UDPATE_ERROR;
            vm.working = false;
        }

        // Handle successful update endorsement
        function updateEndorsementSucceeded(response) {
            queryEndorsements();
            vm.endorsementForm.$setPristine();
            vm.endorsementForm.$setUntouched();
            vm.showTemplate = false;
            vm.message = LOGBOOK_CONSTANT.MSG_ENDORSEMENT_UPDATED;
            vm.working = false;
        }
    }
})();

