(function () {
    'use strict';

    angular
        .module('myApp.home')
        .controller('LoginCtrl', controller);

    controller.$inject = ['$scope', '$location', 'authService', 'appSettings'];

    function controller($scope, $location, authService, appSettings) {

        $scope.authExternalProvider = function (provider) {
			// Open the provider login form as a new window
            var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';
            var externalProviderUrl = appSettings.authApiServiceBaseUri +
                "api/Account/ExternalLogin?provider=" +
                provider +
                "&response_type=token&client_id=" +
                appSettings.clientId +
                "&redirect_uri=" +
                redirectUri;
            window.$windowScope = $scope;
            var oauthWindow = window.open(externalProviderUrl,
                "Logga in med " + provider,
                "location=0,status=0,centerscreen,width=600,height=750");
        };

        $scope.authCompletedCB = function (fragment) {

            $scope.$apply(function () {

                if (fragment.haslocalaccount == 'False') {

                    authService.logOut();

                    authService.externalAuthData = {
                        provider: fragment.provider,
                        userDisplayName: fragment.external_user_name,
                        externalAccessToken: fragment.external_access_token
                    };

                    $location.path('/associate');
                }
                else {
                    //Obtain access token and redirect to orders
                    var externalData = {
                        provider: fragment.provider,
                        externalAccessToken: fragment.external_access_token
                    };
                    authService.obtainAccessToken(externalData).then(function (response) {
                        $scope.authData.isAuthorized = authService.authentication.isAuth;
                    },
                 function (err) {
                     $scope.message = err.error_description;
                 });
                }
            });
        }
    }
})();