(function () {
    var module = appMaintenancePlugin.getAngularModule();
    module.controller('app.maintenance.pluginCtrl',
        ['$scope', '$interval', 'app.maintenance.pluginService', function ($scope, $interval, pluginService) {
            $scope.interval = defaultPageUpdateInterval;
            $scope.curPluginsPage = 1;
            $scope.pluginsPageSize = 20;

            var getUnusedPlugins = function () {
                pluginService.getUnusedPlugins({ page: $scope.curPluginsPage, size: $scope.pluginsPageSize }, function (pluginPage) {
                    $scope.pluginPage = pluginPage;
                });
            };

            $scope.deletePlugin = function(id) {
                pluginService.deletePlugin({ id: id }, function() {
                    getUnusedPlugins();
                });
            };

            $scope.deleteUnusedPlugins = function() {
                pluginService.deleteUnusedPlugins({}, function() {
                    getUnusedPlugins();
                });
            };

            $scope.changePluginsPage = function () {
                getUnusedPlugins();
            };

            $scope.updateInterval = $interval(getUnusedPlugins, $scope.interval);
            var cancelInterval = $scope.$on('$locationChangeSuccess', function () {
                $interval.cancel($scope.updateInterval);
                cancelInterval();
            });
            getUnusedPlugins(); // init page
        }]
    );
})();