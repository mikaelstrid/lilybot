'use strict';

angular.module('myApp.associate', ['ngRoute'])
    .config([
        '$routeProvider', function($routeProvider) {
            $routeProvider.when('/associate',
            {
                templateUrl: 'app/components/associate/associateView.html',
                controller: 'AssociateCtrl'
            });
        }
    ]);