'use strict';

angular.module('myApp.associate').controller('AssociateCtrl', ['$scope', '$location', 'authService', 'ngAuthSettings', function ($scope, $location, authService, ngAuthSettings) {

    $scope.registerData = {
        userName: authService.externalAuthData.userName,
        provider: authService.externalAuthData.provider,
        externalAccessToken: authService.externalAuthData.externalAccessToken
    };

    $scope.registerExternal = function () {

        authService.registerExternal($scope.registerData)
            .then(function (response) {
                $location.path('/home');
            },
            function (response) {
                var errors = [];
                for (var key in response.modelState) {
                    errors.push(response.modelState[key]);
                }
                $scope.message = "Failed to register user due to:" + errors.join(' ');
            });
    };

}]);