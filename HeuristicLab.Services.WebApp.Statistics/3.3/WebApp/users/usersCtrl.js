(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.usersCtrl',
        ['$scope', '$interval', 'app.statistics.userService', function ($scope, $interval, userService) {
            $scope.interval = defaultPageUpdateInterval;

            var getUsers = function() {
                userService.getUsers({}, function(users) {
                    users.splice(0, 1);
                    $scope.users = users;
                });
            };

            $scope.updateInterval = $interval(getUsers, $scope.interval);
            var cancelInterval = $scope.$on('$locationChangeSuccess', function () {
                $interval.cancel($scope.updateInterval);
                cancelInterval();
            });
            getUsers(); // init page
        }]
    );
})();