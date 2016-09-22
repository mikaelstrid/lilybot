'use strict';

angular.module('myApp.shopping.stores', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/handla/butiker', {
        templateUrl: 'app/shopping/components/stores/storesView.html',
        controller: 'StoresCtrl'
    });
    $routeProvider.when('/handla/butiker/:id', {
        templateUrl: 'app/shopping/components/stores/storeDetailsView.html',
        controller: 'StoreDetailsCtrl'
    });
}])