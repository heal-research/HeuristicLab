(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.exceptionDetailsDialogCtrl',
        ['$scope', '$modalInstance', 'message', function ($scope, $modalInstance, message) {
            $scope.message = message;
            $scope.close = function () {
                $modalInstance.dismiss('cancel');
            };
        }]
    );
})();