'use strict';

var AUTH_API_SERVICE_BASE_URI = 'http://localhost:51350/';
var RESOURCE_API_SERVICE_BASE_URI = 'http://localhost:54299/';

angular.module('myApp').constant('appSettings', {
    authApiServiceBaseUri: AUTH_API_SERVICE_BASE_URI,
    resourceApiServiceBaseUri: RESOURCE_API_SERVICE_BASE_URI,
    clientId: 'lilybot.web',
    appendAuthTokenUrls: [RESOURCE_API_SERVICE_BASE_URI]
});
