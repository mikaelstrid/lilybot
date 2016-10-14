(function () {
    'use strict';

    angular
        .module('myApp.commute.car')
        .controller('CarCtrl', controller);

    controller.$inject = ['$log', '$location', '$mdDialog', '$mdToast', '$geolocation', 'googleTrafficService', 'profileService'];

    function controller($log, $location, $mdDialog, $mdToast, $geolocation, googleTrafficService, profileService) {
        var vm = this;
        vm.isLoading = false;
        vm.carRouteAlternatives = [];
        vm.lastUpdateTime = null;

        vm.getCarRouteAlternatives = function () {
            vm.isLoading = true;
            var me = profileService.me;
            $geolocation.getCurrentPosition({
                timeout: 30000
            }).then(function (position) {
                var destinationLatLng = googleTrafficService.getDestinationLatLng(position.coords, me.homeLocation, me.workLocation);
                googleTrafficService.getCarRouteAlternatives(position.coords, destinationLatLng)
                    .then(
                        function (routes) {
                            vm.carRouteAlternatives = routes;
                            vm.lastUpdateTime = new Date();
                        },
                        function (reason) { showError(reason + ' :(', 'googleTrafficService.getCarRouteAlternatives', null); }
                    )
                    .finally(function () { vm.isLoading = false; });
            }, function () {
                showError('Kunde inte bestämma din position :(', '$geolocation.getCurrentPosition', null);
                vm.isLoading = false;
            });
        }

        vm.refresh = function() {
            vm.getCarRouteAlternatives();
        }

        vm.goto = function (page) {
            $location.path('/' + page);
        }

        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (!error || error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            $log.log('Call to ' + failedMethodName + ' failed: ' + error.statusText);
        }
        
        activate();

        function activate() {
            if (profileService.me) {
                vm.getCarRouteAlternatives();
            } else {
                vm.isLoading = true;
                profileService.getMyProfile()
                    .then(
                        function () { vm.getCarRouteAlternatives(); },
                        function(response) {
                            showError('Kunde inte hämta din profil :(', 'profilesService.getMyProfile', response)
                        }
                    );
            }
        }
    }
})();