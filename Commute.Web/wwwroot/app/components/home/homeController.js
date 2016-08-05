(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('HomeCtrl', controller);

    controller.$inject = ['$scope', '$log', '$location', '$mdToast', '$geolocation', 'authService', 'profilesService', 'vasttrafikService', 'googleTrafficService'];

    function controller($scope, $log, $location, $mdToast, $geolocation, authService, profilesService, vasttrafikService, googleTrafficService) {
        //$scope.isLoading = true;
        $scope.isWorking = false;
        
        $scope.upcomingPublicTransportTrips = [];
        $scope.carRouteAlternatives = [];

        $scope.profile = null;
        $scope.authData = {
            isAuthorized: false
        };

        $scope.logout = function () {
            authService.logOut();
            $scope.authData.isAuthorized = authService.authentication.isAuth;
        }

        $scope.selectedTabIndex = 0;

        $scope.switchToPublicTransportTab = function () {
            if ($scope.authData.isAuthorized) {
                $scope.upcomingPublicTransportTrips = [];
                getUpcomingPublicTransportTrips();
            }
        }

        $scope.switchToCarTab = function () {
            if ($scope.authData.isAuthorized) {
                $scope.carRouteAlternatives = [];
                getCarRouteAlternatives();
            }
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
            $scope.isWorking = true;
            $geolocation.getCurrentPosition({
                timeout: 60000
            }).then(function (position) {
                vasttrafikService.getUpcomingTrips(position.coords, $scope.profile.homeLocation, $scope.profile.workLocation, $scope.profile.homePublicTransportStationId, $scope.profile.workPublicTransportStationId)
                    .then(
                        function (trips) { $scope.upcomingPublicTransportTrips = trips },
                        function (reason) { showError(reason + ' :(', 'vasttrafikService.upcomingPublicTransportTrips', null); }
                    )
                    .finally(
                        function () { $scope.isWorking = false; }
                    );
            }, function () {
                showError('Kunde inte bestämma din position :(', '$geolocation.getCurrentPosition', null);
                $scope.isWorking = false;
            });
        }

        function getCarRouteAlternatives() {
            if ($scope.isWorking) return;
            $scope.isWorking = true;
            $geolocation.getCurrentPosition({
                timeout: 60000
            }).then(function (position) {
                googleTrafficService.getCarRouteAlternatives(position.coords, $scope.profile.homeLocation, $scope.profile.workLocation)
                    .then(
                        function (routes) { $scope.carRouteAlternatives = routes },
                        function (reason) { showError(reason + ' :(', 'googleTrafficService.getCarRouteAlternatives', null); }
                    )
                    .finally(
                        function () { $scope.isWorking = false; }
                    );
            }, function () {
                showError('Kunde inte bestämma din position :(', '$geolocation.getCurrentPosition', null);
                $scope.isWorking = false;
            });
        }


        // === INITIALIZATION ===
        activate();

        $scope.$watch('authData.isAuthorized', function () {
            if ($scope.authData.isAuthorized) {
                profilesService.getMyProfile()
                    .then(
                        function(response) {
                            $scope.profile = createProfile(response.data);
                            $scope.selectedTabIndex = getSelectedTabIndex($scope.profile.primaryWayOfCommuting);
                        },
                        function (response) { showError('Kunde inte hämta din profil :(', 'profilesService.getMyProfile', response) }
                    );
            }
        });

        function activate() {
            $scope.authData.isAuthorized = authService.authentication.isAuth;
        }
    }
})();
