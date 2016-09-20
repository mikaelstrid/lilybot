'use strict';
app.factory('authenticationInterceptorService', ['$q', '$injector', '$location', 'appSettings', function ($q, $injector, $location, localStorageService, appSettings) {

    // Checks whether the specified url is listed in the configuration as a url that should have the auth token appended
    function isUrlListed(url) {
        return appSettings.appendAuthTokenUrls && 
            _.some(appSettings.appendAuthTokenUrls, function (listedUrl) { return _.startsWith(url, listedUrl) });
    }

    var _request = function (config) {

        // Only append the auth token to urls in the list in the config
        if (!isUrlListed(config.url)) return config;

        config.headers = config.headers || {};

        var authenticationService = $injector.get('authenticationService');
        if (authenticationService.userData.lilybotAccessToken) {
            config.headers.Authorization = 'Bearer ' + authenticationService.userData.lilybotAccessToken;
        }

        return config;
    }

    var _responseError = function (rejection) {
        if (rejection.status === 401 && isUrlListed(rejection.config.url)) {
            var authenticationService = $injector.get('authenticationService');
            authenticationService.logOut();
            $location.path('/');
        }
        return $q.reject(rejection);
    }

    var factory = {};
    factory.request = _request;
    factory.responseError = _responseError;
    return factory;
}]);