'use strict';
app.factory('profilesService',
[
    '$http', 'appSettings', function($http, appSettings) {

        var serviceBase = appSettings.commuteApiServiceBaseUri;
        var pathBase = 'api/profiles';

        var service = {
            getMyProfile: getMyProfile
        };

        return service;


        function getMyProfile() {
            return $http.get(serviceBase + pathBase + "/me");
        }
    }
]);