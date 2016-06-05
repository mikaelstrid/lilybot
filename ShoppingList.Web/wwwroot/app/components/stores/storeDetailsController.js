(function () {
    'use strict';

    angular
        .module('myApp.stores')
        .controller('StoreDetailsCtrl', controller);

    controller.$inject = ['$scope', '$routeParams', '$location', 'storesService', 'productsService', '$mdDialog', '$mdToast'];

    function controller($scope, $routeParams, $location, storesService, productsService, $mdDialog, $mdToast) {

        $scope.isLoading = true;
        $scope.isWorking = false;
        $scope.store = null;
        $scope.newProductsSection = { id: null, name: "Nya produkter", products: [] }; // "Same schema as the sections"

        $scope.showRenameDialog = function (ev) {
            var prompt = $mdDialog.prompt()
                  .title('Vad vill du att butiken ska heta istället?')
                  .placeholder($scope.store.name)
                  .ariaLabel('Ändra butiksnamn')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(prompt).then(function (dialogResult) {
                $scope.isWorking = true;
                storesService.rename($scope.store.id, dialogResult)
                    .then(function () {
                        $scope.store.name = dialogResult;
                    },
                        function (error) {
                            showError('Lyckades inte ändra butiksnamnet. :(', 'rename', error);
                        })
                    .finally(function () {
                        $scope.isWorking = false;
                    });
            });
        };

        $scope.showDeleteDialog = function (ev) {
            var confirm = $mdDialog.confirm()
                  .title('Vill du verkligen ta bort ' + $scope.store.name + '?')
                  .textContent('Det går inte att ångra sig...')
                  .ariaLabel('Ta bort butik')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function () {
                $scope.isWorking = true;
                storesService.remove($scope.store.id)
                    .then(function () {
                        $location.path('/stores');
                    },
                        function (error) {
                            showError('Lyckades inte ta bort butiken. :(', 'remove', error);
                        })
                    .finally(function () {
                        $scope.isWorking = false;
                    });
            });
        };

        $scope.showAddNewSectionDialog = function (ev) {
            var prompt = $mdDialog.prompt()
                  .title('Vad ska den nya avdelningen heta?')
                  .placeholder('fyll i ett namn')
                  .ariaLabel('Lägg till avdelning')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(prompt).then(function (dialogResult) {
                $scope.isWorking = true;
                storesService.addNewSection($scope.store.id, dialogResult)
                    .then(function (result) {
                        $scope.store.sections.push(result.data);
                        updateCenterIndex();
                    },
                        function (error) {
                            showError('Lyckades inte spara den nya avdelningen. :(', 'addNewSection', error);
                        })
                    .finally(function () {
                        $scope.isWorking = false;
                    });
            });
        };

        $scope.showDeleteSectionDialog = function (ev, section) {
            var confirm = $mdDialog.confirm()
                  .title('Vill du verkligen ta bort ' + section.name + '?')
                  .textContent('Det går inte att ångra sig...')
                  .ariaLabel('Ta bort avdelning')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function () {
                $scope.isWorking = true;
                storesService.removeSection($scope.store.id, section.id)
                    .then(function () {
                        // Remove the section from the scope
                        var index = $scope.store.sections.indexOf(section);
                        $scope.store.sections.splice(index, 1);
                        updateCenterIndex();
                    },
                        function (error) {
                            showError('Lyckades inte ta bort avdelningen. :(', 'removeSection', error);
                        })
                    .finally(function () {
                        $scope.isWorking = false;
                    });
            });
        };

        $scope.showRenameSectionDialog = function (ev, section) {
            var confirm = $mdDialog.prompt()
                .title('Vad vill du att avdelningen ska heta istället?')
                .placeholder(section.name)
                .ariaLabel('Ändra namn på avdelning')
                .targetEvent(ev)
                .ok('OK')
                .cancel('Avbryt');
            $mdDialog.show(confirm)
                .then(function (dialogResult) {
                    $scope.isWorking = true;
                    storesService.renameSection($scope.store.id, section.id, dialogResult)
                        .then(function () {
                            section.name = dialogResult;
                        },
                            function (error) {
                                showError('Lyckades inte ändra namnet på avdelningen. :(', 'renameSection', error);
                            })
                        .finally(function () {
                            $scope.isWorking = false;
                        });
                });
        }

        $scope.moveSectionUp = function (section) {
            $scope.isWorking = true;
            storesService.moveSectionUp($scope.store.id, section.id)
                .then(function () {
                    var index = $scope.store.sections.indexOf(section);
                    if (index !== 0) {
                        $scope.store.sections.splice(index, 1);
                        $scope.store.sections.splice(index - 1, 0, section);
                        updateCenterIndex();
                    }
                },
                    function (error) {
                        showError('Lyckades inte flytta upp avdelningen. :(', 'moveSectionUp', error);
                    })
                .finally(function () {
                    $scope.isWorking = false;
                });
        }

        $scope.moveSectionDown = function (section) {
            $scope.isWorking = true;
            storesService.moveSectionDown($scope.store.id, section.id)
                .then(function () {
                    var index = $scope.store.sections.indexOf(section);
                    if (index !== ($scope.store.sections.length - 1)) {
                        $scope.store.sections.splice(index, 1);
                        $scope.store.sections.splice(index + 1, 0, section);
                        updateCenterIndex();
                    }
                },
                    function (error) {
                        showError('Lyckades inte flytta ner avdelningen. :(', 'moveSectionDown', error);
                    })
                .finally(function () {
                    $scope.isWorking = false;
                });
        }

        $scope.moveProductToSection = function (product, fromSection, toSection) {
            $scope.isWorking = true;

            storesService.moveProductToSection($scope.store.id, product.id, toSection.id)
                .then(function () {
                    var index = fromSection.products.indexOf(product);
                    if (index !== -1) {
                        fromSection.products.splice(index, 1);
                        toSection.products.push(product);
                    }
                },
                    function (error) {
                        showError('Lyckades inte flytta produkten. :(', 'moveProductToSection', error);
                    })
                .finally(function () {
                    $scope.isWorking = false;
                });
        }


        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            console.log('Call to storesService.' + failedMethodName + ' failed: ' + error.statusText);
        }

        function updateCenterIndex() {
            $scope.storeCenterIndex = Math.ceil($scope.store.sections.length / 2);
        }

        function populateProducts(products) {
            for (var i = 0; i < products.length; i++) {
                var product = products[i];
                var productFound = false;

                // Check the sections
                for (var j = 0; j < $scope.store.sections.length; j++) {
                    var section = $scope.store.sections[j];
                    if (section.productIds.indexOf(product.id) !== -1) {
                        section.products.push(product);
                        productFound = true;
                        break;
                    }
                }
                if (productFound) continue;

                // Check the ignoredProducts
                if ($scope.store.ignoredProducts.productIds.indexOf(product.id) !== -1) {
                    $scope.store.ignoredProducts.products.push(product);
                    continue;
                }

                // Fallback to newProductsSection
                $scope.newProductsSection.products.push(product);
            }
        }


        // === INIT ===
        activate();

        function activate() {
            storesService.get($routeParams.id)
                .then(function (result) {
                    $scope.store = result.data;
                    angular.forEach($scope.store.sections, function (value, key) {
                        value.products = [];
                    });
                    $scope.store.ignoredProducts.products = [];
                    updateCenterIndex();

                    // The stores are in place, now get the products
                    productsService.getAll()
                        .then(function (result) {
                            populateProducts(result.data);
                        }, function (result) {
                            showError('Lyckades inte hämta produkterna. :(', 'getAll', error);
                        })
                    .finally(function () {
                        $scope.isLoading = false;
                    });
                },
                function (error) {
                    showError('Lyckades inte hämta butiken. :(', 'get', error);
                    $scope.isLoading = false;
                });
        }
    }
})();