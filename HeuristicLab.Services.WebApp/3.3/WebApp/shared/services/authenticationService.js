(function () {
    var apiUrl = 'api/App/Authentication/';
    var module = appMainPlugin.getAngularModule();
    module.factory('app.authentication.service',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { user: "@user" }, {
                login: { method: 'POST', params: { action: 'Login' } },
                logout: { method: 'POST', params: { action: 'Logout' } }
            });
        }]
    );
})();