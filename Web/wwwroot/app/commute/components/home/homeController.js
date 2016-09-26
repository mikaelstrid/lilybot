(function () {
    'use strict';

    angular
        .module('myApp.commute.home')
        .controller('CommuteHomeCtrl', controller);

    controller.$inject = ['$log', '$location', '$mdToast', '$geolocation', 'profilesService', 'vasttrafikService', 'googleTrafficService'];

    function controller($log, $location, $mdToast, $geolocation, profilesService, vasttrafikService, googleTrafficService) {
        var vm = this;

        vm.isWorking = false;
        
        vm.upcomingPublicTransportTrips = [];
        vm.carRouteAlternatives = [];

        vm.profile = null;

        vm.selectedTabIndex = 0;

        vm.switchToPublicTransportTab = function () {
            vm.upcomingPublicTransportTrips = [];
            getUpcomingPublicTransportTrips();
        }

        vm.switchToCarTab = function () {
            vm.carRouteAlternatives = [];
            getCarRouteAlternatives();
        }
        

        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (!error || error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            console.log('Call to ' + failedMethodName + ' failed: ' + (error ? error.statusText : ''));
        }

        function createProfile(response) {
            return {
                homeLocation: new google.maps.LatLng(response.homeLocationLatitude, response.homeLocationLongitude),
                workLocation: new google.maps.LatLng(response.workLocationLatitude, response.workLocationLongitude),
                homePublicTransportStationId: response.homePublicTransportStationId,
                workPublicTransportStationId: response.workPublicTransportStationId,
                primaryWayOfCommuting: response.primaryWayOfCommuting
            };
        }

        function getSelectedTabIndex(primaryWayOfCommuting) {
            return primaryWayOfCommuting === "publictransport" ? 0 : 1;
        }

        function getUpcomingPublicTransportTrips() {
            vm.isWorking = true;
            $geolocation.getCurrentPosition({
                timeout: 60000
            }).then(function (position) {
                vasttrafikService.getUpcomingTrips(position.coords, vm.profile.homeLocation, vm.profile.workLocation, vm.profile.homePublicTransportStationId, vm.profile.workPublicTransportStationId)
                    .then(
                        function (trips) { vm.upcomingPublicTransportTrips = trips },
                        function (reason) { showError(reason + ' :(', 'vasttrafikService.upcomingPublicTransportTrips', null); }
                    )
                    .finally(
                        function () { vm.isWorking = false; }
                    );
            }, function () {
                showError('Kunde inte bestämma din position :(', '$geolocation.getCurrentPosition', null);
                vm.isWorking = false;
            });
        }

        function getCarRouteAlternatives() {
            if (vm.isWorking) return;
            vm.isWorking = true;
            $geolocation.getCurrentPosition({
                timeout: 60000
            }).then(function (position) {
                googleTrafficService.getCarRouteAlternatives(position.coords, vm.profile.homeLocation, vm.profile.workLocation)
                    .then(
                        function (routes) { vm.carRouteAlternatives = routes },
                        function (reason) { showError(reason + ' :(', 'googleTrafficService.getCarRouteAlternatives', null); }
                    )
                    .finally(
                        function () { vm.isWorking = false; }
                    );
            }, function () {
                showError('Kunde inte bestämma din position :(', '$geolocation.getCurrentPosition', null);
                vm.isWorking = false;
            });
        }


        // === INITIALIZATION ===
        activate();

        function activate() {
            profilesService.getMyProfile()
                .then(
                    function (response) {
                        vm.profile = createProfile(response.data);
                        vm.selectedTabIndex = getSelectedTabIndex(vm.profile.primaryWayOfCommuting);
                    },
                    function (response) { showError('Kunde inte hämta din profil :(', 'profilesService.getMyProfile', response) }
                );
        }
    }
})();
