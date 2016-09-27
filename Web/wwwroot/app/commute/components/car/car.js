'use strict';

angular.module('myApp.commute.car', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/pendla/bil', {
        templateUrl: 'app/commute/components/car/carView.html',
        controller: 'CarCtrl',
        controllerAs: 'vm'
    });
}])