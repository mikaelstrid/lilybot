// ReSharper disable InconsistentNaming

'use strict';
app.factory('authenticationService', ['$http', '$q', '$log', 'localStorageService', 'appSettings', function ($http, $q, $log, localStorageService, appSettings) {

    var serviceBase = appSettings.authApiServiceBaseUri;

    var _userData = {
        isAuthorized: false,
        lilybotAccessToken: '',
        externalProvider: '',
        externalUserName: '',
        externalDisplayName: '',
        externalAccessToken: ''
    };

    var _clearUserData = function () {
        _userData.isAuthorized = false;
        _userData.lilybotAccessToken = '';
        _userData.externalProvider = '';
        _userData.externalUserName = '';
        _userData.externalDisplayName = '';
        _userData.externalAccessToken = '';
        localStorageService.remove('userAuthData');
    }

    var _saveUserData = function () {
        localStorageService.set('userAuthData', _userData);
    }

    var _loadUserData = function () {
        var storedData = localStorageService.get('userAuthData');
        if (storedData) 
            angular.extend(_userData, storedData);
    }

    var _logOut = function () {
        _clearUserData();
    };

    var _handleSuccessfulLilybotTokenRepsonse = function (provider, externalAccessToken, tokenResponse) {
        _userData.isAuthorized = true;
        _userData.lilybotAccessToken = tokenResponse.access_token;
        _userData.externalProvider = provider;
        _userData.externalUserName = tokenResponse.userName;
        _userData.externalAccessToken = externalAccessToken;
        _saveUserData();
    }

    var _createAccount = function (provider, externalAccessToken) {
        var deferred = $q.defer();

        $log.log('_createAccount', 'before post', provider, externalAccessToken);

        $http.post(serviceBase + 'api/register', { provider: provider, externalAccessToken: externalAccessToken })
            .then(
                function (tokenResponse) {
                    _handleSuccessfulLilybotTokenRepsonse(provider, externalAccessToken, tokenResponse.data);
                    $log.log('_createAccount', 'post response', JSON.stringify(_userData));
                    deferred.resolve(tokenResponse);
                },
                function(err) {
                    _logOut();
                    deferred.reject(err);
                });

        return deferred.promise;

    };

    var _obtainAccessToken = function (provider, externalAccessToken) {
        var deferred = $q.defer();

        $http.get(serviceBase + 'api/access-token', { params: { provider: provider, externalAccessToken: externalAccessToken }})
            .then(
                function (tokenResponse) {
                    _handleSuccessfulLilybotTokenRepsonse(provider, externalAccessToken, tokenResponse.data);
                    deferred.resolve(tokenResponse);
                },
                function(err) {
                    _logOut();
                    deferred.reject(err);
                });

        return deferred.promise;
    };


    var authServiceFactory = {};
    authServiceFactory.userData = _userData;
    authServiceFactory.clearUserData = _clearUserData;
    authServiceFactory.saveUserData = _saveUserData;
    authServiceFactory.loadUserData = _loadUserData;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.createAccount = _createAccount;
    authServiceFactory.obtainAccessToken = _obtainAccessToken;
    return authServiceFactory;
}]);