// ReSharper disable InconsistentNaming

'use strict';
app.factory('authenticationService', ['$http', '$q', 'localStorageService', 'appSettings', function ($http, $q, localStorageService, appSettings) {

    var serviceBase = appSettings.authApiServiceBaseUri;
    var authServiceFactory = {};

    var _userData = {
        isAuthorized: false
    };

    var _externalAuthData = {
        provider: '',
        userName: '',
        accessToken: ''
    };


    var _fillUserData = function () {
        var authorizationData = localStorageService.get('authorizationData');
        if (authorizationData) {
            _userData.isAuthorized = true;
        }
    };

    var _logOut = function () {
        localStorageService.remove('authorizationData');
        localStorageService.remove('externalAuthData');
        _userData.isAuthorized = false;
    };

    var _createAccount = function (provider, accessToken) {
        var deferred = $q.defer();

        $http.post(serviceBase + 'api/account/registerexternal', { provider: provider, externalAccessToken: accessToken })
            .success(function (response) {
                localStorageService.set('authorizationData', { token: response.access_token });
                _userData.isAuthorized = true;
                deferred.resolve(response);
            })
            .error(function (err, status) {
                _logOut();
                deferred.reject(err);
            });

        return deferred.promise;

    };

    var _updateExternalAuthData = function (externalAuthData) {
        _externalAuthData.provider = externalAuthData.provider;
        _externalAuthData.userName = externalAuthData.userName,
        _externalAuthData.accessToken = externalAuthData.accessToken;
        localStorageService.set('externalAuthData', externalAuthData);
    }

    var _checkToken = function () {
        var deferred = $q.defer();



        return deferred.promise;
    }

    var _obtainAccessToken = function (externalAuthData) {
        var deferred = $q.defer();

        $http.get(serviceBase + 'api/account/ObtainLocalAccessToken',
            {
                params: {
                    provider: externalAuthData.provider,
                    externalAccessToken: externalAuthData.accessToken
                }
            }).success(function (response) {
                localStorageService.set('authorizationData', { token: response.access_token });
                _userData.isAuthorized = true;
                deferred.resolve(response);
            }).error(function (err, status) {
                _logOut();
                deferred.reject(err);
            });

        return deferred.promise;
    };



    authServiceFactory.userData = _userData;
    authServiceFactory.externalAuthData = _externalAuthData;
    authServiceFactory.fillUserData = _fillUserData;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.createAccount = _createAccount;
    authServiceFactory.updateExternalAuthData = _updateExternalAuthData;
    authServiceFactory.checkToken = _checkToken;
    authServiceFactory.obtainAccessToken = _obtainAccessToken;
    return authServiceFactory;
}]);