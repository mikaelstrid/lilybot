'use strict';

angular.module('myApp.products', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/products', {
        templateUrl: 'app/components/products/productsView.html',
        controller: 'ProductsCtrl'
    });
}])