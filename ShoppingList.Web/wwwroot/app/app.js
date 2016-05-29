'use strict';

angular.module('myApp',
    [
        'ngMaterial',
        'ngRoute',
        'myApp.viewHome',
        'myApp.viewLogin'
    ])
    .config([
        '$locationProvider', '$routeProvider', function($locationProvider, $routeProvider) {
            $locationProvider.hashPrefix('!');

            $routeProvider.otherwise({ redirectTo: '/home' });
        }
    ]);
