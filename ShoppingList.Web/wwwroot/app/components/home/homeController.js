(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('HomeCtrl', controller);

    controller.$inject = ['$scope', '$location', '$mdSidenav', 'authService'];

    function controller($scope, $location, $mdSidenav, authService) {

        $scope.isAuthorized = false;

        $scope.logout = function() {
            authService.logOut();
            $scope.isAuthorized = authService.authentication.isAuth;
        }

        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            console.log('Call to storesService.' + failedMethodName + ' failed: ' + error.statusText);
        }

        function openSidenav() {
            $mdSidenav('right').open();
        }

        function closeSidenav() {
            $mdSidenav('right').close();
        }

        activate();

        function activate() {
            $scope.isAuthorized = authService.authentication.isAuth;
        }
    }
})();
