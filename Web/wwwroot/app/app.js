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
        .primaryPalette('purple')
        .accentPalette('deep-purple');

    $mdThemingProvider.theme('introTheme')
        .dark();
}]);

var AUTH_API_SERVICE_BASE_URI = 'http://localhost:51350/';
var RESOURCE_API_SERVICE_BASE_URI = 'http://localhost:54299/';
//var AUTH_API_SERVICE_BASE_URI = 'https://lilybotauthapi.azurewebsites.net/';
//var RESOURCE_API_SERVICE_BASE_URI = 'https://lilybotshoppingapi.azurewebsites.net/';

app.constant('appSettings', {
    authApiServiceBaseUri: AUTH_API_SERVICE_BASE_URI,
    resourceApiServiceBaseUri: RESOURCE_API_SERVICE_BASE_URI,
    clientId: 'lilybot.web',
    appendAuthTokenUrls: [RESOURCE_API_SERVICE_BASE_URI]
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authenticationInterceptorService');
});

app.run(['authenticationService', function (authenticationService) {
    authenticationService.loadUserData();
}]);


