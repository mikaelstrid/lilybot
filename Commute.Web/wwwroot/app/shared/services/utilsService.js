'use strict';
app.factory('utilsService', [function () {

    var service = {
        getMinutesFromMilliseconds: getMinutesFromMilliseconds,
        distanceTo: distanceTo
    };

    return service;


    // === FUNCTIONS ===

    function getMinutesFromMilliseconds(milliseconds) {
        return Math.floor(milliseconds / (60 * 1000));
    }

    function distanceTo(from, to) {
        return google.maps.geometry.spherical.computeDistanceBetween(from, to);
    }
}
]);