(function () {
    var module = appMaintenancePlugin.getAngularModule();
    module.controller('app.maintenance.spaceUsageCtrl',
        ['$scope', '$interval', 'app.maintenance.spaceUsageService', function ($scope, $interval, spaceUsageService) {
            $scope.interval = defaultPageUpdateInterval;

            var getTableInformations = function () {
                spaceUsageService.getHiveTableInformation({}, function (tableInformations) {
                    $scope.hiveTableInformation = tableInformations;
                });
                spaceUsageService.getStatisticsTableInformation({}, function (tableInformations) {
                    $scope.statisticsTableInformation = tableInformations;
                });
            };

            $scope.updateInterval = $interval(getTableInformations, $scope.interval);
            var cancelInterval = $scope.$on('$locationChangeSuccess', function () {
                $interval.cancel($scope.updateInterval);
                cancelInterval();
            });
            getTableInformations(); // init page
        }]
    );
})();