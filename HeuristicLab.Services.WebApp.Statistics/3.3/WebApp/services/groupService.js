(function () {
    var module = appStatisticsPlugin.getAngularModule();
    var apiUrl = 'api/Statistics/Group/';
    module.factory('app.statistics.groupService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { id: '@id', page: '@page', size: '@size', start: '@start', end: '@end' }, {
                getGroupDetails: { method: 'GET', params: { action: 'GetGroupDetails' } },
                getGroups: { method: 'GET', params: { action: 'GetGroups' } },
                getGroupHistory: { method: 'GET', params: { action: 'GetGroupHistory' }, isArray: true }
            });
        }]
    );
})();