(function () {
    'use strict';

    angular
        .module('myApp.stores')
        .controller('StoresCtrl', controller);

    controller.$inject = ['$scope', '$location', 'storesService', '$mdDialog', '$mdToast'];

    function controller($scope, $location, storesService, $mdDialog, $mdToast) {

        $scope.isLoading = true;
        $scope.stores = [];

        $scope.showAddNewDialog = function (ev) {
            var confirm = $mdDialog.prompt()
                  .title('Vad ska den nya butiken heta?')
                  .placeholder('fyll i ett namn')
                  .ariaLabel('Butiksnamn')
                  .targetEvent(ev)
                  .ok('OK')
                  .cancel('Avbryt');
            $mdDialog.show(confirm).then(function (dialogResult) {
                storesService.add(dialogResult).then(function (result) {
                    $scope.stores.push(result.data);
                }, function (error) {
                    $mdToast.show(
                          $mdToast.simple()
                            .textContent('Lyckades inte spara den nya butiken. :(')
                            .hideDelay(3000)
                        );
                    console.log('Call to storesService.add failed: ' + error.statusText);
                });
            });
        };

        $scope.gotoDetails = function (store) {
            $location.path('/stores/' + store.id);
        }

        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            console.log('Call to storesService.' + failedMethodName + ' failed: ' + error.statusText);
        }

        activate();

        function activate() {
            storesService.getAll()
                .then(function(result) {
                        $scope.stores = result.data;
                    },
                    function (error) {
                        showError('Lyckades inte hämta några butiker. :(', 'getAll', error);
                    })
                .finally(function() {
                    $scope.isLoading = false;
                });
        }
    }
})();