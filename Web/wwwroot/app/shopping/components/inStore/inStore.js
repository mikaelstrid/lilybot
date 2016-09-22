'use strict';

angular.module('myApp.shopping.inStore', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/handla/valj-butik', {
        templateUrl: 'app/shopping/components/inStore/selectStoreView.html',
        controller: 'SelectStoreCtrl'
    });
}])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/handla/i-butik/:id', {
        templateUrl: 'app/shopping/components/inStore/inStoreView.html',
        controller: 'ShoppingCtrl',
        controllerAs: 'vm'
    });
}])