(function () {
    var module = appMaintenancePlugin.getAngularModule();
    module.controller('app.maintenance.facttaskCtrl',
        ['$scope', '$interval', 'app.maintenance.facttaskService', function ($scope, $interval, facttaskService) {
            $scope.interval = defaultPageUpdateInterval;
            $scope.curJobsPage = 1;
            $scope.jobsPageSize = 20;

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

            $scope.getJobs = function () {
                facttaskService.getJobs({
                    start: ConvertFromDate($scope.fromDate),
                    end: ConvertToDate($scope.toDate),
                    page: $scope.curJobsPage,
                    size: $scope.jobsPageSize
                },
                function (jobPage) {
                    $scope.jobPage = jobPage;
                });
            };

            $scope.aggregateJob = function (id) {
                facttaskService.aggregateJob({ id: id }, function () {
                    $scope.getJobs();
                });
            };

            $scope.aggregateAllJobs = function () {
                facttaskService.aggregateAllJobs({
                    start: ConvertFromDate($scope.fromDate),
                    end: ConvertToDate($scope.toDate)
                },
                function () {
                    $scope.getJobs();
                });
            };

            $scope.changeJobsPage = function () {
                $scope.getJobs();
            };

            $scope.getJobs(); // init page
        }]
    );
})();