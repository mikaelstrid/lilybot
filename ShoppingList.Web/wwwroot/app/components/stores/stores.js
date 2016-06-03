'use strict';

angular.module('myApp.stores', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/stores', {
        templateUrl: 'app/components/stores/storesView.html',
        controller: 'StoresCtrl'
    });
    $routeProvider.when('/stores/:id', {
        templateUrl: 'app/components/stores/storeDetailsView.html',
        controller: 'StoreDetailsCtrl'
    });
}])