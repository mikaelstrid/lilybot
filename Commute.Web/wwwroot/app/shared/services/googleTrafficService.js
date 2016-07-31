'use strict';
app.factory('googleTrafficService', ['$http', '$log', '$q', 'appSettings', function ($http, $log, $q, appSettings) {

    var homeLatLng = new google.maps.LatLng(57.5697346, 12.075034);
    var workLatLng = new google.maps.LatLng(57.7083954, 11.9653797);

    var service = {
        getCarRouteAlternatives: getCarRouteAlternatives
    };

    return service;


    // === HELPERS ===


    // === FUNCTIONS ===
    function createTripData(response) {
        return _.map(response, function (route) {
            //var now = new Date().getTime();

            return {
                type: 'car',
                name: 'Älvborgsbron',
                plannedDurationInMinutes: 10,
                expectedDurationInMinutes: 12,
                isDelayed: false
            }
        });
    }


    function getCarRouteAlternatives(currentPosition) {
        var deferred = $q.defer();

        window.setTimeout(function() {
                deferred.resolve(createTripData([1,2]));
            },
            500);

        return deferred.promise;
    }
}
]);