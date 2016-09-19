(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('HomeCtrl', controller);

    controller.$inject = ['$location', '$mdToast', 'authenticationService'];

    function controller($location, $mdToast, authenticationService) {
        /* jshint validthis:true */
        var vm = this;

        vm.user = {
            isAuthorized: false
        };

        vm.goto = function (page) {
            $location.path('/' + page);
        }

        vm.logout = function() {
            authenticationService.logOut();
            vm.user.isAuthorized = authenticationService.userData.isAuthorized;
        }

        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
                console.log('Call to storesService.' + failedMethodName + ' failed: ' + error.statusText);
            }
        }

        activate();

        function activate() {
            authenticationService.checkToken()
                .then(
                    function () {
                        vm.user.isAuthorized = authenticationService.userData.isAuthorized;
                    },
                    function(err) {
                        //$scope.message = err.error_description;
                    });
        }
    }
})();
