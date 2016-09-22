(function () {
    'use strict';

    angular
        .module('myApp.shopping.home')
        .controller('ShoppingHomeCtrl', controller);

    controller.$inject = ['$scope', '$location', '$mdSidenav'];

    function controller($scope, $location, $mdSidenav) {

        $scope.goto = function (page) {
            $location.path('/' + page);
        }

        activate();

        function activate() {
        }
    }
})();
