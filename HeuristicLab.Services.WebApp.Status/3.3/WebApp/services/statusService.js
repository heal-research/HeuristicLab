(function () {
    var module = appStatusPlugin.getAngularModule();
    var apiUrl = 'api/Status/Data/';
    module.factory('app.status.data.service',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { start: '@start', end: '@end' }, {
                getStatus: { method: 'GET', params: { action: 'GetStatus' } },
                getStatusHistory: { method: 'GET', params: { action: 'GetStatusHistory' }, isArray: true }
            });
        }]
    );
})();