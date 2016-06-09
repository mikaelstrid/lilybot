(function () {
    'use strict';

    angular
        .module('myApp.shopping')
        .controller('ShoppingCtrl', controller);

    controller.$inject = ['$scope', '$location', 'storesService', 'itemsService', '$mdToast', '$q'];

    function controller($scope, $location, storesService, itemsService, $mdToast, $q) {

        $scope.isLoading = true;
        $scope.isWorking = false;
        $scope.stores = [];
        $scope.selectedStore = null;
        $scope.items = [];

        $scope.selectStore = function (store) {
            $scope.isWorking = true;
            if (!store.isInitialized) {
                _.forEach($scope.items, function(item) { addItemToStoreSection(store, item); });
                store.isInitialized = true;
            }
            $scope.selectedStore = store;
            $scope.isWorking = false;
        }

        $scope.clearSelectedStore = function() {
            $scope.selectedStore = null;
        }

        $scope.markItemAsDone = function(item) {
            $scope.isWorking = true;
            itemsService.markItemAsDone(item.id)
                .then(
                    function () {
                        _.forEach($scope.selectedStore.sections,
                            function (section) {
                                if (_.some(section.items, ['id', item.id])) {
                                    _.remove(section.items, ['id', item.id]);
                                    showUndoToast(item);
                                    return false;
                                }
                            });
                        _.remove($scope.items, ['id', item.id]);
                    },
                    function(error) {
                        showError('Lyckades inte markera varan som klar. :(', 'itemsService.markItemAsDone', error);
                    })
                .finally(function() {
                    $scope.isWorking = false;
                });
        }


        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            console.log('Call to ' + failedMethodName + ' failed: ' + error.statusText);
        }

        function showUndoToast(item) {
            var toast = $mdToast.simple()
                .textContent(item.productName + ' klar.')
                .action('Ångra')
                .highlightAction(true)
                .hideDelay(10000);
            $mdToast.show(toast)
                .then(function(response) {
                    if (response === 'ok') {
                        //:TODO: Detta verkar inte fungera
                        addItemToList(item.productId, item.productName);
                    }
                });
        }

        function addItemToList(productId, productName) {
            $scope.isWorking = true;
            itemsService.add(productId)
                .then(
                    function (result) {
                        var newItem = { id: result.data.id, productId: productId, productName: productName };
                        $scope.items.push(newItem);
                        addItemToStoreSection($scope.selectedStore, newItem);
                    },
                    function (error) {
                        showError('Lyckades inte lägga till produkten i inköpslistan. :(', 'itemsService.add', error);
                    })
                .finally(function () {
                    $scope.isWorking = false;
                });
        }

        function addItemToStoreSection(store, item) {
            _.forEach(store.sections,
                function(section) {
                    section.items = section.items || [];
                    if (_.some(section.productIds, function(i) { return i === item.productId; })) {
                        section.items.push(item);
                        return false;
                    }
                });
        }

        
        // === INIT ===
        function activate() {
            var getStoresPromise = storesService.getAll()
                .then(
                    function(result) {
                        $scope.stores = result.data;
                    },
                    function(error) {
                        showError('Lyckades inte hämta några butiker. :(', 'storesService.getAll', error);
                    });

            var getItemsPromise = itemsService.getActive()
                .then(
                    function(result) {
                        $scope.items = result.data;
                    },
                    function(error) {
                        showError('Lyckades inte hämta några varor. :(', 'itemsService.getActive', error);
                    });

            $q.all([getStoresPromise, getItemsPromise]).finally(function () { $scope.isLoading = false; });
        }

        activate();
    }
})();