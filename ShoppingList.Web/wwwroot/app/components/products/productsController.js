(function () {
    'use strict';

    angular
        .module('myApp.products')
        .controller('ProductsCtrl', controller);

    controller.$inject = ['$scope', '$location', 'productsService', '$mdDialog', '$mdToast'];

    function controller($scope, $location, productsService, $mdDialog, $mdToast) {

        $scope.isLoading = true;
        $scope.isWorking = false;
        $scope.products = [];

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


        //$scope.gotoDetails = function (store) {
        //    $location.path('/stores/' + store.id);
        //}


        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            console.log('Call to productsService.' + failedMethodName + ' failed: ' + error.statusText);
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