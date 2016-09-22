'use strict';

angular.module('myApp.shopping.planning', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/handla/planera', {
        templateUrl: 'app/shopping/components/planning/planningView.html',
        controller: 'PlanningCtrl',
        controllerAs: 'vm'
    });
}])