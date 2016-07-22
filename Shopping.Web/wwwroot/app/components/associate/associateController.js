(function () {
    'use strict';

    angular
        .module('myApp.associate')
        .controller('AssociateCtrl', controller);

    controller.$inject = ['$scope', '$location', 'authService', 'profilesService'];

    function controller($scope, $location, authService, profilesService) {

        $scope.userDisplayName = authService.externalAuthData.userDisplayName;

        $scope.registerData = {
            provider: authService.externalAuthData.provider,
            externalAccessToken: authService.externalAuthData.externalAccessToken
        };

        $scope.registerExternal = function () {
            authService.registerExternal($scope.registerData)
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