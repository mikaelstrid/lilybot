(function () {
    'use strict';

    angular
        .module('myApp.commute.car')
        .controller('DirectionsMapCtrl', controller);

    controller.$inject = ['$log', '$routeParams', '$location', '$timeout', '$mdDialog', '$mdToast', '$geolocation', 'NgMap', 'googleTrafficService', 'profileService'];

    function controller($log, $routeParams, $location, $timeout, $mdDialog, $mdToast, $geolocation, NgMap, googleTrafficService, profileService) {
        var vm = this;
        vm.isLoading = true;

        NgMap.getMap().then(function (map) {
            $timeout(function() {
                    map.directionsRenderers[0].setRouteIndex(vm.routeIndex);
                    vm.isLoading = false;
                },
                1000);
        });

        activate();

        function activate() {
            vm.origin = googleTrafficService.lastUsedOrigin;
            vm.destination = googleTrafficService.lastUsedDestination;
            vm.routeIndex = parseInt($routeParams.id);
        }
    }
})();