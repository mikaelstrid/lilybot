'use strict';

var AUTH_API_SERVICE_BASE_URI = 'http://localhost:51350/';
var SHOPPING_API_SERVICE_BASE_URI = 'http://localhost:54299/';
var COMMUTE_API_SERVICE_BASE_URI = 'http://localhost:6001/';

angular.module('myApp').constant('appSettings', {
    clientId: 'lilybot.web',
    authApiServiceBaseUri: AUTH_API_SERVICE_BASE_URI,
    shoppingApiServiceBaseUri: SHOPPING_API_SERVICE_BASE_URI,
    commuteApiServiceBaseUri: COMMUTE_API_SERVICE_BASE_URI,
    vasttrafikKey: 'QUTQ96MIf4QvdZ0jG91UVEBLDGMa',
    vasttrafikSecret: '7iLamTRw_HxX_dzUFC3GKVkf8Pca',
    vasttrafikScope: '123',
    appendAuthTokenUrls: [SHOPPING_API_SERVICE_BASE_URI, COMMUTE_API_SERVICE_BASE_URI]
});
