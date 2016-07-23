'use strict';
app.factory('authInterceptorService', ['$q', '$injector', '$location', 'localStorageService', 'appSettings', function ($q, $injector, $location, localStorageService, appSettings) {

    var authInterceptorServiceFactory = {};

    // Checks whether the specified url is listed in the configuration as a url that should have the auth token appended
    function isUrlListed(url) {
        return appSettings.appendAuthTokenUrls &&
            _.some(appSettings.appendAuthTokenUrls, function (listedUrl) { return _.startsWith(url, listedUrl) });
    }

    var _request = function (config) {

        // Only append the auth token to urls in the list in the config
        if (!isUrlListed(config.url)) return config;

        config.headers = config.headers || {};

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
        }

        return config;
    }

    var _responseError = function (rejection) {
        if (rejection.status === 401 && isUrlListed(rejection.config.url)) {
            var authService = $injector.get('authService');
            authService.logOut();
            $location.path('/');
        }
        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);