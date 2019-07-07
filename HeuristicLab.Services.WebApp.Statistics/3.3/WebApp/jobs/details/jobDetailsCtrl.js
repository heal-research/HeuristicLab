(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.jobDetailsCtrl',
        ['$scope', '$interval', '$stateParams', 'app.statistics.jobService', 'app.statistics.taskService', '$modal',
        function ($scope, $interval, $stateParams, jobService, taskService, $modal) {
            $scope.interval = defaultPageUpdateInterval;
            $scope.curTaskPage = 1;
            $scope.taskPageSize = 12;

            var getJobDetails = function () {
                jobService.getJobDetails({ id: $stateParams.id }, function (job) {
                    $scope.job = job;
                    $scope.job.CalculatingWaitingRatio = ($scope.job.TotalCalculatingTime / $scope.job.TotalWaitingTime);

                    var length = job.TasksStates.length;
                    var total = 0;
                    var jsStates = [];
                    for (var i = 0; i < length; ++i) {
                        var state = job.TasksStates[i];
                        var selected = true;
                        if (isDefined($scope.states)) {
                            for (var j = 0; j < $scope.states.length; ++j) {
                                if (state.State == $scope.states[j].State) {
                                    selected = $scope.states[j].Selected;
                                    break;
                                }
                            }
                        }
                        jsStates.push({
                            State: state.State,
                            Count: state.Count,
                            Selected: selected
                        });
                        total += state.Count;
                    }
                    $scope.totalJobTasks = total;
                    $scope.states = jsStates;
                    getTasks();
                });
            };

            var getTasks = function () {
                var states = [];
                var length = $scope.states.length;
                for (var i = 0; i < length; ++i) {
                    var state = $scope.states[i];
                    if (state.Selected) {
                        states.push(state.State);
                    }
                }

                taskService.getTasksByJobId({ id: $stateParams.id, page: $scope.curTaskPage, size: $scope.taskPageSize }, states,
                    function (taskPage) {
                        $scope.taskPage = taskPage;
                    }
                );
            };

            $scope.changeTaskPage = function () {
                getJobDetails();
                $('html, body').animate({
                    scrollTop: $("#job-filter-header").offset().top
                }, 10);
            };

            $scope.filterState = function (state) {
                state.Selected = !state.Selected;
                $scope.curTaskPage = 1;
                getJobDetails();
            };

            $scope.openDialog = function (taskNo, task) {
                $scope.currentTaskNo = taskNo;
                $scope.currentTask = task;
                $modal.open({
                    templateUrl: 'plugin=statistics&view=WebApp/jobs/details/jobTaskDetailsDialog.cshtml',
                    controller: 'app.statistics.jobTaskDetailsDialogCtrl',
                    windowClass: 'app-modal-window',
                    resolve: {
                        task: function() {
                            return $scope.currentTask;
                        },
                        taskNo: function() {
                            return $scope.currentTaskNo;
                        }
                    }
                });
            };

            var update = function () {
                getJobDetails();
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