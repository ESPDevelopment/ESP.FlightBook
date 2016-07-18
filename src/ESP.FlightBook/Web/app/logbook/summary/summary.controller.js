(function () {
    'use strict';

    // Define summary controller
    angular
        .module('app.logbook')
        .controller('SummaryController', SummaryController);

    // Inject dependencies
    SummaryController.$inject = ['$rootScope', 'logbookService', 'LOGBOOK_CONSTANT'];

    function SummaryController($rootScope, logbookService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.activeLogbook = logbookService.activeLogbook;
        vm.aircraftSummary = {};
        vm.message = '';
        vm.hoursSummary = {};
        vm.landingsSummary = {};
        vm.sortPredicate = 'hoursTotal';
        vm.sortReverse = true;
        vm.workingAircraft = false;
        vm.workingHours = false;
        vm.workingLandings = false;

        // Available functions
        vm.activate = activate;
        vm.nullsToTop = nullsToTop;
        vm.order = order;

        // Trigger controller activation
        activate();

        // Activate the controller
        function activate() {
            loadAllData();
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_LOGBOOK_SELECTED, loadAllData);
        }

        // Loads all data
        function loadAllData() {
            getAircraftSummary();
            getHoursSummary();
            getLandingsSummary();
        }

        // Get aircraft summary information
        function getAircraftSummary() {
            vm.workingAircraft = true;
            logbookService.summaryResource.getAircraftSummary(vm.activeLogbook.logbookId)
                .then(getAircraftSummarySucceeded, getAircraftSummaryFailed);
        }

        // Handle failed get aircraft summary
        function getAircraftSummaryFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_SUMMARY_GET_ERROR;
            vm.workingAircraft = false;
        }

        // Handle successful get aircraft summary
        function getAircraftSummarySucceeded(response) {
            angular.copy(response, vm.aircraftSummary);
            vm.workingAircraft = false;
        }

        // Get hours summary
        function getHoursSummary() {
            vm.workingHours = true;
            logbookService.summaryResource.getHoursSummary(vm.activeLogbook.logbookId)
                .then(getHoursSummarySucceeded, getHoursSummaryFailed);
        }

        // Handle failed get hours summary
        function getHoursSummaryFailed(err) {
            vm.workingHours = false;
            vm.message = LOGBOOK_CONSTANT.MSG_HOURS_SUMMARY_GET_ERROR;
        }

        // Handle successful get hours summary
        function getHoursSummarySucceeded(response) {
            angular.copy(response, vm.hoursSummary);
            vm.workingHours = false;
        }

        // Get landings and approaches summary
        function getLandingsSummary() {
            vm.workingLandings = true;
            logbookService.summaryResource.getLandingsSummary(vm.activeLogbook.logbookId)
                .then(getLandingsSummarySucceeded, getLandingsSummaryFailed);
        }

        // Handle failed get landings summary
        function getLandingsSummaryFailed(err) {
            vm.workingLandings = false;
            vm.message = LOGBOOK_CONSTANT.MSG_LANDINGS_SUMMARY_GET_ERROR;
        }

        // Handle successful get landings summary
        function getLandingsSummarySucceeded(response) {
            angular.copy(response, vm.landingsSummary);
            vm.workingLandings = false;
        }

        // Helper function for sorting nulls to top
        function nullsToTop(obj) {
            var value = null;
            var index = vm.sortPredicate.indexOf('.');
            if (index >= 0) {
                var predicate = vm.sortPredicate.substr(index + 1);
                var aircraft = obj['aircraft'];
                value = aircraft = aircraft[predicate];
            } else {
                value = obj[vm.sortPredicate];
            }
            return (value == null ? -1 : 0);
        }

        // Sort function
        function order(predicate) {
            vm.sortReverse = (vm.sortPredicate === predicate) ? !vm.sortReverse : false;
            vm.sortPredicate = predicate;
        }
    }
})();

