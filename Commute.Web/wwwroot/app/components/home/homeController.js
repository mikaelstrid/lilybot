(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('HomeCtrl', controller);

    controller.$inject = ['$scope', '$log', '$location', '$mdToast', '$geolocation', 'authService', 'vasttrafikService'];

    function controller($scope, $log, $location, $mdToast, $geolocation, authService, vasttrafikService) {
        //$scope.isLoading = true;
        $scope.isWorking = false;

        $scope.upcomingPublicTransportTrips = [];

        $scope.authData = {
            isAuthorized: false
        };

        $scope.logout = function() {
            authService.logOut();
            $scope.authData.isAuthorized = authService.authentication.isAuth;
        }

        $scope.switchToPublicTransportTab = function() {
            if ($scope.authData.isAuthorized) {
                $scope.upcomingPublicTransportTrips = [];
                getUpcomingPublicTransportTrips();
            }
        }

        $scope.switchToCarTab = function() {
            if ($scope.authData.isAuthorized) {
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

        function getUpcomingPublicTransportTrips() {
            $scope.isWorking = true;
            $geolocation.getCurrentPosition({
                timeout: 60000
            }).then(function (position) {
                vasttrafikService.getUpcomingTrips(position.coords)
                    .then(
                        function (trips) { $scope.upcomingPublicTransportTrips = trips },
                        function (reason) { showError(reason + ' :(', 'vasttrafikService.getUpcomingTrips', null); }
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
            $log.log('car');
        }


        // === INITIALIZATION ===
        activate();

        function activate() {
            $scope.authData.isAuthorized = authService.authentication.isAuth;
        }
    }
})();
