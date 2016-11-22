(function () {
    var module = appStatisticsPlugin.getAngularModule();
    module.controller('app.statistics.groupDetailsCtrl',
        ['$scope', '$stateParams', '$interval', 'app.statistics.clientService', 'app.statistics.groupService',
        function ($scope, $stateParams, $interval, clientService, groupService) {
            $scope.interval = defaultPageUpdateInterval;
            $scope.curClientsPage = 1;
            $scope.clientsPageSize = 20;


            // details
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

            $scope.knobData = {
                cores: 0,
                cpu: 0,
                memory: 0
            };

            var getGroupDetails = function() {
                groupService.getGroupDetails({ id: $stateParams.id }, function (group) {
                    $scope.group = group;
                    $scope.knobData.cores = (group.UsedCores / group.TotalCores) * 100;
                    $scope.knobData.cpu = group.ActiveCpuUtilization;
                    $scope.knobData.memory = (group.UsedMemory / group.TotalMemory) * 100;

                    var length = group.TasksStates.length;
                    var total = 0;
                    for (var i = 0; i < length; ++i) {
                        total += group.TasksStates[i].Count;
                    }
                    $scope.totalGroupTasks = total;
                });
            };

            var getClients = function () {
                clientService.getClientsByGroupId({id: $stateParams.id, page: $scope.curClientsPage, size: $scope.clientsPageSize, expired: false },
                    function (clientPage) {
                        $scope.clientPage = clientPage;
                    }
                );
            };

            // charts
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
                { id: 4, name: 'Last 30 Days' },
                { id: 5, name: 'Last 6 Months' },
                { id: 6, name: 'Last Year' }
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
                    case 5:
                        var month = today.getMonth() - 6;
                        if (month < 0) {
                            month += 12;
                        }
                        $scope.fromDate = new Date(today.valueOf());
                        $scope.fromDate.setMonth(month);
                        $scope.toDate = new Date(today.valueOf());
                        break;
                    case 6:
                        $scope.fromDate = new Date(today.valueOf());
                        $scope.fromDate.setFullYear(today.getFullYear() - 1);
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
                clientService.getClientHistoryByGroupId({ id: $stateParams.id, start: ConvertFromDate($scope.fromDate), end: ConvertToDate($scope.toDate) }, function (status) {
                    var noOfStatus = status.length;
                    var cpuSeries = [];
                    var coreSeries = [[], []];
                    var memorySeries = [[], []];
                    for (var i = 0; i < noOfStatus; ++i) {
                        var curStatus = status[i];
                        var cpuData = Math.round(curStatus.CpuUtilization);
                        cpuSeries.push([curStatus.Timestamp, cpuData]);
                        coreSeries[0].push([curStatus.Timestamp, curStatus.TotalCores]);
                        coreSeries[1].push([curStatus.Timestamp, curStatus.UsedCores]);
                        memorySeries[0].push([curStatus.Timestamp, curStatus.TotalMemory]);
                        memorySeries[1].push([curStatus.Timestamp, curStatus.UsedMemory]);
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

            var update = function () {
                getGroupDetails();
                getClients();
            };

            $scope.updateInterval = $interval(update, $scope.interval);
            var cancelInterval = $scope.$on('$locationChangeSuccess', function () {
                $interval.cancel($scope.updateInterval);
                cancelInterval();
            });
            update(); // init page
            $scope.updateCharts();
        }]
    );
})();