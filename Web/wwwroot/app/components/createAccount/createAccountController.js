(function () {
    'use strict';

    angular
        .module('myApp.createAccount')
        .controller('CreateAccountCtrl', controller);

    controller.$inject = ['$location', 'authenticationService'];

    function controller($location, authenticationService) {
        var vm = this;

        vm.goto = function (page) {
            $location.path('/' + page);
        }

        vm.userDisplayName = authenticationService.userData.externalDisplayName;

        vm.createAccount = function () {
            authenticationService.createAccount(authenticationService.userData.externalProvider, authenticationService.userData.externalAccessToken)
                .then(
                    function () {
                        $location.path('/home');
                    },
                    function (response) {
                        var errors = [];
                        for (var key in response.modelState) {
                            errors.push(response.modelState[key]);
                        }
                        vm.message = "Failed to register user due to:" + errors.join(' ');
                    });
        };
    }
})();