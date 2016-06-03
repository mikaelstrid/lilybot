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
            get: get,
            add: add,
            rename: rename,
            remove: remove,
            addNewSection: addNewSection,
            removeSection: removeSection,
            moveSectionUp: moveSectionUp,
            moveSectionDown: moveSectionDown
        };

        return service;



        function getAll() {
            return $http.get(serviceBase + 'api/stores');
        }

        function get(id) {
            return $http.get(serviceBase + 'api/stores/' + id);
        }

        function add(name) {
            return $http.post(serviceBase + 'api/stores', { name: name });
        }

        function rename(id, newName) {
            return $http.put(serviceBase + 'api/stores/' + id, { name: newName });
        }

        function remove(id) {
            return $http.delete(serviceBase + 'api/stores/' + id);
        }


        function addNewSection(storeId, sectionName) {
            return $http.post(serviceBase + 'api/stores/' + storeId + '/sections', { name: sectionName });
        }

        function removeSection(storeId, sectionId) {
            return $http.delete(serviceBase + 'api/stores/' + storeId + '/sections/' + sectionId);
        }

        function moveSectionUp(storeId, sectionId) {
            return $http.put(serviceBase + 'api/stores/' + storeId + '/movesectionup/' + sectionId);
        }

        function moveSectionDown(storeId, sectionId) {
            return $http.put(serviceBase + 'api/stores/' + storeId + '/movesectiondown/' + sectionId);
        }
    }
})();