(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.groupsCtrl',
        ['$scope', '$interval', 'app.statistics.groupService',
        function ($scope, $interval, groupService) {
            $scope.interval = defaultPageUpdateInterval;
            $scope.curGroupsPage = 1;
            $scope.groupsPageSize = 20;

            var getGroups = function () {
                groupService.getGroups({ page: $scope.curGroupsPage, size: $scope.groupsPageSize },
                    function (groupPage) {
                        $scope.groupPage = groupPage;
                    }
                );
            };

            var update = function () {
                getGroups();
            };

            $scope.changeGroupsPage = function () {
                update();
            };

            $scope.updateInterval = $interval(update, $scope.interval);
            var cancelInterval = $scope.$on('$locationChangeSuccess', function () {
                $interval.cancel($scope.updateInterval);
                cancelInterval();
            });
            update(); // init page
        }]
    );
})();