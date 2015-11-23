(function () {
    var module = appStatisticsPlugin.getAngularModule();
    var apiUrl = 'api/Statistics/User/';
    module.factory('app.statistics.userService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { id: '@id' }, {
                getUser: { method: 'GET', params: { action: 'GetUser' } },
                getUsers: { method: 'GET', params: { action: 'GetUsers' }, isArray: true }
            });
        }]
    );
})();