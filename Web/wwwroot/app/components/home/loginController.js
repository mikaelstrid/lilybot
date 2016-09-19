(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('LoginCtrl', controller);

    controller.$inject = ['$scope', '$location', '$log', 'authenticationService', 'appSettings'];

    function controller($scope, $location, $log, authenticationService, appSettings) {

        $scope.loginUsingExternalProvider = function (provider) {
            // Open the provider login form as a new window
            var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';
            var externalProviderUrl = appSettings.authApiServiceBaseUri +
                "api/Account/ExternalLogin?provider=" + provider +
                "&response_type=token&client_id=" + appSettings.clientId +
                "&redirect_uri=" + redirectUri;
            window.$windowScope = $scope;
            var oauthWindow = window.open(externalProviderUrl,
                "Logga in med " + provider,
                "location=0,status=0,centerscreen,width=600,height=750");
        };

        $scope.authenticationCompletedCallback = function (fragment) {
            $scope.$apply(function () {
                var externalAuthData = {
                    provider: fragment.provider,
                    userName: fragment.external_user_name,
                    accessToken: fragment.external_access_token
                };
                authenticationService.updateExternalAuthData(externalAuthData);

                if (fragment.haslocalaccount === 'False') {
                    $location.path('/skapa-konto');
                } else {
                    authenticationService.obtainAccessToken(externalAuthData)
                        .then(
                            function(response) {
                                $scope.userData.isAuthorized = authenticationService.userData.isAuthorized; // Parent scope
                            },
                            function(err) {
                                $log.log(err.error_description);
                            });
                }
            });
        }
    }
})();