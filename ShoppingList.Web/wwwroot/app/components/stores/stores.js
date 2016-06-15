'use strict';

angular.module('myApp.stores', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/butiker', {
        templateUrl: 'app/components/stores/storesView.html',
        controller: 'StoresCtrl'
    });
    $routeProvider.when('/butiker/:id', {
        templateUrl: 'app/components/stores/storeDetailsView.html',
        controller: 'StoreDetailsCtrl'
    });
}])