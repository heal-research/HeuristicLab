(function () {
    var module = appMaintenancePlugin.getAngularModule();
    module.controller('app.maintenance.factclientinfoCtrl',
        ['$scope', '$interval', 'app.maintenance.factclientinfoService', function ($scope, $interval, factclientinfoService) {
            $scope.interval = defaultPageUpdateInterval;
            $scope.entries = 2;

            $scope.fromDate = new Date();
            $scope.toDate = new Date();

            $scope.fromIsOpen = false;
            $scope.toIsOpen = false;

            $scope.openFromDateSelection = function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.toIsOpen = false;
                $scope.fromIsOpen = true;
            };

            $scope.openToDateSelection = function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.fromIsOpen = false;
                $scope.toIsOpen = true;
            };

            $scope.dateOptions = {
                formatYear: 'yy',
                startingDay: 1
            };

            $scope.getFactClientInfo = function () {
                factclientinfoService.getFactClientInfo({
                    start: ConvertFromDate($scope.fromDate),
                    end: ConvertToDate($scope.toDate)
                },
                function (factClientInfo) {
                    $scope.factClientInfo = factClientInfo;
                    $scope.aggregateStartDate = ConvertFromDate($scope.fromDate);
                    $scope.aggregateEndDate = ConvertToDate($scope.toDate);
                });
            };

            $scope.aggregate = function () {
                factclientinfoService.aggregate({
                    start: $scope.aggregateStartDate,
                    end: $scope.aggregateEndDate,
                    entries: $scope.entries
                }, function () {
                    $scope.getFactClientInfo();
                });
            };

            $scope.getFactClientInfo(); // init page
        }]
    );
})();