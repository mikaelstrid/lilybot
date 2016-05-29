'use strict';

angular.module('myApp.viewLogin', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/login', {
        templateUrl: 'app/viewLogin/viewLogin.html',
        controller: 'LoginCtrl'
    });
}])

.controller('LoginCtrl', [function () {

}]);