'use strict';

angular.module('myApp.commute', [
    'ngRoute',
    'ngGeolocation',
    'myApp.commute.home',
    'myApp.commute.publicTransport',
    'myApp.commute.car'
]);