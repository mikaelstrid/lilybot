'use strict';

var app = angular.module('myApp',
[
    'ngMaterial',
    'ngRoute',
    'LocalStorageModule',
    'ngGeolocation',
    'myApp.home',
    'myApp.associate'
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


var AUTH_API_SERVICE_BASE_URI = 'http://localhost:51350/';
var RESOURCE_API_SERVICE_BASE_URI = 'http://localhost:6001/';
//var AUTH_API_SERVICE_BASE_URI = 'https://lilybotauthapi.azurewebsites.net/';
//var RESOURCE_API_SERVICE_BASE_URI = 'https://lilybotcommuteapi.azurewebsites.net/';

app.constant('appSettings', {
    authApiServiceBaseUri: AUTH_API_SERVICE_BASE_URI,
    resourceApiServiceBaseUri: RESOURCE_API_SERVICE_BASE_URI,
    clientId: 'lilybot.commute',
    vasttrafikKey: 'QUTQ96MIf4QvdZ0jG91UVEBLDGMa',
    vasttrafikSecret: '7iLamTRw_HxX_dzUFC3GKVkf8Pca',
    vasttrafikScope: '123',
    appendAuthTokenUrls: [RESOURCE_API_SERVICE_BASE_URI]
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);


