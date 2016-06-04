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
            //get: get,
            add: add,
            //rename: rename,
            remove: remove,
            //addNewSection: addNewSection,
            //renameSection: renameSection,
            //removeSection: removeSection,
            //moveSectionUp: moveSectionUp,
            //moveSectionDown: moveSectionDown
        };

        return service;



        function getAll() {
            return $http.get(serviceBase + pathBase);
        }

        //function get(id) {
        //    return $http.get(serviceBase + 'api/stores/' + id);
        //}

        function add(name) {
            return $http.post(serviceBase + pathBase, { name: name });
        }

        //function rename(id, newName) {
        //    return $http.put(serviceBase + 'api/stores/' + id, { name: newName });
        //}

        function remove(id) {
            return $http.delete(serviceBase + pathBase + '/' + id);
        }


        //function addNewSection(storeId, sectionName) {
        //    return $http.post(serviceBase + 'api/stores/' + storeId + '/sections', { name: sectionName });
        //}

        //function renameSection(storeId, sectionId, newSectionName) {
        //    return $http.put(serviceBase + 'api/stores/' + storeId + '/sections/' + sectionId, { name: newSectionName });
        //}

        //function removeSection(storeId, sectionId) {
        //    return $http.delete(serviceBase + 'api/stores/' + storeId + '/sections/' + sectionId);
        //}

        //function moveSectionUp(storeId, sectionId) {
        //    return $http.put(serviceBase + 'api/stores/' + storeId + '/sections/movesectionup/' + sectionId);
        //}

        //function moveSectionDown(storeId, sectionId) {
        //    return $http.put(serviceBase + 'api/stores/' + storeId + '/sections/movesectiondown/' + sectionId);
        //}
    }
})();