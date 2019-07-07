(function () {
    var module = appStatisticsPlugin.getAngularModule();
    var apiUrl = 'api/Statistics/Exception/';
    module.factory('app.statistics.exceptionService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { page: '@page', size: '@size'}, {
                getExceptions: { method: 'GET', params: { action: 'GetExceptions' } }
            });
        }]
    );
})();