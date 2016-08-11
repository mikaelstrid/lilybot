'use strict';

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

    var lightOrange = $mdThemingProvider.extendPalette('orange', {
        'contrastDefaultColor': 'light'
    });
    $mdThemingProvider.definePalette('lightOrange', lightOrange);

    $mdThemingProvider.theme('default')
        .primaryPalette('lightOrange')
        .accentPalette('pink');
}]);

//var AUTH_API_SERVICE_BASE_URI = 'http://localhost:51350/';
//var RESOURCE_API_SERVICE_BASE_URI = 'http://localhost:54299/';
var AUTH_API_SERVICE_BASE_URI = 'http://lilybotauthapi.azurewebsites.net/';
var RESOURCE_API_SERVICE_BASE_URI = 'http://lilybotshoppingapi.azurewebsites.net/';

app.constant('appSettings', {
    authApiServiceBaseUri: AUTH_API_SERVICE_BASE_URI,
    resourceApiServiceBaseUri: RESOURCE_API_SERVICE_BASE_URI,
    clientId: 'lilybot.shopping',
    appendAuthTokenUrls: [RESOURCE_API_SERVICE_BASE_URI]
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);


