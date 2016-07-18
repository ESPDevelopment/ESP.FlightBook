(function () {
    'use strict';

    // Add resource (service) to the module
    angular
        .module('app.logbook')
        .factory('currenciesResource', currenciesResource);

    // Inject dependencies
    currenciesResource.$inject = ['cacheService', 'utilService', 'ENVIRONMENT_CONFIG'];

    // Define resource (service)
    function currenciesResource(cacheService, utilService, ENVIRONMENT_CONFIG) {

        // Define attributes
        var currencies = [];
        var isDataLoaded = false;
        var resource;

        // Define the resource (service)
        var service = {

            currencies: currencies,
            isDataLoaded: isDataLoaded,

            addCurrency: addCurrency,
            deleteCurrency: deleteCurrency,
            getCurrency: getCurrency,
            queryCurrencies: queryCurrencies,
            updateCurrency: updateCurrency,
        };
        initialize();
        return service;

        // Initialize the resource
        function initialize() {
            resource = cacheService.createResource(ENVIRONMENT_CONFIG.RESOURCE_URL_CURRENCIES, { logbookId: '@logbookId', currencyId: '@currencyId' });
        }

        // Add a currency
        function addCurrency(logbookId, currency) {
            var promise = resource.add({ logbookId: logbookId }, currency).$promise
                .then(flushCacheEntries(logbookId));
            return promise;
        }

        // Delete a currency
        function deleteCurrency(logbookId, currencyId) {
            var promise = resource.delete({ logbookId: logbookId, currencyId: currencyId }).$promise
                .then(flushCacheEntries(logbookId, currencyId));
            return promise;
        }

        // Flush appropriate cache entries
        function flushCacheEntries(logbookId, currencyId) {
            if (angular.isDefined(logbookId)) {

                // Flush currencies entries
                var currenciesUri = ENVIRONMENT_CONFIG.RESOURCE_URL_CURRENCIES.replace(':logbookId', logbookId);
                if (angular.isDefined(currencyId)) {
                    var getCurrencyKey = currenciesUri.replace(':currencyId', currencyId);
                    cacheService.flushCacheEntry(getCurrencyKey);
                }
                var queryCurrencyKey = currenciesUri.replace('/:currencyId', '');
                cacheService.flushCacheEntry(queryCurrencyKey);
            }
        }

        // Get a currency
        function getCurrency(logbookId, currencyId) {
            var promise = resource.get({ logbookId: logbookId, currencyId: currencyId }).$promise;
            return promise;
        }

        // Query currencies
        function queryCurrencies(logbookId) {
            var promise = resource.query({ logbookId: logbookId }).$promise
                .then(queryCurrenciesSucceeded, queryCurrenciesFailed);
            return promise;
        }

        // Handle failed query currencies
        function queryCurrenciesFailed(err) {
            utilService.clearArray(currencies);
            isDataLoaded = false;
            return err;
        }

        // Handle successful query currencies
        function queryCurrenciesSucceeded(response) {
            utilService.clearArray(currencies);
            utilService.addToArray(currencies, response.data);
            isDataLoaded = true;
            return response;
        }

        // Update a currency
        function updateCurrency(logbookId, currencyId, currency) {
            var promise = resource.update({ logbookId: logbookId, currencyId: currencyId }, currency).$promise
                .then(flushCacheEntries(logbookId, currencyId));
            return promise;
        }

    }
})();