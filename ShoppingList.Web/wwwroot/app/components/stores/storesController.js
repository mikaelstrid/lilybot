'use strict';

angular.module('myApp.stores').controller('StoresCtrl', ['$scope', 'storesService', '$mdDialog', '$mdToast', function ($scope, storesService, $mdDialog, $mdToast) {

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
                console.log('Call to storesService.add failed: ' + error);
            });
        });
    };


    // INIT
    storesService.getAll().then(function (result) {
        $scope.stores = result.data;
    }, function (result) {
        $mdToast.show(
              $mdToast.simple()
                .textContent('Lyckades inte hämta några butiker. :(')
                .hideDelay(3000)
            );
        console.log('Call to storesService.getAll failed: ' + result.statusText);
    })
    .finally(function () {
        $scope.isLoading = false;
    });
}]);
