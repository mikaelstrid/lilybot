'use strict';

angular.module('myApp.shopping.home', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/handla', {
        templateUrl: 'app/shopping/components/home/homeView.html',
        controller: 'ShoppingHomeCtrl'
    });
}])