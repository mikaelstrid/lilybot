(function () {
    'use strict';

    angular
        .module('myApp.shopping.planning')
        .controller('CreateNewProductDialogController', controller);

    controller.$inject = ['$log', '$q', '$mdDialog', 'storesService', 'productsService'];

    function controller($log, $q, $mdDialog, storesService, productsService) {
        var self = this;

        self.newProduct = {
            name: '',
            storeSections: {}
        };
        self.stores = [];

        self.onCreateClicked = function () {
            if (self.newProduct.name) {
                createNewProduct(self.newProduct)
                    .then(
                        function(result) {
                            $mdDialog.hide({ data: result.data });
                        },
                        function() {
                            $mdDialog.cancel('Lyckades inte lägga till den nya produkten. :(');
                        }
                    );
            } else {
                $mdDialog.hide();
            }
        };
        self.onCancelClicked = function () {
            $mdDialog.hide();
        };

        self.storeHasSections = function (store) {
            return store.sections && store.sections.length > 0;
        };


        // === HELPERS ===
        function createNewProduct(newProduct) {
            return productsService.add(newProduct.name)
                .then(
                    function (newProductResult) {
                        // Associate the new product with the selected sections
                        var promises = [];
                        _.forOwn(newProduct.storeSections, function(value, key) {
                            promises.push(
                                storesService.moveProductToSection(key, newProductResult.data.id, value)
                                .then(
                                    function() {
                                        $log.log('OK', key, value);
                                    },
                                    function(error) {
                                        $log.log('Fail', key, value, error);
                                    })
                            );
                        });

                        return $q.all(promises)
                            .then(function() {
                                return newProductResult;
                            });
                    },
                    function(error) {
                        $log.log('Call to productsService.add failed: ' + error.statusText);
                    });
        }


        // === INIT ===
        function activate() {
            storesService.getAll()
                .then(
                    function(result) {
                        self.stores = result.data;
                    },
                    function(error) {
                        $log.log('Call to storesService.getAll failed: ' + error.statusText);
                        $mdDialog.cancel('Lyckades inte hämta några butiker. :(');
                    });
        }

        activate();
    }
})();