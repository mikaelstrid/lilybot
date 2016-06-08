'use strict';

angular.module('myApp.plan', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/plan', {
        templateUrl: 'app/components/plan/planView.html',
        controller: 'PlanCtrl'
    });
}])