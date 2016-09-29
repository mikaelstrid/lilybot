(function () {
    'use strict';

    angular
        .module('myApp.commute.publicTransport')
        .controller('PublicTransportCtrl', controller);

    controller.$inject = ['$scope', '$log', '$location', '$mdDialog', '$mdToast', '$geolocation', 'vasttrafikService', 'profileService'];

    function controller($scope, $log, $location, $mdDialog, $mdToast, $geolocation, vasttrafikService, profileService) {
        var vm = this;
        vm.isLoading = false;
        vm.ErrorMessage = '';
        vm.upcomingPublicTransportTrips = [];
        vm.lastUpdateTime = null;

        vm.getUpcomingPublicTransportTrips = function() {
            vm.isLoading = true;
            vm.errorMessage = '';
            var me = profileService.me;
            $geolocation.getCurrentPosition({
                    timeout: 60000
                })
                .then(
                    function(position) {
                        vm.lastUpdateTime = null;
                        vm.upcomingPublicTransportTrips = [];
                        vasttrafikService.getUpcomingTrips(position.coords,
                                me.homeLocation,
                                me.workLocation,
                                me.homePublicTransportStationId,
                                me.workPublicTransportStationId)
                            .then(
                                function(trips) {
                                    vm.upcomingPublicTransportTrips = trips;
                                    vm.lastUpdateTime = new Date();
                                },
                                function(reason) {
                                    showError(reason + ' :(', 'vasttrafikService.upcomingPublicTransportTrips', null);
                                }
                            );
                    },
                    function() {
                        showError('Kunde inte bestämma din position :(', '$geolocation.getCurrentPosition', null);
                    })
                .finally(function () { vm.isLoading = false; });
        }

        vm.refresh = function() {
            vm.getUpcomingPublicTransportTrips();
        }


        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (!error || error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
                vm.errorMessage = messageToUser;
            }
            $log.log('Call to ' + failedMethodName + ' failed: ' + error.statusText);
        }
        
        activate();

        function activate() {
            if (profileService.me) {
                vm.getUpcomingPublicTransportTrips();
            } else {
                vm.isLoading = true;
                profileService.getMyProfile()
                    .then(
                        function() { vm.getUpcomingPublicTransportTrips(); },
                        function(response) {
                            showError('Kunde inte hämta din profil :(', 'profilesService.getMyProfile', response)
                        });
            }
        }
    }
})();