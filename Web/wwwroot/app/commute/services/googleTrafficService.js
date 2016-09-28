'use strict';
app.factory('googleTrafficService',
[
    '$http', '$log', '$q', 'utilsService', 'appSettings', function($http, $log, $q, utilsService, appSettings) {

        var service = {
            getDestinationLatLng: getDestinationLatLng,
            getCarRouteAlternatives: getCarRouteAlternatives,
            lastUsedOrigin: null,
            lastUsedDestination: null
        };

        return service;


        // === HELPERS ===
        function getAddressPartBeforeColon(address) {
            var index = address.indexOf(',');
            return index !== -1 ? address.substring(0, index) : address;
        }


        // === FUNCTIONS ===

        function getDestinationLatLng(currentPosition, homeLatLng, workLatLng) {
            var currentPositionLatLng = new google.maps.LatLng(currentPosition.latitude, currentPosition.longitude);
            var distanceToHome = utilsService.computeDistanceBetween(currentPositionLatLng, homeLatLng);
            var distanceToWork = utilsService.computeDistanceBetween(currentPositionLatLng, workLatLng);
            return distanceToHome > distanceToWork ? homeLatLng : workLatLng;
        }

        function createTripData(response) {
            return _.map(response,
                function(route) {
                    return {
                        type: 'car',
                        name: route.summary,
                        distanceInKilometers: _.sumBy(route.legs, function(o) { return o.distance.value; }) / 1000,
                        plannedDurationInMinutes: utilsService
                            .getMinutesFromMilliseconds(_
                                .sumBy(route.legs, function(o) { return o.duration.value * 1000; })),
                        expectedDurationInMinutes: utilsService
                            .getMinutesFromMilliseconds(_
                                .sumBy(route.legs, function(o) { return o.duration.value * 1000; })),
                        startAddress: getAddressPartBeforeColon(route.legs[0].start_address),
                        endAddress: getAddressPartBeforeColon(route.legs[route.legs.length - 1].end_address)
                    }
                });
        }
        
        function getCarRouteAlternatives(currentPosition, destinationLatLng) {
            var deferred = $q.defer();

            var originLatLng = new google.maps.LatLng(currentPosition.latitude, currentPosition.longitude);

            var directionsService = new google.maps.DirectionsService;
            directionsService.route(
                {
                    origin: originLatLng,
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
                if (status === 'OK') {
                    service.lastUsedOrigin = originLatLng;
                    service.lastUsedDestination = destinationLatLng;
                    deferred.resolve(createTripData(response.routes));
                } else {
                    service.lastUsedOrigin = null;
                    service.lastUsedDestination = null;
                    deferred.reject(status);
                }
            }

            return deferred.promise;
        }
    }
]);