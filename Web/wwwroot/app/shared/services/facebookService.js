(function () {
    'use strict';

    angular
        .module('myApp')
        .factory('facebookService', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var service = {
            getData: getData
        };

        return service;

        function getData() { }
    }
})();

