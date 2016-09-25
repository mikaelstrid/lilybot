'use strict';

var AUTH_API_SERVICE_BASE_URI = 'https://lilybotauthapi.azurewebsites.net/';
var RESOURCE_API_SERVICE_BASE_URI = 'https://lilybotshoppingapi.azurewebsites.net/';

angular.module('myApp').constant('appSettings', {
    authApiServiceBaseUri: AUTH_API_SERVICE_BASE_URI,
    resourceApiServiceBaseUri: RESOURCE_API_SERVICE_BASE_URI,
    clientId: 'lilybot.web',
    appendAuthTokenUrls: [RESOURCE_API_SERVICE_BASE_URI]
});
