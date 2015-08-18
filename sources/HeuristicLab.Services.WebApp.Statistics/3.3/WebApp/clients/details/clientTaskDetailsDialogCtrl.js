(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.clientTaskDetailsDialogCtrl',
        ['$scope', '$modalInstance', 'taskNo', 'task', function ($scope, $modalInstance, taskNo, task) {
            $scope.taskNo = taskNo;
            $scope.task = task;
            $scope.close = function () {
                $modalInstance.dismiss('cancel');
            };
        }]
    );
})();