(function () {
    'use strict';

    angular
        .module('myApp.products')
        .controller('ProductsCtrl', controller);

    controller.$inject = ['$scope', '$location', 'productsService', '$mdDialog', '$mdToast', '$mdSidenav'];

    function controller($scope, $location, productsService, $mdDialog, $mdToast, $mdSidenav) {

        $scope.isLoading = true;
        $scope.isWorking = false;
        $scope.products = [];
        $scope.selectedProduct = null;
        $scope.selectedProductEditModel = null;

        $scope.showAddNewDialog = function (ev) {
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
                    .then(function (result) {
                        $scope.products.push(result.data);
                    },
                        function (error) {
                            showError('Lyckades inte spara den nya produkten. :(', 'add', error);
                        })
                    .finally(function () {
                        $scope.isWorking = false;
                    });
            });
        };

        $scope.onRemoveProduct = function (product, index) {
            var confirm = $mdDialog.confirm()
                  .title('Vill du verkligen ta bort ' + product.name + '?')
                  .textContent('Det går inte att ångra sig...')
                  .ariaLabel('Ta bort produkt')
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function () {
                $scope.isWorking = true;
                productsService.remove(product.id)
                    .then(function () {
                    },
                        function (error) {
                            showError('Lyckades inte ta bort produkten. :(', 'remove', error);
                            $scope.products.splice(index, 0, product);
                        })
                    .finally(function () {
                        $scope.isWorking = false;
                    });
            }, function () {
                // User cancelled the remove, reinsert the product
                $scope.products.splice(index, 0, product);
            });
        };

        $scope.onProductClicked = function (product) {
            $scope.selectedProduct = product;
            $scope.selectedProductEditModel = { name: product.name };
            openSidenav();
        }

        $scope.onSaveProductClicked = function () {
            closeSidenav();
            $scope.isWorking = true;
            productsService.rename($scope.selectedProduct.id, $scope.selectedProductEditModel.name)
                .then(function (result) {
                    $scope.selectedProduct.name = $scope.selectedProductEditModel.name;
                },
                    function (error) {
                        showError('Lyckades inte spara produkten. :(', 'rename', error);
                    })
                .finally(function () {
                    $scope.isWorking = false;
                });
        }

        $scope.showDeleteDialog = function (ev) {
            var confirm = $mdDialog.confirm()
                  .title('Vill du verkligen ta bort ' + $scope.selectedProduct.name + '?')
                  .textContent('Det går inte att ångra sig...')
                  .ariaLabel('Ta bort produkt')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function () {
                $scope.isWorking = true;
                closeSidenav();
                productsService.remove($scope.selectedProduct.id)
                    .then(function () {
                        var index = $scope.products.indexOf($scope.selectedProduct);
                        $scope.products.splice(index, 1);
                        $scope.selectedProduct = null;
                    },
                        function (error) {
                            showError('Lyckades inte ta bort produkten. :(', 'remove', error);
                        })
                    .finally(function () {
                        $scope.isWorking = false;
                    });
            });
        };

        $scope.onCancelClicked = function () {
            closeSidenav();
        }




        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            console.log('Call to productsService.' + failedMethodName + ' failed: ' + error.statusText);
        }

        function openSidenav() {
            $mdSidenav('right').open();
        }

        function closeSidenav() {
            $mdSidenav('right').close();
        }

        activate();

        function activate() {
            productsService.getAll()
                .then(function (result) {
                    $scope.products = result.data;
                },
                    function (result) {
                        $mdToast.show(
                            $mdToast.simple()
                            .textContent('Lyckades inte hämta några produkter. :(')
                            .hideDelay(3000)
                        );
                        console.log('Call to productsService.getAll failed: ' + result.statusText);
                    })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
    }
})();