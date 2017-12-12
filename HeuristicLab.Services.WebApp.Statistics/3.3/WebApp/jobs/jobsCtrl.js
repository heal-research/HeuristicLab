(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.jobsCtrl',
        ['$scope', '$interval', 'app.statistics.jobService', function ($scope, $interval, jobService) {
            var first = true;
            $scope.isAdministrator = false;
            $scope.interval = defaultPageUpdateInterval;
            $scope.completedJobCurPage = 1;
            $scope.completedJobPageSize = 20;

            var getAllJobs = function() {
                jobService.getAllJobs({ completed: false }, function(jobs) {
                    $scope.jobs = jobs;
                });
            };

            var getCompletedJobs = function() {
                jobService.getJobs({ page: $scope.completedJobCurPage, size: $scope.completedJobPageSize, completed: true },
                    function (jobPage) {
                        $scope.completedJobPage = jobPage;
                    }
                );
            };

            var getAllActiveJobsFromAllUsers = function () {
                jobService.getAllActiveJobsFromAllUsers({}, function (jobs) {
                    $scope.isAdministrator = true;
                    $scope.allUsersJobs = jobs;
                });
            };

            $scope.changeCompletedJobPage = function () {
                update();
            };

            var update = function () {
                getAllJobs();
                if (first || $scope.isAdministrator) {
                    getAllActiveJobsFromAllUsers();
                }
                getCompletedJobs();
            };

            $scope.updateInterval = $interval(update, $scope.interval);
            var cancelInterval = $scope.$on('$locationChangeSuccess', function () {
                $interval.cancel($scope.updateInterval);
                cancelInterval();
            });
            update(); // init page
            first = false;
        }]
    );
})();