(function () {
    var module = appMaintenancePlugin.getAngularModule();
    var apiUrl = 'api/Maintenance/SpaceUsage/';
    module.factory('app.maintenance.spaceUsageService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', {}, {
                getHiveTableInformation: { method: 'GET', params: { action: 'GetHiveTableInformation' }, isArray: true },
                getStatisticsTableInformation: { method: 'GET', params: { action: 'GetStatisticsTableInformation' }, isArray: true }
            });
        }]
    );
})();