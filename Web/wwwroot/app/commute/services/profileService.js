(function () {
    'use strict';

    angular
        .module('myApp.commute')
        .factory('profileService', factory);

    factory.$inject = ['$http', '$q', '$log', 'appSettings'];

    function factory($http, $q, $log, appSettings) {

        var serviceBase = appSettings.commuteApiServiceBaseUri;
        var pathBase = 'api/profiles';

        var service = {
            me: null,
            getMyProfile: getMyProfile
        };

        return service;


        function getMyProfile() {
            var deferred = $q.defer();
            $http.get(serviceBase + pathBase + "/me")
                .then(
                    function(response) {
                        service.me = createProfile(response.data);
                        $log.log('Profile received', JSON.stringify(service.me));
                        deferred.resolve();
                    },
                    function(error) {
                        deferred.reject(error);
                    });
            return deferred.promise;
        }

        function createProfile(response) {
            return {
                homeLocation: new google.maps.LatLng(response.homeLocationLatitude, response.homeLocationLongitude),
                workLocation: new google.maps.LatLng(response.workLocationLatitude, response.workLocationLongitude),
                homePublicTransportStationId: response.homePublicTransportStationId,
                workPublicTransportStationId: response.workPublicTransportStationId,
                primaryWayOfCommuting: response.primaryWayOfCommuting
            };
        }

    }
})();

