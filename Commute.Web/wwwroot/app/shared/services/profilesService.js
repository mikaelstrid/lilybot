'use strict';
app.factory('profilesService', ['$http', 'appSettings', function ($http, appSettings) {

        var serviceBase = appSettings.resourceApiServiceBaseUri;
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