(function () {
    'use strict';

    angular
        .module('myApp.products')
        .factory('productsService', factory);

    factory.$inject = ['$http', 'appSettings'];

    function factory($http, appSettings) {
        var serviceBase = appSettings.resourceApiServiceBaseUri;
        var pathBase = 'api/products';

        var service = {
            getAll: getAll,
            getTop: getTop,
            add: add,
            rename: rename,
            remove: remove,
        };

        return service;



        function getAll() {
            return $http.get(serviceBase + pathBase);
        }

        function getTop(count) {
            return $http.get(serviceBase + pathBase + '/top/' + count);
        }

        function add(name) {
            return $http.post(serviceBase + pathBase, { name: name });
        }

        function rename(id, newName) {
            return $http.put(serviceBase + 'api/products/' + id, { name: newName });
        }

        function remove(id) {
            return $http.delete(serviceBase + pathBase + '/' + id);
        }
    }
})();