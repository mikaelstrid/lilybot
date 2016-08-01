'use strict';
app.factory('googleTrafficService', ['$http', '$log', '$q', 'utilsService', 'appSettings', function ($http, $log, $q, utilsService, appSettings) {

    var homeLatLng = new google.maps.LatLng(57.5697346, 12.075034);
    var workLatLng = new google.maps.LatLng(57.7083954, 11.9653797);

    var service = {
        getCarRouteAlternatives: getCarRouteAlternatives
    };

    return service;


    // === HELPERS ===
    function getAddressPartBeforeColon(address) {
        var index = address.indexOf(',');
        return index !== -1 ? address.substring(0, index) : address;
    }



    // === FUNCTIONS ===

    function getDestinationLatLng(currentPosition) {
        var currentPositionLatLng = new google.maps.LatLng(currentPosition.latitude, currentPosition.longitude);
        var distanceToHome = utilsService.computeDistanceBetween(currentPositionLatLng, homeLatLng);
        var distanceToWork = utilsService.computeDistanceBetween(currentPositionLatLng, workLatLng);
        return distanceToHome > distanceToWork ? homeLatLng : workLatLng;
    }
    
    function createTripData(response) {
        return _.map(response, function (route) {
            return {
                type: 'car',
                name: route.summary,
                distanceInKilometers: _.sumBy(route.legs, function(o) { return o.distance.value; }) / 1000,
                plannedDurationInMinutes: utilsService.getMinutesFromMilliseconds(_.sumBy(route.legs, function(o) { return o.duration.value * 1000; })),
                expectedDurationInMinutes: utilsService.getMinutesFromMilliseconds(_.sumBy(route.legs, function (o) { return o.duration.value * 1000; })),
                startAddress: getAddressPartBeforeColon(route.legs[0].start_address),
                endAddress: getAddressPartBeforeColon(route.legs[route.legs.length-1].end_address)
            }
        });
    }


    function getCarRouteAlternatives(currentPosition) {
        var deferred = $q.defer();

        var destinationLatLng = getDestinationLatLng(currentPosition);

        var directionsService = new google.maps.DirectionsService;
        directionsService.route(
            {
                origin: new google.maps.LatLng(currentPosition.latitude, currentPosition.longitude),
                destination: destinationLatLng,
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