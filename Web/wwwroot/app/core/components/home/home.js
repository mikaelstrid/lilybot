'use strict';

angular.module('myApp.home', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/', {
        templateUrl: 'app/core/components/home/homeView.html',
        controller: 'HomeCtrl',
        controllerAs: 'vm'
    });
}])