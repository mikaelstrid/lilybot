'use strict';

angular.module('myApp.stores', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/stores', {
        templateUrl: 'app/components/stores/storesView.html',
        controller: 'StoresCtrl'
    });
}])