'use strict';
app.factory('vasttrafikService', ['$http', '$log', '$q', 'localStorageService', 'appSettings', function ($http, $log, $q, localStorageService, appSettings) {

    var lindomeStationId = '9021014012711000';
    var gotebordCId = '9021014008000000';

    var service = {
        getUpcomingTrips: getUpcomingTrips
    };

    return service;


    // === HELPERS ===
    function getMinutes(milliseconds) {
        return Math.floor(milliseconds / (60 * 1000));
    }


    // === FUNCTIONS ===

    function getAccessToken() {
        var localStorageKey = 'vasttrafikAuthorizationData';
        var deferred = $q.defer();

        // Get any existing access tokens from local storage
        var authData = localStorageService.get(localStorageKey);

        // If no access token or if has expired, request a new one and save it to local storage
        if (!authData || authData.expirationTime < new Date().getTime()) {
            requestNewAccessToken()
                .then(function (response) {
                        localStorageService.set(localStorageKey,
                        {
                            expirationTime: new Date().getTime() + response.data.expires_in * 1000,
                            accessToken: response.data.access_token
                        });
                        deferred.resolve(response.data.access_token);
                    },
                    function (response) {
                        $log.log('Could not get new access token from Västtrafik', response);
                        deferred.reject();
                    });
        } else {
            deferred.resolve(authData.accessToken);
        }

        return deferred.promise;
    }

    function requestNewAccessToken() {
        return $http.post('https://api.vasttrafik.se/token', 
            'grant_type=client_credentials&scope=' + appSettings.vasttrafikScope,
            {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'Authorization': 'Bearer ' + btoa(appSettings.vasttrafikKey + ':' + appSettings.vasttrafikSecret)
                }
            });
    }

    function createTripData(response) {
        return _.map(response.data.TripList.Trip, function (trip) {
            var plannedOriginTime = Date.parse(trip.Leg.Origin.date + ' ' + trip.Leg.Origin.time);
            var plannedDestinationTime = Date.parse(trip.Leg.Destination.date + ' ' + trip.Leg.Destination.time);
            var expectedOriginTime = Date.parse(trip.Leg.Origin.rtDate + ' ' + trip.Leg.Origin.rtTime);
            var expectedDestinationTime = Date.parse(trip.Leg.Destination.rtDate + ' ' + trip.Leg.Destination.rtTime);

            var now = new Date().getTime();

            return {
                type: 'train',
                plannedDurationInMinutes: getMinutes(plannedDestinationTime - plannedOriginTime),
                expectedDurationInMinutes: getMinutes(expectedDestinationTime - expectedOriginTime),
                plannedDepartsInMinutes: getMinutes(plannedOriginTime - now),
                expectedDepartsInMinutes: getMinutes(expectedOriginTime - now),
                isDelayed: trip.Leg.Origin.time !== trip.Leg.Origin.rtTime || trip.Leg.Destination.time !== trip.Leg.Destination.rtTime,
                origin: {
                    name: trip.Leg.Origin.name,
                    planned: {
                        date: trip.Leg.Origin.date,
                        time: trip.Leg.Origin.time
                    },
                    expected: {
                        date: trip.Leg.Origin.rtDate,
                        time: trip.Leg.Origin.rtTime
                    }
                },
                destination: {
                    name: trip.Leg.Destination.name,
                    planned: {
                        date: trip.Leg.Destination.date,
                        time: trip.Leg.Destination.time,
                    },
                    expected: {
                        date: trip.Leg.Destination.rtDate,
                        time: trip.Leg.Destination.rtTime
                    }
                }

            }
        });
    }
    
    function getUpcomingTrips() {
        var deferred = $q.defer();

        getAccessToken()
            .then(
                function(accessToken) {
                    var url = 'https://api.vasttrafik.se/bin/rest.exe/v2/trip?originId=' +lindomeStationId + '&destId=' + gotebordCId + '&format=json';
                    $http.get(url, { headers: { 'Authorization': 'Bearer ' + accessToken } })
                        .then(
                            function(response) {
                                deferred.resolve(createTripData(response));
                            },
                            function(response) {
                                $log.log('Error when getting trip data.', response);
                                deferred.reject('Kunde inte hämta data från Västtrafik.');
                            });
                },
                function() {
                    deferred.reject('Kunde inte hämta data från Västtrafik.');
                });

        return deferred.promise;
    }
}
]);