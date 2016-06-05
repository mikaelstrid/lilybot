'use strict';

//var app = angular.module('myApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar']);

var app = angular.module('myApp',
[
    'ngMaterial',
    'ngRoute',
    'LocalStorageModule',
    'myApp.home',
    'myApp.login',
    'myApp.associate',
    'myApp.stores',
    'myApp.products'
]);

app.config(['$locationProvider', '$routeProvider', function ($locationProvider, $routeProvider) {
    $locationProvider.hashPrefix('!');
    $routeProvider.otherwise({ redirectTo: '/home' });
}]);

//var serviceBase = 'http://ngauthenticationapi.azurewebsites.net/';
app.constant('appSettings', {
    authApiServiceBaseUri: 'http://localhost:51350/',
    resourceApiServiceBaseUri: 'http://localhost:54299/',
    clientId: 'lily.shoppinglist'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

//app.run(['authService', function (authService) {
//    authService.fillAuthData();
//}]);


