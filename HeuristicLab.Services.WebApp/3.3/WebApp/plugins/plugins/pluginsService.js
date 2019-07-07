(function () {
    var apiUrl = 'api/App/Plugin/';
    var module = appPluginsPlugin.getAngularModule();
    module.factory('app.plugins.service',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { name: "@name" }, {
                getPlugins: { method: 'GET', params: { action: 'GetPlugins' }, isArray: true },
                reloadPlugin: { method: 'POST', params: { action: 'ReloadPlugin' } }
            });
        }]
    );
})();