'use strict';

var app = angular.module('myApp',
[
    'ngMaterial',
    'ngRoute',
    'LocalStorageModule',
    'myApp.home',
    'myApp.createAccount',
    'myApp.shopping'
]);

app.config(['$locationProvider', '$routeProvider', '$mdThemingProvider', function ($locationProvider, $routeProvider, $mdThemingProvider) {
    $locationProvider.hashPrefix('!');
    $routeProvider.otherwise({ redirectTo: '/' });

    $mdThemingProvider.theme('default')
        .primaryPalette('indigo')
        .accentPalette('purple');

    $mdThemingProvider.theme('introTheme')
        .dark();
}]);

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authenticationInterceptorService');
});

app.run(['authenticationService', function (authenticationService) {
    authenticationService.loadUserData();
}]);


