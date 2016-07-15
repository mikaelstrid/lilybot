'use strict';
app.factory('profilesService', ['$http', 'appSettings', function ($http, appSettings) {

        var serviceBase = appSettings.resourceApiServiceBaseUri;
        var pathBase = 'api/profiles';

        var service = {
            create: create
        };

        return service;


        function create() {
            return $http.post(serviceBase + pathBase);
        }
    }
]);