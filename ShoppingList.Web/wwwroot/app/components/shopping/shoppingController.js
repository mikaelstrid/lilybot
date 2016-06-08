(function () {
    'use strict';

    angular
        .module('myApp.shopping')
        .controller('ShoppingCtrl', controller);

    controller.$inject = ['$scope', '$location', 'itemsService', '$mdToast'];

    function controller($scope, $location, itemsService, $mdToast) {

        $scope.isLoading = true;
        $scope.isWorking = false;
        $scope.items = [];

        $scope.markItemAsDone = function(item) {
            $scope.isWorking = true;
            itemsService.markItemAsDone(item.id)
                .then(
                    function() {
                        _.remove($scope.items, function (i) { return i.id === item.id; });
                        showUndoToast(item);
                    },
                    function(error) {
                        showError('Lyckades inte markera varan som klar. :(', 'itemsService.markItemAsDone', error);
                    })
                .finally(function() {
                    $scope.isWorking = false;
                });
        }


        // === HELPERS ===
        function showError(messageToUser, failedMethodName, error) {
            if (error.status !== 401) {
                $mdToast.show($mdToast.simple().textContent(messageToUser).hideDelay(3000));
            }
            console.log('Call to ' + failedMethodName + ' failed: ' + error.statusText);
        }

        function showUndoToast(item) {
            var toast = $mdToast.simple()
                .textContent(item.productName + ' klar.')
                .action('Ångra')
                .highlightAction(true)
                .hideDelay(10000);
            $mdToast.show(toast)
                .then(function(response) {
                    if (response === 'ok') {
                        addItemToList(item.productId, item.productName);
                    }
                });
        }

        function addItemToList(productId, productName) {
            $scope.isWorking = true;
            itemsService.add(productId)
                .then(
                    function (result) {
                        $scope.items.push({ id: result.data.id, productId: productId, productName: productName });
                    },
                    function (error) {
                        showError('Lyckades inte lägga till produkten i inköpslistan. :(', 'itemsService.add', error);
                    })
                .finally(function () {
                    $scope.isWorking = false;
                });
        }

        
        // === INIT ===
        function activate() {
            itemsService.getActive()
                .then(
                    function(result) {
                        $scope.items = result.data;
                    },
                    function(error) {
                        showError('Lyckades inte hämta några varor. :(', 'itemsService.getActive', error);
                    })
                .finally(function() {
                    $scope.isLoading = false;
                });
        }

        activate();
    }
})();