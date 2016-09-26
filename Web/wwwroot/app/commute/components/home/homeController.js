(function () {
    'use strict';

    angular
        .module('myApp.commute.home')
        .controller('CommuteHomeCtrl', controller);

    controller.$inject = ['$log', '$location', '$mdToast', '$mdSidenav', '$geolocation', 'profileService'];

    function controller($log, $location, $mdToast, $mdSidenav, $geolocation, profileService) {
        var vm = this;

        vm.isLoadingProfile = true;

        vm.goto = function (page) {
            $location.path('/' + page);
        }

        vm.onMenuButtonClicked = function () {
            //$mdSidenav('right').open();
        }

        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (!error || error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            $log.log('Call to ' + failedMethodName + ' failed: ' + (error ? error.statusText : ''));
        }



        // === INITIALIZATION ===
        activate();

        function activate() {
            profileService.getMyProfile()
                .then(
                    function () { },
                    function (response) { showError('Kunde inte hämta din profil :(', 'profilesService.getMyProfile', response) }
                )
                .finally(function () { vm.isLoadingProfile = false; });
        }
    }
})();
