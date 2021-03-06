(function () {
    'use strict';

    angular
        .module('myApp.shopping.inStore')
        .controller('SelectStoreCtrl', controller);

    controller.$inject = ['$scope', '$location', 'storesService', 'itemsService', '$mdToast', '$q'];

    function controller($scope, $location, storesService, itemsService, $mdToast, $q) {

        $scope.isLoading = true;
        $scope.isWorking = false;
        $scope.stores = [];
        $scope.selectedStore = null;
        $scope.items = [];

        $scope.selectStore = function (store) {
            $location.path('/handla/i-butik/' + store.id);
        }

        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            console.log('Call to ' + failedMethodName + ' failed: ' + error.statusText);
        }
        
        // === INIT ===
        function activate() {
            storesService.getAll()
                .then(
                    function (result) {
                        if (result.data.length === 1) {
                            $scope.selectStore(result.data[0]);
                        } else {
                            $scope.stores = result.data;
                        }
                    },
                    function(error) {
                        showError('Lyckades inte hämta några butiker. :(', 'storesService.getAll', error);
                    })
                .finally(function () { $scope.isLoading = false; });
        }

        activate();
    }
})();