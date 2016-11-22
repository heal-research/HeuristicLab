(function () {
    var module = appMaintenancePlugin.getAngularModule();
    var apiUrl = 'api/Maintenance/Plugin/';
    module.factory('app.maintenance.pluginService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { id: '@id', page: '@page', size: '@size' }, {
                getUnusedPlugins: { method: 'GET', params: { action: 'GetUnusedPlugins' } },
                deletePlugin: { method: 'POST', params: { action: 'DeletePlugin' } },
                deleteUnusedPlugins: { method: 'POST', params: { action: 'DeleteUnusedPlugins' } }
            });
        }]
    );
})();