(function () {
    'use strict';

    angular
        .module('myApp.createAccount')
        .controller('CreateAccountCtrl', controller);

    controller.$inject = ['$scope', '$location', 'authenticationService'];

    function controller($scope, $location, authenticationService) {

        $scope.userDisplayName = authenticationService.externalAuthData.userName;

        $scope.createAccount = function () {
            var externalAuthData = authenticationService.externalAuthData;
            authenticationService.createAccount(externalAuthData.provider, externalAuthData.accessToken)
                .then(
                    function () {
                        $location.path('/home');
                    },
                    function (response) {
                        var errors = [];
                        for (var key in response.modelState) {
                            errors.push(response.modelState[key]);
                        }
                        $scope.message = "Failed to register user due to:" + errors.join(' ');
                        console.log($scope.message);
                    });
        };

    }
})();