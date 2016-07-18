(function () {
    'use strict';

    // Add controller to the module
    angular
        .module('app.logbook')
        .controller('CurrenciesController', CurrenciesController);

    // Inject dependencies
    CurrenciesController.$inject = ['$rootScope', 'logbookService', 'LOGBOOK_CONSTANT'];

    // Define the controller
    function CurrenciesController($rootScope, logbookService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.activeLogbook = logbookService.activeLogbook;
        vm.currencies = logbookService.currenciesResource.currencies;
        vm.currenciesLoaded = false;
        vm.currencyForm;
        vm.constants = logbookService.constantsResource.constants;
        vm.message = '';
        vm.selectedCurrencyType = null;
        vm.showAddButton = false;
        vm.showDeleteButton = false;
        vm.showUpdateButton = false;
        vm.tempCurrency = {};
        vm.working = false;

        // Available functions
        vm.activate = activate;
        vm.addCurrency = addCurrency;
        vm.deleteCurrency = deleteCurrency;
        vm.getCurrency = getCurrency;
        vm.initAddModal = initAddModal;
        vm.initUpdateModal = initUpdateModal;
        vm.queryCurrencies = queryCurrencies;
        vm.updateCurrency = updateCurrency;

        // Activate the controller
        activate();

        // Define controller activation
        function activate() {
            queryCurrencies();
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_LOGBOOK_SELECTED, queryCurrencies);
        }

        // Add new currency
        function addCurrency() {
            vm.working = true;
            if (vm.selectedCurrencyType != null) {
                vm.tempCurrency.logbookId = vm.activeLogbook.logbookId;
                vm.tempCurrency.currencyType = vm.selectedCurrencyType;
                vm.tempCurrency.currencyTypeId = vm.selectedCurrencyType.currencyTypeId;
                if (vm.selectedCurrencyType.calculationType != 1) {
                    vm.tempCurrency.isNightCurrency = false;
                }
                logbookService.currenciesResource.addCurrency(vm.activeLogbook.logbookId, vm.tempCurrency)
                    .then(addCurrencySucceeded, addCurrencyFailed);
            }
        }

        // Handle failed add (POST) to Currencies resource
        function addCurrencyFailed(error) {
            vm.message = LOGBOOK_CONSTANT.MSG_CURRENCY_ADD_ERROR;
            vm.working = false;
        }

        // Handle successful add currency
        function addCurrencySucceeded(response) {
            queryCurrencies();
            vm.tempCurrency = {};
            vm.currencyForm.$setPristine();
            vm.currencyForm.$setUntouched();
            vm.message = LOGBOOK_CONSTANT.MSG_CURRENCY_ADDED;
            vm.working = false;
        }

        // Delete existing currency
        function deleteCurrency() {
            vm.working = true;
            logbookService.currenciesResource.deleteCurrency(vm.activeLogbook.logbookId, vm.tempCurrency.currencyId)
                .then(deleteCurrencySucceeded, deleteCurrencyFailed);
        }

        // Handle failed delete (DELETE) from Currencies resource
        function deleteCurrencyFailed(error) {
            vm.message = LOGBOOK_CONSTANT.MSG_CURRENCY_DELETE_ERROR;
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.working = false;
        }

        // Handle successful delete currency
        function deleteCurrencySucceeded(response) {
            queryCurrencies();
            vm.showDeleteButton = false;
            vm.showUpdateButton = false;
            vm.message = LOGBOOK_CONSTANT.MSG_CURRENCY_DELETED;
            vm.working = false;
        }

        // Get specified currency
        function getCurrency(currencyId) {
            vm.working = true;
            logbookService.currenciesResource.getCurrency(vm.activeLogbook.logbookId, currencyId)
                .then(getCurrencySucceeded, getCurrencyFailed);
        }

        // Handle failed get currency
        function getCurrencyFailed(error) {
            vm.message = LOGBOOK_CONSTANT.MSG_CURRENCY_GET_ERROR;
            vm.working = false;
        }

        // Handle successful get currency
        function getCurrencySucceeded(response) {
            angular.copy(response, vm.tempCurrency);
            vm.selectedCurrencyType = selectCurrencyType(vm.tempCurrency);
            vm.showDeleteButton = true;
            vm.showUpdateButton = true;
            vm.working = false;
        }

        // Initialize add modal dialog box
        function initAddModal() {
            vm.message = '';
            vm.showAddButton = true;
            vm.showDeleteButton = false;
            vm.showUpdateButton = false;
            vm.currencyForm.$setPristine();
            vm.currencyForm.$setUntouched();
            vm.tempCurrency = {};
            vm.selectedCurrencyType = null;
        }

        // Initialize update modal dialog box
        function initUpdateModal(currencyId) {
            vm.message = '';
            vm.showAddButton = false;
            vm.showDeleteButton = false;
            vm.showUpdateButton = false;
            vm.currencyForm.$setPristine();
            vm.currencyForm.$setUntouched();
            vm.getCurrency(currencyId);
        }

        // Retrieve currencies for specified pilot
        function queryCurrencies() {
            vm.working = true;
            vm.currenciesLoaded = false;
            logbookService.currenciesResource.queryCurrencies(vm.activeLogbook.logbookId)
                .then(queryCurrenciesSucceeded, queryCurrenciesFailed);
        }

        // Handle failed query (GET) from Currencies resource
        function queryCurrenciesFailed(error) {
            vm.message = LOGBOOK_CONSTANT.MSG_CURRENCY_QUERY_ERROR;
            vm.currenciesLoaded = false;
            vm.working = false;
        }

        // Handle successful query currencies
        function queryCurrenciesSucceeded(response) {
            vm.currenciesLoaded = true;
            vm.working = false;
        }

        // Select appropriate currency type
        function selectCurrencyType(currency) {
            var selectedItem = "";
            if (currency != null) {
                angular.forEach(vm.constants.currencyTypes, function (row) {
                    if (row.currencyTypeId == currency.currencyTypeId) {
                        selectedItem = row;
                    }
                });
            }
            return selectedItem;
        }

        // Update currency
        function updateCurrency() {
            vm.working = true;
            if (vm.selectedCurrencyType != null) {
                vm.tempCurrency.logbookId = vm.activeLogbook.logbookId;
                vm.tempCurrency.currencyType = vm.selectedCurrencyType;
                vm.tempCurrency.currencyTypeId = vm.selectedCurrencyType.currencyTypeId;
                if (vm.selectedCurrencyType.calculationType != 1) {
                    vm.tempCurrency.isNightCurrency = false;
                }
                logbookService.currenciesResource.updateCurrency(vm.activeLogbook.logbookId, vm.tempCurrency.currencyId, vm.tempCurrency)
                    .then(updateCurrencySucceeded, updateCurrencyFailed);
            }
        }

        // Handle failed update (PUT) to Currencies resource
        function updateCurrencyFailed(error) {
            vm.message = LOGBOOK_CONSTANT.MSG_AIRCRAFT_UPDATE_ERROR;
            vm.working = false;
        }

        // Handle successful update currency
        function updateCurrencySucceeded(response) {
            queryCurrencies();
            vm.currencyForm.$setPristine();
            vm.currencyForm.$setUntouched();
            vm.message = LOGBOOK_CONSTANT.MSG_CURRENCY_UPDATED;
            vm.working = false;
        }
    }
})();