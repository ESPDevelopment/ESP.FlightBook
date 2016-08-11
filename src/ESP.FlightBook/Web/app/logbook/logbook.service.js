(function () {
    'use strict';

    // Add service to the module
    angular
        .module('app.logbook')
        .factory('logbookService', logbookService);

    // Inject dependencies
    logbookService.$inject = ['$rootScope', '$location', 'localStorageService', 'cacheService', 'aircraftResource', 'approachesResource', 'certificatesResource', 'constantsResource', 'currenciesResource', 'endorsementsResource', 'flightsResource', 'logbooksResource', 'pilotsResource', 'ratingsResource', 'summaryResource', 'exportResource', 'LOGBOOK_CONSTANT'];

    // Define the service
    function logbookService($rootScope, $location, localStorageService, cacheService, aircraftResource, approachesResource, certificatesResource, constantsResource, currenciesResource, endorsementsResource, flightsResource, logbooksResource, pilotsResource, ratingsResource, summaryResource, exportResource, LOGBOOK_CONSTANT) {

        // Define the service
        var activeLogbook = { title: LOGBOOK_CONSTANT.TXT_SELECT_LOGBOOK, logbookId: 0 };
        var preferenceData = {};
        var service = {

            aircraftResource: aircraftResource,
            approachesResource: approachesResource,
            certificatesResource: certificatesResource,
            constantsResource: constantsResource,
            currenciesResource: currenciesResource,
            endorsementsResource: endorsementsResource,
            exportResource: exportResource,
            flightsResource: flightsResource,
            logbooksResource: logbooksResource,
            pilotsResource: pilotsResource,
            ratingsResource: ratingsResource,
            summaryResource: summaryResource,

            flushCache: flushCache,

            activeLogbook: activeLogbook,
            clearPreferences: clearPreferences,
            initialize: initialize,
            setActiveLogbook: setActiveLogbook,
        };
        return service;

        // Clear preference data
        function clearPreferences() {
            preferenceData.activeLogbook = {};
            localStorageService.remove(LOGBOOK_CONSTANT.KEY_PREFERENCE_DATA);
        }

        // Flush the cache
        function flushCache() {
            cacheService.flushCache();
        }

        // Load constants from the API service
        function getConstants() {
            constantsResource.getConstants();
        }

        // Initialize the service
        function initialize() {
            getConstants();
            loadPreferences();
            $rootScope.$on('$routeChangeStart', isNotSelected);
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_SIGNIN_SUCCESSFUL, loadPreferences);
        }

        // Redirects logbook routes to the manage page
        function isNotSelected(event, next, current) {
            if (activeLogbook.logbookId <= 0) {
                if ($.inArray($location.path(), LOGBOOK_CONSTANT.PATH_LOGBOOK_PATHS) >= 0) {
                    return $location.path(LOGBOOK_CONSTANT.PATH_DEFAULT_WHEN_NOT_SELECTED);
                }
            }
        }

        // Loads user preferences
        function loadPreferences() {
            activeLogbook.title = LOGBOOK_CONSTANT.TXT_SELECT_LOGBOOK
            activeLogbook.logbookId = 0;
            var savedPreferenceData = localStorageService.get(LOGBOOK_CONSTANT.KEY_PREFERENCE_DATA);
            if (savedPreferenceData) {
                if (angular.isDefined(savedPreferenceData.activeLogbook) && angular.isDefined(savedPreferenceData.activeLogbook.logbookId)) {
                    activeLogbook.logbookId = savedPreferenceData.activeLogbook.logbookId;
                }
                if (angular.isDefined(savedPreferenceData.activeLogbook) && angular.isDefined(savedPreferenceData.activeLogbook.title)) {
                    activeLogbook.title = savedPreferenceData.activeLogbook.title;
                }
            }
        }

        // Set the active logbook
        function setActiveLogbook(title, logbookId) {
            activeLogbook.title = title;
            activeLogbook.logbookId = logbookId;
            preferenceData.activeLogbook = activeLogbook;
            localStorageService.set(LOGBOOK_CONSTANT.KEY_PREFERENCE_DATA, preferenceData);
            $rootScope.$broadcast(LOGBOOK_CONSTANT.EVENT_LOGBOOK_SELECTED);
        }
    }
})();

