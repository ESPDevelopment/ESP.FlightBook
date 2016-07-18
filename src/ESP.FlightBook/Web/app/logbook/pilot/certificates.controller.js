(function () {
    'use strict';

    // Define certificates controller
    angular
        .module('app.logbook')
        .controller('CertificatesController', CertificatesController);

    // Inject dependencies
    CertificatesController.$inject = ['$rootScope', 'logbookService', 'utilService', 'LOGBOOK_CONSTANT'];

    function CertificatesController($rootScope, logbookService, utilService, LOGBOOK_CONSTANT) {

        var vm = this;

        // Available attributes
        vm.activeLogbook = logbookService.activeLogbook;
        vm.certificateMessage = '';
        vm.certificates = logbookService.certificatesResource.certificates;
        vm.constants = logbookService.constantsResource.constants;
        vm.ratingMessage = '';
        vm.showAddCertificateButton = false;
        vm.showAddRatingButton = false;
        vm.showDeleteCertificateButton = false;
        vm.showDeleteRatingButton = false;
        vm.showUpdateCertificateButton = false;
        vm.showUpdateRatingButton = false;
        vm.tempCertificate = {};
        vm.tempCertificateDateAsDate;
        vm.tempRating = {};
        vm.tempRatingDateAsDate;
        vm.working = false;

        // Available functions
        vm.addCertificate = addCertificate;
        vm.addRating = addRating;
        vm.deleteCertificate = deleteCertificate;
        vm.deleteRating = deleteRating;
        vm.initAddCertificateModal = initAddCertificateModal;
        vm.initAddRatingModal = initAddRatingModal;
        vm.initUpdateCertificateModal = initUpdateCertificateModal;
        vm.initUpdateRatingModal = initUpdateRatingModal;
        vm.updateCertificate = updateCertificate;
        vm.updateRating = updateRating;

        // Activate the controller
        activate();

        // Initializes the controller
        function activate() {
            queryCertificates();
            $rootScope.$on(LOGBOOK_CONSTANT.EVENT_LOGBOOK_SELECTED, queryCertificates);
            $('#ratingModal').on('hidden.bs.modal', function (e) {
                $(document.body).addClass('modal-open');
            })
        }

        // Add a certificate
        function addCertificate() {
            vm.working = true;
            vm.tempCertificate.certificateDate = vm.tempCertificateDateAsDate.toISOString();
            vm.tempCertificate.expirationDate = (vm.tempCertificateExpirationDateAsDate == undefined || vm.tempCertificateExpirationDateAsDate == null) ? null : vm.tempCertificateExpirationDateAsDate.toISOString();
            logbookService.certificatesResource.addCertificate(vm.activeLogbook.logbookId, vm.tempCertificate)
                .then(addCertificateSucceeded, addCertificateFailed);
        }

        // Handle failed add certificate
        function addCertificateFailed(err) {
            vm.certificateMessage = LOGBOOK_CONSTANT.MSG_CERTIFICATE_ADD_ERROR;
            vm.working = false;
        }

        // Handle successful add certificate
        function addCertificateSucceeded(response) {
            queryCertificates();
            vm.tempCertificate = {};
            vm.tempCertificateDateAsDate = utilService.getCurrentDate();
            vm.certificateForm.$setPristine();
            vm.certificateForm.$setUntouched();
            vm.certificateMessage = LOGBOOK_CONSTANT.MSG_CERTIFICATE_ADDED;
            vm.working = false;
        }

        // Add a rating
        function addRating() {
            if (angular.isUndefined(vm.tempCertificate.certificateId) || vm.tempCertificate.certificateId <= 0) {
                vm.tempRating.ratingId = getRandomId();
                vm.tempRating.ratingDate = vm.tempRatingDateAsDate.toISOString();
                vm.tempCertificate.ratings.push(vm.tempRating);
                addRatingSucceeded();
            } else {
                vm.working = true;
                vm.tempRating.ratingDate = vm.tempRatingDateAsDate.toISOString();
                logbookService.ratingsResource.addRating(vm.activeLogbook.logbookId, vm.tempCertificate.certificateId, vm.tempRating)
                    .then(addRatingSucceeded, addRatingFailed);
            }
        }

        // Handle failed add rating
        function addRatingFailed() {
            vm.ratingMessage = LOGBOOK_CONSTANT.MSG_RATING_ADD_ERROR;
            vm.working = false;
        }

        // Handle successful add rating
        function addRatingSucceeded() {
            queryCertificates();
            getCertificate(vm.tempCertificate.certificateId);
            vm.tempRating = {};
            vm.tempRatingDateAsDate = utilService.getCurrentDate();
            vm.ratingForm.$setPristine();
            vm.ratingForm.$setUntouched();
            vm.ratingMessage = LOGBOOK_CONSTANT.MSG_RATING_ADDED;
            vm.working = false;
        }

        // Delete a certificate
        function deleteCertificate() {
            vm.working = true;
            logbookService.certificatesResource.deleteCertificate(vm.activeLogbook.logbookId, vm.tempCertificate.certificateId)
                .then(deleteCertificateSucceeded, deleteCertificateFailed);
        }

        // Handle failed delete certificate
        function deleteCertificateFailed() {
            vm.showAddCertificateButton = false;
            vm.showDeleteCertificateButton = true;
            vm.showUpdateCertificateButton = true;
            vm.certificateMessage = LOGBOOK_CONSTANT.MSG_CERTIFICATE_DELETE_ERROR;
            vm.working = false;
        }

        // Handle successful delete certificate
        function deleteCertificateSucceeded() {
            queryCertificates();
            vm.tempCertificateDateAsDate = utilService.getCurrentDate();
            vm.showAddCertificateButton = false;
            vm.showDeleteCertificateButton = false;
            vm.showUpdateCertificateButton = false;
            vm.certificateMessage = LOGBOOK_CONSTANT.MSG_CERTIFICATE_DELETED;
            vm.working = false;
        }

        // Delete a rating
        function deleteRating() {
            if (angular.isUndefined(vm.tempCertificate.certificateId) || vm.tempCertificate.certificateId <= 0) {
                var index = utilService.getIndexByProperty('ratingId', vm.tempRating.ratingId, vm.tempCertificate.ratings);
                vm.tempCertificate.ratings.splice(index, 1);
                deleteRatingSucceeded();
            } else {
                vm.working = true;
                logbookService.ratingsResource.deleteRating(vm.activeLogbook.logbookId, vm.tempRating.certificateId, vm.tempRating.ratingId)
                    .then(deleteRatingSucceeded, deleteRatingFailed);
            }
        }

        // Handle failed delete (DELETE) on Rating resource
        function deleteRatingFailed() {
            vm.ratingMessage = LOGBOOK_CONSTANT.MSG_RATING_DELETE_ERROR;
            vm.working = false;
        }

        // Handle successful delete rating
        function deleteRatingSucceeded() {
            queryCertificates();
            getCertificate(vm.tempCertificate.certificateId);
            vm.tempRating = {};
            vm.showAddRatingButton = false;
            vm.showDeleteRatingButton = false;
            vm.showUpdateRatingButton = false;
            vm.ratingForm.$setPristine();
            vm.ratingForm.$setUntouched();
            vm.ratingMessage = LOGBOOK_CONSTANT.MSG_RATING_DELETED;
            vm.working = false;
        }

        // Retrieve the specified certificate
        function getCertificate(certificateId) {
            if (angular.isDefined(certificateId)) {
                vm.working = true;
                logbookService.certificatesResource.getCertificate(vm.activeLogbook.logbookId, certificateId)
                    .then(getCertificateSucceeded, getCertificateFailed);
            }
        }

        // Handle failed get certificate
        function getCertificateFailed(err) {
            vm.certificateMessage = LOGBOOK_CONSTANT.MSG_CERTIFICATE_GET_ERROR;
            vm.working = false;
        }

        // Handle successful get certificate
        function getCertificateSucceeded(response) {
            angular.copy(response, vm.tempCertificate);
            vm.tempCertificateDateAsDate = utilService.stringToDate(vm.tempCertificate.certificateDate);
            vm.tempCertificateExpirationDateAsDate = (response.certificateExpirationDate == undefined || response.certificateExpirationDate == null) ? undefined : utilService.stringToDate(response.certificateExpirationDate);
            vm.showDeleteCertificateButton = true;
            vm.showUpdateCertificateButton = true;
            vm.working = false;
        }

        // Return a random identifier
        function getRandomId() {
            return Math.floor((Math.random() * 100000) + 1);
        }

        // Initialize the "add" modal
        function initAddCertificateModal() {
            vm.certificateMessage = '';
            vm.showAddCertificateButton = true;
            vm.showDeleteCertificateButton = false;
            vm.showUpdateCertificateButton = false;
            vm.certificateForm.$setPristine();
            vm.certificateForm.$setUntouched();
            vm.tempCertificate = {};
            vm.tempCertificate.Ratings = [];
            vm.tempCertificateDateAsDate = utilService.getCurrentDate();
            vm.tempCertificateExpirationDateAsDate = undefined;
        }

        // Initialize the "add rating" form
        function initAddRatingModal() {
            vm.ratingMessage = '';
            vm.tempRating = {};
            vm.tempRatingDateAsDate = utilService.getCurrentDate();
            vm.showAddRatingButton = true;
            vm.showDeleteRatingButton = false;
            vm.showUpdateRatingButton = false;
            vm.ratingForm.$setPristine();
            vm.ratingForm.$setUntouched();
        }

        // Initialize the "edit" modal
        function initUpdateCertificateModal(certificateId) {
            vm.certificateMessage = '';
            vm.showAddCertificateButton = false;
            vm.showDeleteCertificateButton = false;
            vm.showUpdateCertificateButton = false;
            vm.certificateForm.$setPristine();
            vm.certificateForm.$setUntouched();
            getCertificate(certificateId);
        }

        // Initialize the "edit" rating modal
        function initUpdateRatingModal(certificateId, ratingId) {
            var rating = utilService.getElementByProperty('ratingId', ratingId, vm.tempCertificate.ratings);
            if (angular.isDefined(rating)) {
                vm.ratingMessage = '';
                angular.copy(rating, vm.tempRating);
                vm.tempRatingDateAsDate = utilService.stringToDate(vm.tempRating.ratingDate);
                vm.showAddRatingButton = false;
                vm.showDeleteRatingButton = true;
                vm.showUpdateRatingButton = true;
            } else {
                vm.ratingMessage = LOGBOOK_CONSTANT.MSG_RATING_GET_ERROR;
                vm.tempRating = { ratingId: getRandomId() };
                vm.showAddRatingButton = false;
                vm.showDeleteRatingButton = false;
                vm.showUpdateRatingButton = false;
            }
            vm.ratingForm.$setPristine();
            vm.ratingForm.$setUntouched();
        }

        // Get certificates information
        function queryCertificates() {
            vm.working = true;
            logbookService.certificatesResource.queryCertificates(vm.activeLogbook.logbookId)
                .then(queryCertificatesSucceeded, queryCertificatesFailed);
        }

        // Handle failed query certificates
        function queryCertificatesFailed(err) {
            vm.certificateMessage = LOGBOOK_CONSTANT.MSG_CERTIFICATE_QUERY_ERROR;
            vm.working = false;
        }

        // Handle successful query certificates
        function queryCertificatesSucceeded(response) {
            vm.working = false;
        }

        // Update certificate
        function updateCertificate() {
            vm.working = true;
            vm.tempCertificate.certificateDate = vm.tempCertificateDateAsDate.toISOString();
            vm.tempCertificate.certificateExpirationDate = (vm.tempCertificateExpirationDateAsDate == undefined || vm.tempCertificateExpirationDateAsDate == null) ? null : vm.tempCertificateExpirationDateAsDate.toISOString();
            logbookService.certificatesResource.updateCertificate(vm.activeLogbook.logbookId, vm.tempCertificate.certificateId, vm.tempCertificate)
                .then(updateCertificateSucceeded, updateCertificateFailed);
        }

        // Handle failed update certificate
        function updateCertificateFailed(err) {
            vm.certificateMessage = LOGBOOK_CONSTANT.MSG_CERTIFICATE_UPDATE_ERROR;
            vm.working = false;
        }

        // Handle successful update certificate
        function updateCertificateSucceeded(response) {
            queryCertificates();
            vm.certificateForm.$setPristine();
            vm.certificateForm.$setUntouched();
            vm.certificateMessage = LOGBOOK_CONSTANT.MSG_CERTIFICATE_UPDATED;
            vm.working = false;
        }

        // Update rating
        function updateRating() {
            if (angular.isUndefined(vm.tempCertificate.certificateId) || vm.tempCertificate.certificateId <= 0) {
                var index = utilService.getIndexByProperty('ratingId', vm.tempRating.ratingId, vm.tempCertificate.ratings);
                angular.copy(vm.tempRating, vm.tempCertificate.ratings[index]);
                updateRatingSucceeded();
            } else {
                vm.working = true;
                vm.tempRating.ratingDate = vm.tempRatingDateAsDate.toISOString();
                logbookService.ratingsResource.updateRating(vm.activeLogbook.logbookId, vm.tempRating.certificateId, vm.tempRating.ratingId, vm.tempRating)
                    .then(updateRatingSucceeded, updateRatingFailed);
            }
        }
        
        // Handle failed update rating
        function updateRatingFailed(err) {
            vm.ratingMessage = LOGBOOK_CONSTANT.MSG_RATING_UPDATE_ERROR;
            vm.working = false;
        }

        // Handled successful update rating
        function updateRatingSucceeded(response) {
            queryCertificates();
            getCertificate(vm.tempCertificate.certificateId);
            vm.ratingForm.$setPristine();
            vm.ratingForm.$setUntouched();
            vm.ratingMessage = LOGBOOK_CONSTANT.MSG_RATING_UPDATED;
            vm.working = false;
        }
    }
})();

