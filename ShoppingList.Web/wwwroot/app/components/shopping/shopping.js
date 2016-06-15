'use strict';

angular.module('myApp.shopping', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/handla', {
        templateUrl: 'app/components/shopping/shoppingView.html',
        controller: 'ShoppingCtrl'
    });
}])