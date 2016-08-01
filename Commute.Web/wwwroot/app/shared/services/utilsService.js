'use strict';
app.factory('utilsService', [function () {

    var service = {
        getMinutesFromMilliseconds: getMinutesFromMilliseconds,
        computeDistanceBetween: computeDistanceBetween
    };

    return service;


    // === FUNCTIONS ===

    function getMinutesFromMilliseconds(milliseconds) {
        return Math.floor(milliseconds / (60 * 1000));
    }

    function computeDistanceBetween(from, to) {
        return google.maps.geometry.spherical.computeDistanceBetween(from, to);
    }
}
]);