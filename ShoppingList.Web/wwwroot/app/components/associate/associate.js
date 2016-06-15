'use strict';

angular.module('myApp.associate', ['ngRoute'])
    .config([
        '$routeProvider', function($routeProvider) {
            $routeProvider.when('/skapa-konto',
            {
                templateUrl: 'app/components/associate/associateView.html',
                controller: 'AssociateCtrl'
            });
        }
    ]);