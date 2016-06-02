//angular.module('myApp.stores').factory('storesService', ['$http', 'appSettings', function ($http, appSettings) {

//    var serviceBase = appSettings.resourceApiServiceBaseUri;

//    var storesServiceFactory = {};

//    function getAll() {
//        return $http.get(serviceBase + 'api/stores').then(function (results) {
//            return results;
//        });
//    }

//    function add(name) {
//        return $http.post(
//                serviceBase + 'api/stores',
//                { name: name })
//            .then(function (results) {
//                return results;
//            });
//    }

//    storesServiceFactory.getAll = getAll;
//    storesServiceFactory.add = add;
//    return storesServiceFactory;
//}]);

(function () {
    'use strict';

    angular
        .module('myApp.stores')
        .factory('storesService', factory);

    factory.$inject = ['$http', 'appSettings'];

    function factory($http, appSettings) {
        var serviceBase = appSettings.resourceApiServiceBaseUri;

        var service = {
            getAll: getAll,
            add :add
        };

        return service;



        function getAll() {
            return $http.get(serviceBase + 'api/stores');
        }

        function add(name) {
            return $http.post(serviceBase + 'api/stores', { name: name });
        }
    }
})();