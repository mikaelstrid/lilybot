'use strict';

angular.module('myApp.shopping.products', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/handla/produkter', {
        templateUrl: 'app/shopping/components/products/productsView.html',
        controller: 'ProductsCtrl'
    });
}])