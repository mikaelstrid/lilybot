(function () {
    'use strict';

    angular
        .module('myApp.planning')
        .controller('PlanningCtrl', controller);

    controller.$inject = ['$location', 'productsService', 'itemsService', '$mdDialog', '$mdToast', '$q'];

    function controller($location, productsService, itemsService, $mdDialog, $mdToast, $q) {
        var self = this;

        self.isLoading = true;
        self.isWorking = false;
        self.products = [];
        self.items = [];

        self.searchText = null;
        self.selectedItem = null;

        self.querySearch = function (query) {
            return productsService.search(query)
                .then(function (result) {
                    return _.filter(result.data, function (p) {
                        console.log(p.Id);
                        return !_.some(self.items, ['productId', p.id]);
                    });
                });
        }

        self.autocompleteSelectedItemChanged = function (item) {
            if (!item) return;
            self.searchText = null;
            self.addItemToList(item);
        }

        self.addItemToList = function (product) {
            self.isWorking = true;
            itemsService.add(product.id)
                .then(
                    function(result) {
                        var productInTop20 = _.find(self.products, function (p) { return p.id === product.id; });
                        if (productInTop20) productInTop20.hidden = true;
                        self.items.push({ id: result.data.id, productId: product.id, productName: product.name });
                    },
                    function(error) {
                        showError('Lyckades inte lägga till produkten i inköpslistan. :(', 'itemsService.add', error);
                    })
                .finally(function() {
                    self.isWorking = false;
                });
        }

        self.removeFromList = function(item) {
            self.isWorking = true;
            itemsService.remove(item.id)
                .then(
                    function() {
                        _.remove(self.items, function (i) { return i.id === item.id; });
                        var top20Product = _.find(self.products, function (p) { return p.id === item.productId; });
                        if (top20Product) top20Product.hidden = false;
                    },
                    function(error) {
                        showError('Lyckades inte ta bort varan från listan. :(', 'itemsService.remove', error);
                    })
                .finally(function() {
                    self.isWorking = false;
                });
        }

        self.showCommentDialog = function (ev, item) {
            var confirm = $mdDialog.prompt()
                .title('Fyll i en kommentar om varan.')
                .placeholder(item.comment)
                .ariaLabel('Kommentar')
                .targetEvent(ev)
                .ok('OK')
                .cancel('Avbryt');
            $mdDialog.show(confirm).then(function (dialogResult) {
                self.isWorking = true;
                itemsService.setComment(item.id, dialogResult)
                    .then(
                        function(result) {
                            item.comment = dialogResult;
                        },
                        function(error) {
                            showError('Lyckades inte spara kommentaren. :(', 'itemsService.setComment', error);
                        })
                    .finally(function() { self.isWorking = false; });
            });
        };
        
        self.showAddNewProductDialog = function (ev) {
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
                    .then(
                        function(result) {
                            self.products.push(result.data);
                        },
                        function(error) {
                            showError('Lyckades inte spara den nya produkten. :(', 'productsService.add', error);
                        })
                    .finally(function() {
                        self.isWorking = false;
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
            _(products)
                .forEach(function(product) {
                    var existingItem = _.find(items, function(i) { return i.productId === product.id; });
                    product.hidden = !!existingItem;
                    self.products.push(product);
                });

            _(items)
                .forEach(function(item) {
                    self.items.push(item);
                });
        }


        
        // === INIT ===
        function activate() {
            var receivedTopProducts = null;
            var receivedItems = null;

            var getProductsPromise = productsService.getTop(40)
                .then(
                    function(result) {
                        receivedTopProducts = result.data;
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
                if (receivedTopProducts && receivedItems)
                    populateProductsAndItems(receivedTopProducts, receivedItems);
            }).finally(function () {
                self.isLoading = false;
            });
        }

        activate();
    }
})();