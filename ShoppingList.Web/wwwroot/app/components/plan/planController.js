(function () {
    'use strict';

    angular
        .module('myApp.plan')
        .controller('PlanCtrl', controller);

    controller.$inject = ['$scope', '$location', 'productsService', 'itemsService', '$mdDialog', '$mdToast', '$q'];

    function controller($scope, $location, productsService, itemsService, $mdDialog, $mdToast, $q) {

        $scope.isLoading = true;
        $scope.isWorking = false;
        $scope.products = [];
        $scope.items = [];

        $scope.addItemToList = function (product) {
            $scope.isWorking = true;
            itemsService.add(product.id)
                .then(
                    function(result) {
                        _.find($scope.products, function (p) { return p.id === product.id; }).hidden = true;
                        $scope.items.push({ id: result.data.id, productId: product.id, productName: product.name });
                    },
                    function(error) {
                        showError('Lyckades inte lägga till produkten i inköpslistan. :(', 'itemsService.add', error);
                    })
                .finally(function() {
                    $scope.isWorking = false;
                });
        }

        $scope.removeFromList = function(item) {
            $scope.isWorking = true;
            itemsService.remove(item.id)
                .then(
                    function() {
                        _.remove($scope.items, function(i) { return i.id === item.id; });
                        _.find($scope.products, function (p) { return p.id === item.productId; }).hidden = false;
                    },
                    function(error) {
                        showError('Lyckades inte ta bort varan från listan. :(', 'itemsService.remove', error);
                    })
                .finally(function() {
                    $scope.isWorking = false;
                });
        }

        $scope.showCommentDialog = function (ev, item) {
            var confirm = $mdDialog.prompt()
                .title('Fyll i en kommentar om varan.')
                .placeholder(item.comment)
                .ariaLabel('Kommentar')
                .targetEvent(ev)
                .ok('OK')
                .cancel('Avbryt');
            $mdDialog.show(confirm).then(function (dialogResult) {
                $scope.isWorking = true;
                itemsService.setComment(item.id, dialogResult)
                    .then(
                        function(result) {
                            item.comment = dialogResult;
                        },
                        function(error) {
                            showError('Lyckades inte spara kommentaren. :(', 'itemsService.setComment', error);
                        })
                    .finally(function() { $scope.isWorking = false; });
            });
        };
        
        $scope.showAddNewProductDialog = function (ev) {
            var confirm = $mdDialog.prompt()
                  .title('Vad ska den nya produkten heta?')
                  .placeholder('fyll i ett namn')
                  .ariaLabel('Produktnamn')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function (dialogResult) {
                $scope.isWorking = true;
                productsService.add(dialogResult)
                    .then(
                        function(result) {
                            $scope.products.push(result.data);
                        },
                        function(error) {
                            showError('Lyckades inte spara den nya produkten. :(', 'productsService.add', error);
                        })
                    .finally(function() {
                        $scope.isWorking = false;
                    });
            });
        };

        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            console.log('Call to ' + failedMethodName + ' failed: ' + error.statusText);
        }

        function populateProductsAndItems(products, items) {
            _(products).forEach(function(product) {
                var existingItem = _.find(items, function (i) { return i.productId === product.id; });
                product.hidden = !!existingItem;
                if (existingItem) {
                    existingItem.productName = product.name;
                    $scope.items.push(existingItem);
                    $scope.products.push(product);
                } else {
                    $scope.products.push(product);
                }
            });
        }


        
        // === INIT ===
        function activate() {
            var receivedProducts = null;
            var receivedItems = null;

            var getProductsPromise = productsService.getAll()
                .then(
                    function(result) {
                        receivedProducts = result.data;
                    },
                    function(error) {
                        showError('Lyckades inte hämta några produkter. :(', 'productsService.getAll', error);
                    });

            var getItemsPromise = itemsService.getActive()
                .then(
                    function(result) {
                        receivedItems = result.data;
                    },
                    function(error) {
                        showError('Lyckades inte hämta några varor. :(', 'itemsService.getActive', error);
                    });

            $q.all([getProductsPromise, getItemsPromise]).then(function () {
                if (receivedProducts && receivedItems)
                    populateProductsAndItems(receivedProducts, receivedItems);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        activate();
    }
})();