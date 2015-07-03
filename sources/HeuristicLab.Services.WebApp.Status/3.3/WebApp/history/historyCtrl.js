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

            $scope.quickSelectionList = [
                { id: 0, name: 'Custom' },
                { id: 1, name: 'Today' },
                { id: 2, name: 'Yesterday' },
                { id: 3, name: 'Last 7 Days' },
                { id: 4, name: 'Last 30 Days' }
            ];
            $scope.changeQuickSelection = function (quickSelection) {
                var today = new Date();
                var oneDayInMs = 24 * 60 * 60 * 1000;
                switch (quickSelection.id) {
                    case 1:
                        $scope.fromDate = new Date(today.valueOf());
                        $scope.toDate = new Date(today.valueOf());
                        break;
                    case 2:
                        $scope.fromDate = new Date(today.valueOf() - oneDayInMs);
                        $scope.toDate = new Date(today.valueOf() - oneDayInMs);
                        break;
                    case 3:
                        $scope.fromDate = new Date(today.valueOf() - (7 * oneDayInMs));
                        $scope.toDate = new Date(today.valueOf());
                        break;
                    case 4:
                        $scope.fromDate = new Date(today.valueOf() - (30 * oneDayInMs));
                        $scope.toDate = new Date(today.valueOf());
                        break;
                }
                $scope.curQuickSelection = quickSelection;
            };
            // set default 'today'
            $scope.changeQuickSelection($scope.quickSelectionList[1]);

            $scope.openFromDateSelection = function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.toIsOpen = false;
                $scope.fromIsOpen = true;
                $scope.curQuickSelection = $scope.quickSelectionList[0];
            };

            $scope.openToDateSelection = function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.fromIsOpen = false;
                $scope.toIsOpen = true;
                $scope.curQuickSelection = $scope.quickSelectionList[0];
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