(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.exceptionsCtrl',
        ['$scope', '$interval', '$modal', 'app.statistics.exceptionService',
            function ($scope, $interval, $modal, exceptionService) {
                $scope.interval = defaultPageUpdateInterval;
                $scope.curExceptionsPage = 1;
                $scope.exceptionsPageSize = 20;

                var getExceptions = function () {
                    exceptionService.getExceptions({ page: $scope.curExceptionsPage, size: $scope.exceptionsPageSize },
                        function (exceptionPage) {
                            $scope.exceptionPage = exceptionPage;
                        }
                    );
                };

                $scope.changeExceptionsPage = function () {
                    update();
                };

                var update = function () {
                    getExceptions();
                };

                $scope.openDialog = function (message) {
                    $scope.message = message;
                    $modal.open({
                        templateUrl: 'plugin=statistics&view=WebApp/exceptions/exceptionDetailsDialog.cshtml',
                        controller: 'app.statistics.exceptionDetailsDialogCtrl',
                        windowClass: 'app-modal-window',
                        resolve: {
                            message: function () {
                                return $scope.message;
                            }
                        }
                    });
                };

                $scope.updateInterval = $interval(update, $scope.interval);
                var cancelInterval = $scope.$on('$locationChangeSuccess', function () {
                    $interval.cancel($scope.updateInterval);
                    cancelInterval();
                });
                update();
            }
        ]
    );
})();