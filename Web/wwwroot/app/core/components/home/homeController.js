(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('HomeCtrl', controller);

    controller.$inject = ['$scope', '$location', '$mdToast', 'authenticationService'];

    function controller($scope, $location, $mdToast, authenticationService) {
        var vm = this;

        vm.user = {
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
            vm.user.isAuthorized = authenticationService.userData.isAuthorized;
        }
    }
})();
