(function () {
    'use strict';

    // Add controller to the module
    angular
        .module('app.logbook')
        .controller('FlightsController', FlightsController);

    // Inject dependencies
    FlightsController.$inject = ['$rootScope', 'logbookService', 'utilService', 'LOGBOOK_CONSTANT'];

    // Define the controller
    function FlightsController($rootScope, logbookService, utilService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.activeLogbook = logbookService.activeLogbook;
        vm.aircraft = logbookService.aircraftResource.aircraft;
        vm.approachMessage = '';
        vm.constants = logbookService.constantsResource.constants;
        vm.disableNext = false;
        vm.disablePrev = false;
        vm.filter = {
            active: false,
            flightDateStart: '',
            flightDateEnd: '',
            aircraftIdentifier: '',
            aircraftType: '',
            isComplex: false,
            isRetractable: false
        };
        vm.filterPanelTitle = 'Filter';
        vm.flights = logbookService.flightsResource.flights;
        vm.message = '';
        vm.pages = [];
        vm.pageCurrent = 0;
        vm.pageTotal = 0;
        vm.showAddApproachButton = false;
        vm.showAddFlightButton = false;
        vm.showDeleteApproachButton = false;
        vm.showDeleteFlightButton = false;
        vm.showPagination = false;
        vm.showUpdateApproachButton = false;
        vm.showUpdateFlightButton = false;
        vm.sortPredicate = 'flightDate';
        vm.sortReverse = true;
        vm.tempFlight = {};
        vm.tempApproach = {};
        vm.working = false;

        // Available functions
        vm.addApproach = addApproach;
        vm.addFlight = addFlight;
        vm.applyFilter = applyFilter;
        vm.clearFilter = clearFilter;
        vm.deleteApproach = deleteApproach;
        vm.deleteFlight = deleteFlight;
        vm.getRoute = getRoute;
        vm.initAddApproachModal = initAddApproachModal;
        vm.initAddFlightModal = initAddFlightModal;
        vm.initUpdateApproachModal = initUpdateApproachModal;
        vm.initUpdateFlightModal = initUpdateFlightModal;
        vm.queryFlights = queryFlights;
        vm.updateApproach = updateApproach;
        vm.updateFlight = updateFlight;

        // Trigger controller activation
        activate();

        // Activate the controller
        function activate() {
            loadAllData();
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_LOGBOOK_SELECTED, loadAllData);
            $('#approachModal').on('hidden.bs.modal', function (e) {
                $(document.body).addClass('modal-open');
            })
        }

        // Add a new approach
        function addApproach() {
            if (angular.isUndefined(vm.tempFlight.flightId) || vm.tempFlight.flightId <= 0) {
                vm.tempApproach.approachId = getRandomId();
                vm.tempFlight.approaches.push(vm.tempApproach);
                addApproachSucceeded();
            } else {
                vm.working = true;
                logbookService.approachesResource.addApproach(vm.activeLogbook.logbookId, vm.tempFlight.flightId, vm.tempApproach)
                    .then(addApproachSucceeded, addApproachFailed);
            }
        }

        // Handle failed add approach
        function addApproachFailed(err) {
            vm.approachMessage = LOGBOOK_CONSTANT.MSG_APPROACH_ADD_ERROR;
            vm.working = false;
        }

        // Handle successful add approach
        function addApproachSucceeded(response) {
            getFlight(vm.tempFlight.flightId);
            vm.tempApproach = {};
            vm.approachForm.$setPristine();
            vm.approachForm.$setUntouched();
            vm.approachMessage = LOGBOOK_CONSTANT.MSG_APPROACH_ADDED;
            vm.working = false;
        }

        // Add a new flight
        function addFlight() {
            vm.working = true;
            vm.tempFlight.flightDate = vm.tempFlightDateAsDate.toISOString();
            logbookService.flightsResource.addFlight(vm.activeLogbook.logbookId, vm.tempFlight)
                .then(addFlightSucceeded, addFlightFailed);
        }

        // Handle failed add (POST) to flights resource
        function addFlightFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_FLIGHT_ADD_ERROR;
            vm.working = false;
        }

        // Handle successful add flight
        function addFlightSucceeded(response) {
            queryFlights();
            resetFlightData();
            vm.flightForm.$setPristine();
            vm.flightForm.$setUntouched();
            vm.message = LOGBOOK_CONSTANT.MSG_FLIGHT_ADDED;
            vm.working = false;
        }

        // Apply filter
        function applyFilter() {
            vm.filter.active = true;
            vm.filterPanelTitle = 'Filter (applied)';
            queryFlights(0);
        }

        // Clear filter
        function clearFilter() {
            vm.filterPanelTitle = 'Filter';
            vm.filter.active = false;
            vm.filter.flightDateStart = '';
            vm.filter.flightDateEnd = '';
            vm.filter.aircraftIdentifier = '';
            vm.filter.aircraftType = '';
            queryFlights(0);
        }

        // Delete existing approach
        function deleteApproach() {
            if (angular.isUndefined(vm.tempFlight.flightId) || vm.tempFlight.flightId < 0) {
                var index = utilService.getIndexByProperty('approachId', vm.tempApproach.approachId, vm.tempFlight.approaches);
                vm.tempFlight.approaches.splice(index, 1);
                deleteApproachSucceeded();
            } else {
                vm.working = true;
                logbookService.approachesResource.deleteApproach(vm.activeLogbook.logbookId, vm.tempFlight.flightId, vm.tempApproach.approachId)
                    .then(deleteApproachSucceeded, deleteApproachFailed);
            }
        }

        // Handle failed delete (DELETE) to approaches resource
        function deleteApproachFailed(err) {
            vm.approachMessage = LOGBOOK_CONSTANT.MSG_APPROACH_DELETE_ERROR;
            vm.working = false;
        }

        // Handle successful delete approach
        function deleteApproachSucceeded(response) {
            getFlight(vm.tempFlight.flightId);
            vm.tempApproach = {};
            vm.showAddApproachButton = false;
            vm.showDeleteApproachButton = false;
            vm.showUpdateApproachButton = false;
            vm.approachForm.$setPristine();
            vm.approachForm.$setUntouched();
            vm.approachMessage = LOGBOOK_CONSTANT.MSG_APPROACH_DELETED;
            vm.working = false;
        }

        // Delete existing flight
        function deleteFlight() {
            vm.working = true;
            logbookService.flightsResource.deleteFlight(vm.activeLogbook.logbookId, vm.tempFlight.flightId)
                .then(deleteFlightSucceeded, deleteFlightFailed);
        }

        // Handle failed delete flight
        function deleteFlightFailed(error) {
            vm.showAddFlightButton = false;
            vm.showDeleteFlightButton = true;
            vm.showUpdateFlightButton = true;
            vm.message = LOGBOOK_CONSTANT.MSG_CURRENCY_DELETE_ERROR;
            vm.working = false;
        }

        // Handle successful delete flight
        function deleteFlightSucceeded(response) {
            queryFlights();
            vm.showAddButton = false;
            vm.showDeleteButton = false;
            vm.showUpdateButton = false;
            vm.message = LOGBOOK_CONSTANT.MSG_FLIGHT_DELETED;
            vm.working = false;
        }

        // Get existing flight
        function getFlight(flightId) {
            if (angular.isDefined(flightId)) {
                vm.working = true;
                logbookService.flightsResource.getFlight(vm.activeLogbook.logbookId, flightId)
                    .then(getFlightSucceeded, getFlightFailed);
            }
        }

        // Handle failed get flight
        function getFlightFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_FLIGHT_GET_ERROR;
            vm.working = false;
        }

        // Handle successful get flight
        function getFlightSucceeded(response) {
            angular.copy(response, vm.tempFlight);
            vm.tempFlightDateAsDate = utilService.stringToDate(vm.tempFlight.flightDate);
            vm.showDeleteFlightButton = true;
            vm.showUpdateFlightButton = true;
            vm.working = false;
        }

        // Return a formatted route string
        function getRoute(flight) {
            var route = "";
            if (flight != undefined && flight != null) {
                if (flight.departureCode != null && flight.departureCode.length > 0) route = route + flight.departureCode;
                if (route.length > 0 && flight.route != null && flight.route.length > 0) route = route + "&nbsp;/&nbsp;";
                if (flight.route != null && flight.route.length > 0) route = route + flight.route;
                if (route.length > 0 && flight.destinationCode != null && flight.destinationCode.length > 0) route = route + "&nbsp;/&nbsp;";
                if (flight.destinationCode != null && flight.destinationCode.length > 0) route = route + flight.destinationCode;
            }
            return route;
        }

        // Return a random identifier
        function getRandomId() {
            return Math.floor((Math.random() * 100000) + 1);
        }

        // Initialize the add approach modal
        function initAddApproachModal() {
            vm.approachMessage = '';
            vm.tempApproach = {};
            vm.showAddApproachButton = true;
            vm.showDeleteApproachButton = false;
            vm.showUpdateApproachButton = false;
            vm.approachForm.$setPristine();
            vm.approachForm.$setUntouched();
        }

        // Initialize the add modal
        function initAddFlightModal() {
            vm.message = '';
            vm.showAddFlightButton = true;
            vm.showDeleteFlightButton = false;
            vm.showUpdateFlightButton = false;
            vm.flightForm.$setPristine();
            vm.flightForm.$setUntouched();
            resetFlightData();
        }

        // Initialize the update approach modal
        function initUpdateApproachModal(flightId, approachId) {
            var approach = utilService.getElementByProperty('approachId', approachId, vm.tempFlight.approaches);
            if (angular.isDefined(approach)) {
                vm.approachMessage = '';
                angular.copy(approach, vm.tempApproach);
                vm.showAddApproachButton = false;
                vm.showDeleteApproachButton = true;
                vm.showUpdateApproachButton = true;
            } else {
                vm.approachMessage = LOGBOOK_CONSTANT.MSG_APPROACH_GET_ERROR;
                vm.tempApproach = { approachId: getRandomId() };
                vm.showAddApproachButton = false;
                vm.showDeleteApproachButton = false;
                vm.showUpdateApproachButton = false;
            }
            vm.approachForm.$setPristine();
            vm.approachForm.$setUntouched();
        }

        // Initialize the update modal
        function initUpdateFlightModal(flightId) {
            vm.message = '';
            vm.showAddFlightButton = false;
            vm.showDeleteFlightButton = false;
            vm.showUpdateFlightButton = false;
            vm.flightForm.$setPristine();
            vm.flightForm.$setUntouched();
            getFlight(flightId);
        }

        // Load all data
        function loadAllData() {
            queryAircraft();
            queryFlights(0);
        }

        // Sort function
        function order(predicate) {
            vm.sortReverse = (vm.sortPredicate === predicate) ? !vm.sortReverse : false;
            vm.sortPredicate = predicate;
        }

        // Get all aircraft
        function queryAircraft() {
            vm.working = true;
            logbookService.aircraftResource.queryAircraft(vm.activeLogbook.logbookId)
                .then(queryAircraftSucceeded, queryAircraftFailed);
        }

        // Handle failed query aircraft
        function queryAircraftFailed(error) {
            vm.working = false;
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_QUERY_ERROR;
        }

        // Handle successful query aircraft
        function queryAircraftSucceeded(response) {
            vm.working = false;
        }

        // Get all flights
        function queryFlights(page) {
            vm.working = true;
            vm.pageCurrent = page;
            logbookService.flightsResource.queryFlights(vm.activeLogbook.logbookId, vm.pageCurrent, LOGBOOK_CONSTANT.FLIGHTS_PER_PAGE, vm.filter)
                .then(queryFlightsSucceeded, queryFlightsFailed);
        }

        // Handle failed query flights
        function queryFlightsFailed(err) {
            resetPagination();
            vm.message = LOGBOOK_CONSTANT.MSG_FLIGHT_QUERY_ERROR;
            vm.working = false;
        }

        // Handle successful query flights
        function queryFlightsSucceeded(response) {
            vm.pageCurrent = parseInt(response.headers['x-eflightbook-pagination-page']);
            vm.pageTotal = parseInt(response.headers['x-eflightbook-pagination-totalpages']);
            resetPagination();
            vm.working = false;
        }

        // Reset flight data
        function resetFlightData() {
            vm.tempFlight = {};
            vm.tempFlightDateAsDate = utilService.getCurrentDate();
            vm.tempFlight.approaches = [];
            vm.tempFlight.flightTimeActualInstrument = 0.0;
            vm.tempFlight.flightTimeCrossCountry = 0.0;
            vm.tempFlight.flightTimeDay = 0.0;
            vm.tempFlight.flightTimeDual = 0.0;
            vm.tempFlight.flightTimeNight = 0.0;
            vm.tempFlight.flightTimePIC = 0.0;
            vm.tempFlight.flightTimeSimulatedInstrument = 0.0;
            vm.tempFlight.flightTimeSolo = 0.0;
            vm.tempFlight.flightTimeTotal = 0.0;
            vm.tempFlight.numberOfLandingsDay = 0;
            vm.tempFlight.numberOfLandingsNight = 0;
            vm.tempFlight.numberOfHolds = 0;
        }

        // Reset pagination
        function resetPagination() {
            vm.showPagination = (vm.pageTotal > 1);
            vm.disablePrev = (vm.pageCurrent == 0);
            vm.disableNext = (vm.pageCurrent == vm.pageTotal - 1);
        }

        // Update an existing approach
        function updateApproach() {
            if (angular.isUndefined(vm.tempFlight.flightId) || vm.tempFlight.flightId <= 0) {
                var index = utilService.getIndexByProperty('approachId', vm.tempApproach.approachId, vm.tempFlight.approaches);
                angular.copy(vm.tempApproach, vm.tempFlight.approaches[index]);
                updateApproachSucceeded();
            } else {
                vm.working = true;
                logbookService.approachesResource.updateApproach(vm.activeLogbook.logbookId, vm.tempFlight.flightId, vm.tempApproach.approachId, vm.tempApproach)
                    .then(updateApproachSucceeded, updateApproachFailed);
            }
        }

        // Handle failed update (PUT) to approaches resource
        function updateApproachFailed(err) {
            vm.approachMessage = LOGBOOK_CONSTANT.MSG_APPROACH_UPDATE_ERROR;
            vm.working = false;
        }

        // Handle successful update approach
        function updateApproachSucceeded(response) {
            getFlight(vm.tempFlight.flightId);
            vm.approachForm.$setPristine();
            vm.approachForm.$setUntouched();
            vm.approachMessage = LOGBOOK_CONSTANT.MSG_APPROACH_UPDATED;
            vm.working = false;
        }

        // Update a flight
        function updateFlight() {
            vm.working = true;
            vm.tempFlight.flightDate = vm.tempFlightDateAsDate.toISOString();
            logbookService.flightsResource.updateFlight(vm.activeLogbook.logbookId, vm.tempFlight.flightId, vm.tempFlight)
                .then(updateFlightSucceeded, updateFlightFailed);
        }

        // Handle failed update flight
        function updateFlightFailed(error) {
            vm.message = LOGBOOK_CONSTANT.MSG_FLIGHT_UPDATE_ERROR;
            vm.working = false;
        }

        // Handle successful update flight
        function updateFlightSucceeded(response) {
            vm.message = LOGBOOK_CONSTANT.MSG_FLIGHT_UPDATED;
            queryFlights();
            vm.flightForm.$setPristine();
            vm.flightForm.$setUntouched();
            vm.working = false;
        }
    }
})();

