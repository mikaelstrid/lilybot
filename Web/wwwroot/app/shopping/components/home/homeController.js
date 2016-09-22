(function () {
    'use strict';

    angular
        .module('myApp.shopping.home')
        .controller('ShoppingHomeCtrl', controller);

    controller.$inject = ['$location', '$mdSidenav'];

    function controller($location, $mdSidenav) {
        var vm = this;

        vm.goto = function (page) {
            $location.path('/' + page);
        }

        activate();

        function activate() {
        }
    }
})();
