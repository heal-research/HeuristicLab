(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.userDetailsCtrl',
        ['$scope', '$stateParams', '$interval', 'app.statistics.userService', 'app.statistics.jobService',
        function ($scope, $stateParams, $interval, userService, jobService) {
            $scope.interval = defaultPageUpdateInterval;
            $scope.completedJobCurPage = 1;
            $scope.completedJobPageSize = 20;

            var getUserDetails = function () {
                userService.getUser({ id: $stateParams.id }, function(user) {
                    $scope.user = user;

                    var length = user.TasksStates.length;
                    var total = 0;
                    for (var i = 0; i < length; ++i) {
                        total += user.TasksStates[i].Count;
                    }
                    $scope.totalUserTasks = total;
                });
            };

            var getAllJobs = function () {
                jobService.getAllJobsByUserId({ id: $stateParams.id, completed: false }, function (jobs) {
                    $scope.jobs = jobs;
                });
            };

            var getCompletedJobs = function () {
                jobService.getJobsByUserId({ id: $stateParams.id, page: $scope.completedJobCurPage, size: $scope.completedJobPageSize, completed: true },
                    function (jobPage) {
                        $scope.completedJobPage = jobPage;
                    }
                );
            };

            $scope.changeCompletedJobPage = function () {
                update();
            };

            var update = function () {
                getUserDetails();
                getAllJobs();
                getCompletedJobs();
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