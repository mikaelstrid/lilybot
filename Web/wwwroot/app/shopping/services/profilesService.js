(function () {
    'use strict';

    angular
        .module('myApp.shopping')
        .factory('profilesService', factory);

    factory.$inject = ['$http', 'appSettings'];

    function factory($http, appSettings) {

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
})();
