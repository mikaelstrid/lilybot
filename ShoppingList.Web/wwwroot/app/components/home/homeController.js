'use strict';

angular.module('myApp.home').controller('HomeCtrl', ['$scope', '$http', function ($scope, $http) {

    $scope.orders = [];

    $http.get('http://localhost:54299/api/protected').then(function (results) {
        $scope.orders = results.data;
    }, function (error) {
        //alert(error.data.message);
    });
}]);