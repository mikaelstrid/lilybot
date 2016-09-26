'use strict';

angular.module('myApp.commute.publicTransport', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/pendla/kollektivt', {
        templateUrl: 'app/commute/components/publicTransport/publicTransportView.html',
        controller: 'PublicTransportCtrl',
        controllerAs: 'vm'
    });
}])