(function () {
    'use strict';

    angular
        .module('myApp.shopping.products')
        .controller('ProductsCtrl', controller);

    controller.$inject = ['$scope', '$log', '$location', 'productsService', '$mdDialog', '$mdToast', '$mdSidenav'];

    function controller($scope, $log, $location, productsService, $mdDialog, $mdToast, $mdSidenav) {
        var self = this;

        self.isLoading = true;
        self.isWorking = false;
        self.products = [];
        self.selectedProduct = null;
        self.selectedProductEditModel = { name: '', barcodes: [] };

        self.showAddNewDialog = function (ev) {
            var confirm = $mdDialog.prompt()
                  .title('Vad ska den nya produkten heta?')
                  .placeholder('fyll i ett namn')
                  .ariaLabel('Produktnamn')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function (dialogResult) {
                self.isWorking = true;
                productsService.add(dialogResult)
                    .then(function (result) {
                        self.products.push(result.data);
                    },
                        function (error) {
                            showError('Lyckades inte spara den nya produkten. :(', 'add', error);
                        })
                    .finally(function () {
                        self.isWorking = false;
                    });
            });
        };

        self.onRemoveProduct = function (product, index) {
            var confirm = $mdDialog.confirm()
                  .title('Vill du verkligen ta bort ' + product.name + '?')
                  .textContent('Det går inte att ångra sig...')
                  .ariaLabel('Ta bort produkt')
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function () {
                self.isWorking = true;
                productsService.remove(product.id)
                    .then(function () {
                    },
                        function (error) {
                            showError('Lyckades inte ta bort produkten. :(', 'remove', error);
                            self.products.splice(index, 0, product);
                        })
                    .finally(function () {
                        self.isWorking = false;
                    });
            }, function () {
                // User cancelled the remove, reinsert the product
                self.products.splice(index, 0, product);
            });
        };

        self.onProductClicked = function (product) {
            self.selectedProduct = product;
            self.selectedProductEditModel = { name: product.name, barcodes: angular.copy(product.barcodes) };
            openSidenav();
        }

        self.renameProduct = function(id, newName) {
            self.isWorking = true;
            productsService.rename(id, newName)
                .then(
                    function() {
                        self.selectedProduct.name = self.selectedProductEditModel.name;
                    },
                    function(error) {
                        showError('Lyckades inte spara produkten. :(', 'rename', error);
                    })
                .finally(function() {
                    self.isWorking = false;
                });
        }

        self.showDeleteDialog = function (ev) {
            var confirm = $mdDialog.confirm()
                  .title('Vill du verkligen ta bort ' + self.selectedProduct.name + '?')
                  .textContent('Det går inte att ångra sig...')
                  .ariaLabel('Ta bort produkt')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function () {
                self.isWorking = true;
                closeSidenav();
                productsService.remove(self.selectedProduct.id)
                    .then(function () {
                        var index = self.products.indexOf(self.selectedProduct);
                        self.products.splice(index, 1);
                        self.selectedProduct = null;
                    },
                        function (error) {
                            showError('Lyckades inte ta bort produkten. :(', 'remove', error);
                        })
                    .finally(function () {
                        self.isWorking = false;
                    });
            });
        };

        self.onCancelClicked = function () {
            closeSidenav();
        }

        self.onScanBarcodeClicked = function (ev) {
            document.querySelector('input[type=file]#fileInput').click();
        }

        self.decode = function (src) {
            var config = {
                // It worked better if not including the settings commented out below
                // inputStream: { singleChannel: false, size: 800, constraints: { facingMode: "environment" } }, 
                // locator: { patchSize: 'medium', halfSample: true },
                decoder: { readers: ['ean_reader'] },
                locate: true,
                //numOfWorkers: 4,
                src: URL.createObjectURL(src)
            }
            Quagga.decodeSingle(config, function (result) {
                if (result.codeResult)
                    $scope.$apply(function () {
                        self.addBarcodeToProduct(self.selectedProduct.id, result.codeResult.code);
                    });
                else
                    $mdToast.show($mdToast.simple().textContent('Kunde inte tolka streckkoden. :(').hideDelay(2000));
            });
        }

        self.addBarcodeToProduct = function(id, barcode) {
            self.isWorking = true;
            productsService.addBarcode(id, barcode)
                .then(
                    function () {
                        self.selectedProduct.barcodes.push(barcode);
                        self.selectedProductEditModel.barcodes.push(barcode);
                    },
                    function (error) {
                        showError('Lyckades inte lägga till streckkoden. :(', 'addBarcode', error);
                    })
                .finally(function () {
                    self.isWorking = false;
                });
        }

        self.onRemoveBarcodeClicked = function (barcode) {
            self.removeBarcodeFromProduct(self.selectedProduct.id, barcode);
        }

        self.removeBarcodeFromProduct = function(id, barcode) {
            self.isWorking = true;
            productsService.removeBarcode(id, barcode)
                .then(
                    function () {
                        _.remove(self.selectedProduct.barcodes, function (n) { return n === barcode; });
                        _.remove(self.selectedProductEditModel.barcodes, function (n) { return n === barcode; });
                    },
                    function (error) {
                        showError('Lyckades inte ta bort streckkoden. :(', 'removeBarcode', error);
                        self.selectedProduct.barcodes.push(barcode);
                        self.selectedProductEditModel.barcodes.push(barcode);
                    })
                .finally(function () {
                    self.isWorking = false;
                });
        }

        $scope.$watch(angular.bind(this, function() { return this.selectedProductEditModel.name; }),
            function (newVal, oldVal) {
                if (self.selectedProduct && oldVal === self.selectedProduct.name && oldVal !== newVal)
                    self.renameProduct(self.selectedProduct.id, newVal);
            });



        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
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
                    self.products = result.data;
                },
                    function (error) {
                        showError('Lyckades inte hämta några produkter. :(', 'getAll', error);
                    })
                .finally(function () {
                    self.isLoading = false;
                });
        }
    }
})();