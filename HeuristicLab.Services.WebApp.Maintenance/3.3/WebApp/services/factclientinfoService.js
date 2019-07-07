(function () {
    var module = appMaintenancePlugin.getAngularModule();
    var apiUrl = 'api/Maintenance/FactClientInfo/';
    module.factory('app.maintenance.factclientinfoService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { start: '@start', end: '@end', entries: '@entries' }, {
                getFactClientInfo: { method: 'GET', params: { action: 'GetFactClientInfo' } },
                aggregate: { method: 'POST', params: { action: 'Aggregate' } }
            });
        }]
    );
})();