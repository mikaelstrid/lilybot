(function () {
    'use strict';

    angular
        .module('myApp.stores')
        .controller('StoreDetailsCtrl', controller);

    controller.$inject = ['$scope', '$routeParams', '$location', 'storesService', '$mdDialog', '$mdToast'];

    function controller($scope, $routeParams, $location, storesService, $mdDialog, $mdToast) {

        $scope.isLoading = true;
        $scope.store = null;

        $scope.showRenameDialog = function (ev) {
            var confirm = $mdDialog.prompt()
                  .title('Vad ska den nya butiken heta?')
                  .placeholder($scope.store.name)
                  .ariaLabel('Ändra butiksnamn')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function (dialogResult) {
                $scope.isLoading = true;
                storesService.rename($scope.store.id, dialogResult)
                    .then(function () {
                        $scope.store.name = dialogResult;
                    },
                        function (error) {
                            showError('Lyckades inte ändra butiksnamnet. :(', 'rename', error);
                        })
                    .finally(function () {
                        $scope.isLoading = false;
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
                $scope.isLoading = true;
                storesService.remove($scope.store.id)
                    .then(function () {
                        $location.path('/stores');
                    },
                        function (error) {
                            showError('Lyckades inte ta bort butiken. :(', 'remove', error);
                        })
                    .finally(function () {
                        $scope.isLoading = false;
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
                storesService.addNewSection($scope.store.id, dialogResult).then(function (result) {
                    $scope.store.sections.push(result.data);
                }, function (error) {
                    showError('Lyckades inte spara den nya avdelningen. :(', 'addNewSection', error);
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
                storesService.removeSection($scope.store.id, section.id)
                    .then(function() {
                            // Remove the section from the scope
                            var index = $scope.store.sections.indexOf(section);
                            if (index > -1) $scope.store.sections.splice(index, 1);
                        },
                        function(error) {
                            showError('Lyckades inte ta bort avdelningen. :(', 'removeSection', error);
                        });
            });
        };

        $scope.showRenameSectionDialog = function(ev, section) {
            
        }

        $scope.moveSectionUp = function (ev) {

        }

        $scope.moveSectionDown = function (ev) {

        }


        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            console.log('Call to storesService.' + failedMethodName + ' failed: ' + error.statusText);
        }

        // === INIT ===
        activate();

        function activate() {
            storesService.get($routeParams.id)
                .then(function (result) {
                    $scope.storeCenterIndex = Math.ceil(result.data.sections.length / 2);
                    $scope.store = result.data;
                },
                    function (result) {
                        $mdToast.show(
                            $mdToast.simple()
                            .textContent('Lyckades inte hämta butiken. :(')
                            .hideDelay(3000)
                        );
                        console.log('Call to storesService.get failed: ' + result.statusText);
                    })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
    }
})();