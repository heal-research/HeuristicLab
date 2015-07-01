(function () {
    var module = appStatusPlugin.getAngularModule();
    module.controller('app.status.historyCtrl',
        ['$scope', '$interval', 'app.status.data.service',
        function ($scope, $interval, dataService) {
            $scope.chartOptions = {
                grid: {
                    borderWidth: 1,
                    labelMargin: 15
                },
                series: {
                    shadowSize: 0
                },
                yaxis: {
                    min: 0,
                    max: 100
                },
                xaxis: {
                    mode: "time",
                    twelveHourClock: false
                }
            };

            $scope.fillChartOptions = {
                grid: {
                    borderWidth: 1,
                    labelMargin: 15
                },
                series: {
                    shadowSize: 0,
                    lines: {
                        show: true,
                        fill: true
                    }
                },
                xaxis: {
                    mode: "time",
                    twelveHourClock: false
                }
            };


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

            $scope.cpuSeries = [[]];
            $scope.coreSeries = [[]];
            $scope.memorySeries = [[]];

            $scope.updateCharts = function () {
                dataService.getStatusHistory({ start: ConvertFromDate($scope.fromDate), end: ConvertToDate($scope.toDate) }, function (status) {
                    var noOfStatus = status.length;
                    var cpuSeries = [];
                    var coreSeries = [[], []];
                    var memorySeries = [[], []];
                    for (var i = 0; i < noOfStatus; ++i) {
                        var curStatus = status[i];
                        var cpuData = Math.round(curStatus.CpuUtilizationStatus.ActiveCpuUtilization);
                        cpuSeries.push([curStatus.Timestamp, cpuData]);
                        coreSeries[0].push([curStatus.Timestamp, curStatus.CoreStatus.ActiveCores]);
                        coreSeries[1].push([curStatus.Timestamp, curStatus.CoreStatus.CalculatingCores]);
                        memorySeries[0].push([curStatus.Timestamp, Math.round(curStatus.MemoryStatus.ActiveMemory / 1024)]);
                        memorySeries[1].push([curStatus.Timestamp, Math.round(curStatus.MemoryStatus.UsedMemory / 1024)]);
                    }
                    $scope.cpuSeries = [{ data: cpuSeries, label: "&nbsp;CPU Utilization", color: "#f7921d" }];
                    $scope.coreSeries = [
                        { data: coreSeries[0], label: "&nbsp;Total Cores", color: "LightGreen" },
                        { data: coreSeries[1], label: "&nbsp;Used Cores", color: "LightPink" }
                    ];
                    $scope.memorySeries = [
                        { data: memorySeries[0], label: "&nbsp;Total Memory", color: "LightGreen" },
                        { data: memorySeries[1], label: "&nbsp;Used Memory", color: "LightPink" }
                    ];

                });
            };
            $scope.updateCharts();
        }]
    );
})();