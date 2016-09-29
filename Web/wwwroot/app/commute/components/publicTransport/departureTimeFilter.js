angular.module('myApp.commute.publicTransport')
.filter('departureTime', function() {
    return function (input) {
        if (input > 0)
            return 'Om ' + input + ' min';
        else
            return 'Nu';
  };
});

