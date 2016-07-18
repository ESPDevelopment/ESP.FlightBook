(function () {
    'use strict';

    // Add filter to the module
    angular
        .module('app.utils')
        .filter('pages', pages);

    // Inject dependencies
    pages.$inject = [];

    // Define the filter
    function pages() {

        var filter = filter;
        return filter;

        function filter(input, currentPage, totalPages, range) {

            // Convert input to integers
            currentPage = parseInt(currentPage);
            totalPages = parseInt(totalPages);
            range = parseInt(range);

            // Calculate minimum and maximum page
            var minPage = (currentPage - ((range * 2) + 1) < 0) ? 0 : ((currentPage + (range + 1)) < totalPages) ? (currentPage - range) : (totalPages - ((range * 2) + 1));
            var maxPage = (((range * 2) + 1) >= totalPages) ? totalPages : (currentPage >= totalPages) ? totalPages : (minPage + ((range * 2) + 1));

            // Return an array of page numbers
            for (var i = minPage; i < maxPage; i++) {
                input.push(i);
            }
            return input;
        }
    }
})();