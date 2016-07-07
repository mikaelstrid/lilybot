'use strict';

//var app = angular.module('myApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar']);

var app = angular.module('myApp',
[
    'ngMaterial',
    'ngRoute',
    'LocalStorageModule',
    'myApp.home',
    'myApp.associate',
    'myApp.stores',
    'myApp.products',
    'myApp.planning',
    'myApp.shopping'
]);

app.config(['$locationProvider', '$routeProvider', '$mdThemingProvider', function ($locationProvider, $routeProvider, $mdThemingProvider) {
    $locationProvider.hashPrefix('!');
    $routeProvider.otherwise({ redirectTo: '/' });

    $mdThemingProvider.theme('default')
        .primaryPalette('amber')
        .accentPalette('deep-orange');
}]);

app.constant('appSettings', {
    authApiServiceBaseUri: 'http://localhost:51350/',
    resourceApiServiceBaseUri: 'http://localhost:54299/',
    //authApiServiceBaseUri: 'http://lilyauthenticationapi.azurewebsites.net/',
    //resourceApiServiceBaseUri: 'http://lilyshoppinglistapi.azurewebsites.net/',
    clientId: 'lily.shoppinglist'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);


