angular.module('myApp.commute.publicTransport')
.filter('stopName', function() {
    return function (input) {
        var index = input.indexOf(',');
        return index !== -1 ? input.substring(0, index) : input;
    };
});

