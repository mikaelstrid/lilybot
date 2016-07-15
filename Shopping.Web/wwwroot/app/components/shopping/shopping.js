'use strict';

angular.module('myApp.shopping', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/valj-butik', {
        templateUrl: 'app/components/shopping/selectStoreView.html',
        controller: 'SelectStoreCtrl'
    });
}])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/handla/:id', {
        templateUrl: 'app/components/shopping/shoppingView.html',
        controller: 'ShoppingCtrl'
    });
}])