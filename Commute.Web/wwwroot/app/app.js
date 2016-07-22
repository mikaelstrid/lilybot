'use strict';

var app = angular.module('myApp',
[
    'ngMaterial',
    'ngRoute',
    'LocalStorageModule',
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

app.constant('appSettings', {
    authApiServiceBaseUri: 'http://localhost:51350/',
    resourceApiServiceBaseUri: 'http://localhost:6001/',
    //authApiServiceBaseUri: 'http://lilybotauthapi.azurewebsites.net/',
    //resourceApiServiceBaseUri: 'http://lilybotcommuteapi.azurewebsites.net/',
    clientId: 'lilybot.commute'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);


