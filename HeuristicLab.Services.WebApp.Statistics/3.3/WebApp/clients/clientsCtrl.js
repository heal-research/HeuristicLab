(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.clientsCtrl',
        ['$scope', '$interval', 'app.statistics.clientService',
        function ($scope, $interval, clientService) {
            $scope.interval = defaultPageUpdateInterval;
            $scope.curClientsPage = 1;
            $scope.clientsPageSize = 20;
            $scope.curExpiredClientsPage = 1;
            $scope.expiredClientsPageSize = 20;

            var getClients = function () {
                clientService.getClients({ page: $scope.curClientsPage, size: $scope.clientsPageSize, expired: false },
                    function (clientPage) {
                        $scope.clientPage = clientPage;
                    }
                );
            };

            var getExpiredClients = function() {
                clientService.getClients({ page: $scope.curExpiredClientsPage, size: $scope.expiredClientsPageSize, expired: true },
                    function (clientPage) {
                        $scope.expiredClientPage = clientPage;
                    }
                );
            };

            var update = function () {
                getClients();
                getExpiredClients();
            };

            $scope.changeClientsPage = function () {
                update();
            };

            $scope.changeExpiredClientsPage = function () {
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