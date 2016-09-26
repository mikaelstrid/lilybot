'use strict';

angular.module('myApp.commute.home', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/pendla/', {
        templateUrl: 'app/commute/components/home/homeView.html',
        controller: 'CommuteHomeCtrl',
        controllerAs: 'vm'
    });
}])