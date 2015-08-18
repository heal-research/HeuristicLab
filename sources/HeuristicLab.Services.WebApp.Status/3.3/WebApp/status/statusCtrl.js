(function () {
    var module = appStatusPlugin.getAngularModule();
    module.controller('app.status.ctrl',
        ['$scope', '$interval', 'app.status.data.service',
        function ($scope, $interval, dataService) {
            $scope.interval = 10000; // update interval in ms
            $scope.knobOptions = {
                'fgColor': "#f7921d",
                'angleOffset': -125,
                'angleArc': 250,
                'readOnly': true,
                'width': "80%",
                'targetvalue': "100",
                'format': function (value) {
                    return value;
                },
                draw: function () {
                    $(this.i).val(this.cv + '%');
                }
            };

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
                },
                yaxis: {
                    min: 0
                }
            };

            $scope.cpu = {
                series: [{ data: [], label: "&nbsp;CPU Utilization", color: "#f7921d" }],
                knobData: 0
            };

            $scope.core = {
                series: [
                    { data: [], label: "&nbsp;Total Cores", color: "LightGreen" },
                    { data: [], label: "&nbsp;Used Cores", color: "LightPink" }
                ],
                knobData: 0
            };

            $scope.memory = {
                series: [
                    { data: [], label: "&nbsp;Total Memory", color: "LightGreen" },
                    { data: [], label: "&nbsp;Used Memory", color: "LightPink" }
                ],
                knobData: 0
            };

            $scope.tasks = {
                WaitingTasks: 0,
                CalculatingTasks: 0
            };

            var updateStatus = function () {
                // update status data
                dataService.getStatus({}, function (status) {
                    var oneDayInMs = 24 * 60 * 60 * 1000;
                    var today = new Date().getTime() - oneDayInMs;
                    // raw status data
                    $scope.status = status;
                    // tasks data
                    $scope.tasks.WaitingTasks = 0;
                    $scope.tasks.CalculatingTasks = 0;
                    for (var i = 0; i < status.TasksStatus.length; ++i) {
                        var task = status.TasksStatus[i];
                        $scope.tasks.WaitingTasks += task.WaitingTasks;
                        $scope.tasks.CalculatingTasks += task.CalculatingTasks;
                    }
                    // knobs
                    $scope.cpu.knobData = Math.round(status.CpuUtilizationStatus.ActiveCpuUtilization);
                    $scope.core.knobData = Math.round(status.CoreStatus.CalculatingCores / status.CoreStatus.ActiveCores * 100);
                    $scope.memory.knobData = Math.round(status.MemoryStatus.CalculatingMemory / status.MemoryStatus.ActiveMemory * 100);
                    // chart series
                    var cpuSeries = $scope.cpu.series[0].data.splice(0);
                    var coreSeries = [$scope.core.series[0].data, $scope.core.series[1].data];
                    var memorySeries = [$scope.memory.series[0].data, $scope.memory.series[1].data];
                    if ($scope.status.Timestamp < today) {
                        if (cpuSeries.length > 2) {
                            cpuSeries.splice(0, 1);
                        }
                        if (coreSeries[0].length > 2) {
                            coreSeries[0].splice(0, 1);
                        }
                        if (coreSeries[1].length > 2) {
                            coreSeries[1].splice(0, 1);
                        }
                        if (memorySeries[0].length > 2) {
                            memorySeries[0].splice(0, 1);
                        }
                        if (memorySeries[1].length > 2) {
                            memorySeries[1].splice(0, 1);
                        }
                    }
                    
                    cpuSeries.push([$scope.status.Timestamp, Math.round(status.CpuUtilizationStatus.TotalCpuUtilization)]);
                    coreSeries[0].push([$scope.status.Timestamp, status.CoreStatus.TotalCores]);
                    coreSeries[1].push([$scope.status.Timestamp, status.CoreStatus.UsedCores]);
                    memorySeries[0].push([$scope.status.Timestamp, Math.round(status.MemoryStatus.TotalMemory / 1024)]);
                    memorySeries[1].push([$scope.status.Timestamp, Math.round(status.MemoryStatus.UsedMemory / 1024)]);
                    $scope.cpu.series = [{ data: cpuSeries, label: "&nbsp;CPU Utilization", color: "#f7921d" }];
                    $scope.core.series = [
                        { data: coreSeries[0], label: "&nbsp;Total Cores", color: "LightGreen" },
                        { data: coreSeries[1], label: "&nbsp;Used Cores", color: "LightPink" }
                    ];
                    $scope.memory.series = [
                        { data: memorySeries[0], label: "&nbsp;Total Memory", color: "LightGreen" },
                        { data: memorySeries[1], label: "&nbsp;Used Memory", color: "LightPink" }
                    ];
                });
            };

            var init = function () {
                // load the chart history of the last 24 hours
                var nowDate = new Date();
                var startDate = new Date();
                startDate.setTime(nowDate.getTime() - (24 * 60 * 60 * 1000));
                dataService.getStatusHistory({ start: ConvertDate(startDate), end: ConvertDate(nowDate) }, function (status) {
                    var noOfStatus = status.length;
                    var cpuSeries = [];
                    var coreSeries = [[], []];
                    var memorySeries = [[], []];
                    for (var i = 0; i < noOfStatus; ++i) {
                        var curStatus = status[i];
                        var cpuData = Math.round(curStatus.CpuUtilizationStatus.TotalCpuUtilization);
                        cpuSeries.push([curStatus.Timestamp, cpuData]);
                        coreSeries[0].push([curStatus.Timestamp, curStatus.CoreStatus.TotalCores]);
                        coreSeries[1].push([curStatus.Timestamp, curStatus.CoreStatus.UsedCores]);
                        memorySeries[0].push([curStatus.Timestamp, Math.round(curStatus.MemoryStatus.TotalMemory / 1024)]);
                        memorySeries[1].push([curStatus.Timestamp, Math.round(curStatus.MemoryStatus.UsedMemory / 1024)]);
                    }
                    $scope.cpu.series = [{ data: cpuSeries, label: "&nbsp;CPU Utilization", color: "#f7921d" }];
                    $scope.core.series = [
                        { data: coreSeries[0], label: "&nbsp;Total Cores", color: "LightGreen" },
                        { data: coreSeries[1], label: "&nbsp;Used Cores", color: "LightPink" }
                    ];
                    $scope.memory.series = [
                        { data: memorySeries[0], label: "&nbsp;Total Memory", color: "LightGreen" },
                        { data: memorySeries[1], label: "&nbsp;Used Memory", color: "LightPink" }
                    ];
                    updateStatus();
                });
            };
            init();
            // set interval and stop updating when changing location
            $scope.updateInterval = $interval(updateStatus, $scope.interval);
            var cancelInterval = $scope.$on('$locationChangeSuccess', function () {
                $interval.cancel($scope.updateInterval);
                cancelInterval();
            });
        }]
    );
})();