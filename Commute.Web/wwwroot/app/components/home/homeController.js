(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('HomeCtrl', controller);

    controller.$inject = ['$scope', '$location', '$mdToast', '$geolocation', 'authService', 'vasttrafikService'];

    function controller($scope, $location, $mdToast, $geolocation, authService, vasttrafikService) {
        //$scope.isLoading = true;
        $scope.isWorking = false;

        $scope.upcomingTrips = [];

        $scope.authData = {
            isAuthorized: false
        };

        $scope.logout = function() {
            authService.logOut();
            $scope.authData.isAuthorized = authService.authentication.isAuth;
        }




        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (!error || error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            console.log('Call to ' + failedMethodName + ' failed: ' + (error ? error.statusText : ''));
        }

        function getUpcomingTrips() {
            $scope.isWorking = true;
            $geolocation.getCurrentPosition({
                timeout: 60000
            }).then(function (position) {
                vasttrafikService.getUpcomingTrips(position.coords)
                    .then(
                        function (trips) { $scope.upcomingTrips = trips },
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


        // === INITIALIZATION ===
        activate();

        $scope.$watch('authData.isAuthorized', function () {
            if ($scope.authData.isAuthorized) {
                getUpcomingTrips();
            }
        });

        function activate() {
            $scope.authData.isAuthorized = authService.authentication.isAuth;
        }
    }
})();
