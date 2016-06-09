'use strict';
app.factory('itemsService', ['$http', 'appSettings', function($http, appSettings) {

        var serviceBase = appSettings.resourceApiServiceBaseUri;
        var pathBase = 'api/items';

        var service = {
            getActive: getActive,
            add: add,
            reAdd: reAdd,
            remove: remove,
            markItemAsDone: markItemAsDone,
            setComment: setComment
        };

        return service;


        function getActive() {
            return $http.get(serviceBase + pathBase + '/active');
        }

        function add(productId) {
            return $http.post(serviceBase + pathBase, { productId: productId });
        }

        function reAdd(itemId) {
            return $http.post(serviceBase + pathBase + '/readd/' + itemId);
        }

        function remove(id) {
            return $http.delete(serviceBase + pathBase + '/' + id);
        }

        function markItemAsDone(id) {
            return $http.put(serviceBase + pathBase + '/markasdone/' + id);
        }

        function setComment(id, comment) {
            return $http.put(serviceBase + pathBase + '/comment/' + id, { comment: comment });
        }
    }
]);