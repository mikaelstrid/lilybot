'use strict';

angular.module('myApp.createAccount', ['ngRoute'])
    .config([
        '$routeProvider', function($routeProvider) {
            $routeProvider.when('/skapa-konto',
            {
                templateUrl: 'app/core/components/createAccount/createAccountView.html',
                controller: 'CreateAccountCtrl',
                controllerAs: 'vm'
            });
        }
    ]);