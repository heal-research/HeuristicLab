(function () {
    var module = appMaintenancePlugin.getAngularModule();
    var apiUrl = 'api/Maintenance/FactTask/';
    module.factory('app.maintenance.facttaskService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { start: '@start', end: '@end', id: '@id', page: '@page', size: '@size' }, {
                getJobs: { method: 'GET', params: { action: 'GetJobs' } },
                aggregateJob: { method: 'POST', params: { action: 'AggregateJob' } },
                aggregateAllJobs: { method: 'POST', params: { action: 'AggregateAllJobs' } }
            });
        }]
    );
})();