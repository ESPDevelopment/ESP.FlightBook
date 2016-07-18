(function () {
    'use strict';

    // Add service to module
    angular
        .module('app.utils')
        .service('utilService', utilService);

    // Inject dependencies
    utilService.$inject = [];

    // Implement the service
    function utilService() {

        // Define the service
        var service = {
            addDaysToDate: addDaysToDate,
            addToArray: addToArray,
            clearArray: clearArray,
            getCurrentDate: getCurrentDate,
            getElementByProperty: getElementByProperty,
            getIndexByProperty: getIndexByProperty,
            stringToDate: stringToDate
        };
        return service;

        // Returns the result of adding days to a reference date
        function addDaysToDate(date, days) {
            var refDate = new Date(date);
            var result = new Date(refDate);
            result.setDate(refDate.getDate() + days);
            result = result.toISOString();
            return result;
        }

        // Add elements to an array
        function addToArray(array, data) {
            if (angular.isDefined(array) && array != null) {
                if (angular.isDefined(data) && data != null) {
                    var length = data.length;
                    for (var i = 0; i < length; i++) {
                        array.push(data[i]);
                    }
                }
            }
        }

        // Clear an array without destroying it
        function clearArray(array) {
            if (angular.isDefined(array) && array != null) {
                var length = array.length;
                for (var i = 0; i < length; i++) {
                    array.pop(i);
                }
            }
        }

        // Returns the current date with zero-based time
        function getCurrentDate() {
            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            return currentDate;
        }

        // Returns an element from an array based on a property value
        function getElementByProperty(propertyName, propertyValue, collection) {

            // Validate parameters
            if (propertyName == undefined || propertyName == null)
                return null;
            if (propertyValue == undefined || propertyValue == null)
                return null;
            if (collection == undefined || collection == null)
                return null;

            // Search by property
            var len = collection.length;
            for (var i = 0; i < len; i++) {
                if (collection[i][propertyName] == propertyValue) {
                    return collection[i];
                }
            }
            return null;
        }

        // Returns the index of an object in an array based on a property value
        function getIndexByProperty(propertyName, propertyValue, collection) {

            // Validate parameters
            if (propertyName == undefined || propertyName == null)
                return -1;
            if (propertyValue == undefined || propertyValue == null)
                return -1;
            if (collection == undefined || collection == null)
                return -1;

            // Search by property
            var len = collection.length;
            for (var i = 0; i < len; i++) {
                if (collection[i][propertyName] == propertyValue) {
                    return i;
                }
            }
            return -1;
        }

        // Converts a string to a native JavaScript date
        function stringToDate(dateString) {
            var retDate = new Date();
            if (dateString == undefined || dateString == null)
                return retDate;
            var momentDate = moment(dateString);
            if (momentDate == undefined || momentDate == null || momentDate.isValid() == false)
                return retDate;
            var retDate = momentDate.toDate();
            retDate.setHours(0, 0, 0, 0);
            return retDate;
        }
    }
})();