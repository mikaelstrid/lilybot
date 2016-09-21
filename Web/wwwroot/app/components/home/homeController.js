(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('HomeCtrl', controller);

    controller.$inject = ['$scope', '$location', '$mdToast', 'authenticationService'];

    function controller($scope, $location, $mdToast, authenticationService) {
        var vm = this;

        vm.user = {
            authenticationPending: false,
            isAuthorized: false
        };

        vm.goto = function (page) {
            $location.path('/' + page);
        }

        vm.logout = function() {
            authenticationService.logOut();
            vm.user.isAuthorized = authenticationService.userData.isAuthorized;
        }

        activate();

        function activate() {
            //window.setTimeout(function() { $scope.$apply(function() {
            //    vm.user.authenticationPending = false;
            //}); }, 300);
            vm.user.isAuthorized = authenticationService.userData.isAuthorized;
        }
    }
})();
