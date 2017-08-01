(function () {
    var module = appStatisticsPlugin.getAngularModule();
    var apiUrl = 'api/Statistics/Task/';
    module.factory('app.statistics.taskService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { id: '@id', page: '@page', size: '@size', states: '@states' }, {
                getTasksByJobId: { method: 'POST', params: { action: 'GetTasksByJobId' } },
                getTasksByClientId: { method: 'POST', params: { action: 'GetTasksByClientId' } }
            });
        }]
    );
})();