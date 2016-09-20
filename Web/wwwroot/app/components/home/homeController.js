(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('HomeCtrl', controller);

    controller.$inject = ['$scope', '$location', '$mdToast', 'authenticationService'];

    function controller($scope, $location, $mdToast, authenticationService) {
        var vm = this;

        vm.user = {
            authenticationPending: true,
            isAuthorized: false
        };

        vm.goto = function (page) {
            $location.path('/' + page);
        }

        vm.logout = function() {
            authenticationService.logOut();
            vm.user.isAuthorized = authenticationService.userData.isAuthorized;
        }

        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
                console.log('Call to storesService.' + failedMethodName + ' failed: ' + error.statusText);
            }
        }

        activate();

        function activate() {
            window.setTimeout(function() { $scope.$apply(function() {
                vm.user.authenticationPending = false;
            }); }, 300);
        }
    }
})();
