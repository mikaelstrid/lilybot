'use strict';

angular.module('myApp.planning', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/planera', {
        templateUrl: 'app/components/planning/planningView.html',
        controller: 'PlanningCtrl',
        controllerAs: 'vm'
    });
}])