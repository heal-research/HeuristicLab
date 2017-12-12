(function () {
    var module = appStatisticsPlugin.getAngularModule();
    var apiUrl = 'api/Statistics/Client/';
    module.factory('app.statistics.clientService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { id: '@id', page: '@page', size: '@size', states: '@states', expired: '@expired', start: '@start', end: '@end', userId: '@userId' }, {
                getClientDetails: { method: 'GET', params: { action: 'GetClientDetails' } },
                getClients: { method: 'GET', params: { action: 'GetClients' } },
                getClientsByGroupId: { method: 'GET', params: { action: 'GetClientsByGroupId' } },
                getClientHistory: { method: 'GET', params: { action: 'GetClientHistory' }, isArray: true },
                getClientHistoryByGroupId: { method: 'GET', params: { action: 'GetClientHistoryByGroupId' }, isArray: true }
            });
        }]
    );
})();