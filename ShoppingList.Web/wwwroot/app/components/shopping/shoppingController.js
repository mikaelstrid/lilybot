(function () {
    'use strict';

    angular
        .module('myApp.shopping')
        .controller('ShoppingCtrl', controller);

    controller.$inject = ['$scope', '$location', '$routeParams', 'storesService', 'itemsService', '$mdToast', '$q'];

    function controller($scope, $location, $routeParams, storesService, itemsService, $mdToast, $q) {

        $scope.isLoading = true;
        $scope.isWorking = false;
        $scope.selectedStore = null;
        $scope.items = [];

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
                        reAddItemToList(item);
                    }
                });
        }

        function reAddItemToList(item) {
            $scope.isWorking = true;
            itemsService.reAdd(item.id)
                .then(
                    function () {
                        $scope.items.push(item);
                        addItemToStoreSection(item);
                    },
                    function (error) {
                        showError('Lyckades inte lägga tillbaka varan i inköpslistan. :(', 'itemsService.reAdd', error);
                    })
                .finally(function () {
                    $scope.isWorking = false;
                });
        }

        function addItemToStoreSection(item) {
            _.forEach($scope.selectedStore.sections,
                function(section) {
                    section.items = section.items || [];
                    if (_.some(section.products, ['id', item.productId])) {
                        section.items.push(item);
                        return false;
                    }
                });
        }

        
        // === INIT ===
        function activate() {
            var getStorePromise = storesService.get($routeParams.id)
                .then(
                    function(result) {
                        $scope.selectedStore = result.data;
                    },
                    function(error) {
                        showError('Lyckades inte hämta butiken. :(', 'storesService.get', error);
                    });

            var getItemsPromise = itemsService.getActive()
                .then(
                    function(result) {
                        $scope.items = result.data;
                    },
                    function(error) {
                        showError('Lyckades inte hämta några varor. :(', 'itemsService.getActive', error);
                    });

            $q.all([getStorePromise, getItemsPromise])
                .then(function () {
                    if ($scope.selectedStore) {
                        _.forEach($scope.items, function(item) {
                                addItemToStoreSection(item);
                            });
                    }
                })
                .finally(function () { $scope.isLoading = false; });
        }

        activate();
    }
})();