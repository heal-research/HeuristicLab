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
                    max: 100,
                    zoomRange: false,
                    panRange: false
                },
                xaxis: {
                    mode: "time",
                    twelveHourClock: false
                },
                zoom: {
                    interactive: true
                },
                pan: {
                    interactive: true
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
                yaxis: {
                    zoomRange: false,
                    panRange: false
                },
                xaxis: {
                    mode: "time",
                    twelveHourClock: false
                },
                zoom: {
                    interactive: true
                },
                pan: {
                    interactive: true
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

            var updateCharts = function () {
                dataService.getStatusHistory({start: ConvertFromDate($scope.fromDate), end: ConvertToDate($scope.toDate)}, function (status) {
                    var noOfStatus = status.length;
                    var cpuSeries = [];
                    var coreSeries = [[], []];
                    var memorySeries = [[], []];
                    for (var i = 0; i < noOfStatus; ++i) {
                        var curStatus = status[i];
                        var cpuData = Math.round(curStatus.CpuUtilizationStatus.UsedCpuUtilization);
                        var usedCores = curStatus.CoreStatus.TotalCores - curStatus.CoreStatus.FreeCores;
                        var usedMemory = curStatus.MemoryStatus.TotalMemory - curStatus.MemoryStatus.FreeMemory;
                        cpuSeries.push([curStatus.Timestamp, cpuData]);
                        coreSeries[0].push([curStatus.Timestamp, curStatus.CoreStatus.TotalCores]);
                        coreSeries[1].push([curStatus.Timestamp, usedCores]);
                        memorySeries[0].push([curStatus.Timestamp, curStatus.MemoryStatus.TotalMemory]);
                        memorySeries[1].push([curStatus.Timestamp, usedMemory]);
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

            $scope.$watch('fromDate', function (newValue, oldValue) {
                updateCharts();
            });
            $scope.$watch('toDate', function (newValue, oldValue) {
                updateCharts();
            });
        }]
    );
})();