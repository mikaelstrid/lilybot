(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('LoginCtrl', controller);

    controller.$inject = ['$scope', '$location', '$log', 'authenticationService', 'appSettings'];

    function controller($scope, $location, $log, authenticationService, appSettings) {
        var vm = this;

        vm.loginUsingExternalProvider = function (provider) {
            var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';
            var externalProviderUrl =
                appSettings.authApiServiceBaseUri +
                "api/login?provider=" + provider +
                "&response_type=token&client_id=" + appSettings.clientId +
                "&redirect_uri=" + redirectUri;
            window.$windowScope = $scope;
            var oauthWindow = window.open(
                externalProviderUrl,
                "Logga in med " + provider,
                "location=0,status=0,centerscreen,width=600,height=750");
        };

        vm.authenticationCompletedCallback = function (fragment) {
            $scope.$apply(function () {
                authenticationService.userData.externalProvider = fragment.provider;
                authenticationService.userData.externalDisplayName = fragment.external_user_name;
                authenticationService.userData.externalAccessToken = fragment.external_access_token;
                authenticationService.saveUserData();

                if (fragment.haslocalaccount === 'False') {
                    $location.path('/skapa-konto');
                } else {
                    authenticationService.obtainAccessToken(fragment.provider, fragment.external_access_token)
                        .then(
                            function() {
                            },
                            function(err) {
                                $log.log(err.error_description);
                            })
                        .finally(
                            function() {
                                $scope.$parent.vm.user.isAuthorized = authenticationService.userData.isAuthorized;
                            });
                }
            });
        }
    }
})();