'use strict';

angular.module('myApp.home', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/', {
        templateUrl: 'app/components/home/homeView.html',
        controller: 'HomeCtrl',
        controllerAs: 'vm'
    });
}])