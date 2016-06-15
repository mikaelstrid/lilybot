'use strict';

angular.module('myApp.products', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/produkter', {
        templateUrl: 'app/components/products/productsView.html',
        controller: 'ProductsCtrl'
    });
}])