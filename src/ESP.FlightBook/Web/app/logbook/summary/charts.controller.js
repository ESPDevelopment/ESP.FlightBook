(function () {
    'use strict';

    // Add controller to module
    angular
        .module('app.logbook')
        .controller('ChartsController', ChartsController);

    // Inject dependencies
    ChartsController.$inject = ['$rootScope', 'googleChartApiPromise', 'logbookService', 'LOGBOOK_CONSTANT'];

    // Define controller
    function ChartsController($rootScope, googleChartApiPromise, logbookService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.activeLogbook = logbookService.activeLogbook;
        vm.hoursByTypeChart = {};
        vm.hoursByTypeData = {};
        vm.hoursByYearChart = {};
        vm.hoursByYearData = {};
        vm.hoursSummary = {};

        // Trigger controller activation
        activate();

        // Activate the controller
        function activate() {
            googleChartApiPromise.then(buildCharts);
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_LOGBOOK_SELECTED, buildCharts);
        }

        // Build data tables
        function buildCharts() {
            getHoursSummary();
            buildHoursByTypeChart();
            buildHoursByYearChart();
        }

        // Build hours by type chart
        function buildHoursByTypeChart() {
            vm.hoursByTypeData = new google.visualization.DataTable();
            vm.hoursByTypeData.addColumn("string", "Type");
            vm.hoursByTypeData.addColumn("number", "Total Hours");
            vm.hoursByTypeChart = {
                type: 'PieChart',
                cssStyle: 'height:100%; width:100%',
                options: {
                    legend: { position: 'right' },
                    pieHole: 0.4,
                    chartArea: { left: '10%', top: '5%', width: '90%', height: '90%' }
                },
                data: vm.hoursByTypeData
            };
        }

        // Build hours by year chart
        function buildHoursByYearChart() {
            vm.hoursByYearData = new google.visualization.DataTable();
            vm.hoursByYearData.addColumn("string", "Year");
            vm.hoursByYearData.addColumn("number", "Total Hours");
            vm.hoursByYearChart = {
                type: 'LineChart',
                cssStyle: 'height:100%; width:100%',
                options: {
                    curveType: 'none',
                    legend: { position: 'bottom' },
                    chartArea: { left: '10%', top: '10%', width: '80%', height: '70%' },
                    pointsVisible: true
                },
                data: vm.hoursByYearData
            };
        }

        // Get hours summary information
        function getHoursSummary() {
            logbookService.summaryResource.getHoursSummary(vm.activeLogbook.logbookId)
                .then(getHoursSummarySucceeded, getHoursSummaryFailed);
        }

        // Handle failed get hours summary
        function getHoursSummaryFailed(err) {
            vm.message = LOGBOOK_CONSTANT.MSG_HOURS_SUMMARY_GET_ERROR;
        }

        // Handle successful get hours summary
        function getHoursSummarySucceeded(response) {
            angular.copy(response, vm.hoursSummary);
            for (var key in vm.hoursSummary.totalHoursByYear) {
                var year = key;
                var hours = vm.hoursSummary.totalHoursByYear[key];
                vm.hoursByYearData.addRow([year, hours]);
            }
            for (var key in vm.hoursSummary.totalHoursByType) {
                var type = key;
                var hours = vm.hoursSummary.totalHoursByType[key];
                vm.hoursByTypeData.addRow([type, hours]);
            }
        }
    }
})();