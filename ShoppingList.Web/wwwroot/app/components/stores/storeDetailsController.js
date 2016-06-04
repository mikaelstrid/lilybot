(function () {
    'use strict';

    angular
        .module('myApp.stores')
        .controller('StoreDetailsCtrl', controller);

    controller.$inject = ['$scope', '$routeParams', '$location', 'storesService', '$mdDialog', '$mdToast'];

    function controller($scope, $routeParams, $location, storesService, $mdDialog, $mdToast) {

        $scope.isLoading = true;
        $scope.isWorking = false;
        $scope.store = null;

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
                    .then(function(result) {
                            $scope.store.sections.push(result.data);
                            updateCenterIndex();
                        },
                        function(error) {
                            showError('Lyckades inte spara den nya avdelningen. :(', 'addNewSection', error);
                        })
                    .finally(function() {
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
                    .then(function() {
                            // Remove the section from the scope
                            var index = $scope.store.sections.indexOf(section);
                            $scope.store.sections.splice(index, 1);
                            updateCenterIndex();
                        },
                        function(error) {
                            showError('Lyckades inte ta bort avdelningen. :(', 'removeSection', error);
                        })
                    .finally(function() {
                        $scope.isWorking = false;
                    });
            });
        };

        $scope.showRenameSectionDialog = function(ev, section) {
            var confirm = $mdDialog.prompt()
                .title('Vad vill du att avdelningen ska heta istället?')
                .placeholder(section.name)
                .ariaLabel('Ändra namn på avdelning')
                .targetEvent(ev)
                .ok('OK')
                .cancel('Avbryt');
            $mdDialog.show(confirm)
                .then(function(dialogResult) {
                    $scope.isWorking = true;
                    storesService.renameSection($scope.store.id, section.id, dialogResult)
                        .then(function() {
                                section.name = dialogResult;
                            },
                            function(error) {
                                showError('Lyckades inte ändra namnet på avdelningen. :(', 'renameSection', error);
                            })
                        .finally(function() {
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
                            $scope.store.sections.splice(index-1, 0, section);
                            updateCenterIndex();
                        }
                    },
                    function(error) {
                        showError('Lyckades inte flytta upp avdelningen. :(', 'moveSectionUp', error);
                    })
                .finally(function() {
                    $scope.isWorking = false;
                });
        }

        $scope.moveSectionDown = function (section) {
            $scope.isWorking = true;
            storesService.moveSectionDown($scope.store.id, section.id)
                .then(function () {
                        var index = $scope.store.sections.indexOf(section);
                        if (index !== ($scope.store.sections.length-1)) {
                            $scope.store.sections.splice(index, 1);
                            $scope.store.sections.splice(index + 1, 0, section);
                            updateCenterIndex();
                        }
                    },
                    function(error) {
                        showError('Lyckades inte flytta ner avdelningen. :(', 'moveSectionDown', error);
                    })
                .finally(function() {
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


        // === INIT ===
        activate();

        function activate() {
            storesService.get($routeParams.id)
                .then(function(result) {
                        $scope.store = result.data;
                        updateCenterIndex();
                    },
                    function(result) {
                        $mdToast.show(
                            $mdToast.simple()
                            .textContent('Lyckades inte hämta butiken. :(')
                            .hideDelay(3000)
                        );
                        console.log('Call to storesService.get failed: ' + result.statusText);
                    })
                .finally(function() {
                    $scope.isLoading = false;
                });
        }
    }
})();