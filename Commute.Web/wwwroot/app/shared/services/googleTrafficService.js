'use strict';
app.factory('googleTrafficService', ['$http', '$log', '$q', 'utilsService', 'appSettings', function ($http, $log, $q, utilsService, appSettings) {

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
            return {
                type: 'car',
                name: route.summary,
                plannedDurationInMinutes: utilsService.getMinutesFromMilliseconds(_.sumBy(route.legs, function(o) { return o.duration.value * 1000; })),
                expectedDurationInMinutes: utilsService.getMinutesFromMilliseconds(_.sumBy(route.legs, function (o) { return o.duration.value * 1000; })),
                isDelayed: false
            }
        });
    }


    function getCarRouteAlternatives(currentPosition) {
        var deferred = $q.defer();

        var directionsService = new google.maps.DirectionsService;
        directionsService.route(
            {
                origin: new google.maps.LatLng(currentPosition.latitude, currentPosition.longitude),
                destination: workLatLng,
                provideRouteAlternatives: true,
                travelMode: 'DRIVING',
                drivingOptions: {
                    departureTime: new Date(),
                    trafficModel: 'bestguess'
                }
            },
            callback);

        function callback(response, status) {
            if (status === 'OK')
                deferred.resolve(createTripData(response.routes));
            else
                deferred.reject(status);
        }
        
        return deferred.promise;
    }
}
]);