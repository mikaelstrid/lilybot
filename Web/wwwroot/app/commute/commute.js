'use strict';

angular.module('myApp.commute', [
    'ngRoute',
    'ngGeolocation',
    'ngMap',
    'myApp.commute.home',
    'myApp.commute.publicTransport',
    'myApp.commute.car'
])