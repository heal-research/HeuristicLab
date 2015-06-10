(function () {
    var module = appMainPlugin.getAngularModule();
    module.directive('flot', function () {
        return {
            restrict: 'E',
            link: function (scope, element, attrs) {
                var chart = null, opts = scope[attrs.options];

                scope.$watchCollection(attrs.dataset, function (newData, oldData) {
                    if (!chart) {
                        var div = element.append("<div>");
                        div.bind("plotselected", function (event, ranges) {
                            $.each(chart.getXAxes(), function (_, axis) {
                                var opts2 = axis.options;
                                opts2.min = ranges.xaxis.from;
                                opts2.max = ranges.xaxis.to;
                            });
                            chart.setupGrid();
                            chart.draw();
                            chart.clearSelection();
                        });
                        div.dblclick(function () {
                            var axes = chart.getAxes();
                            var xaxis = axes.xaxis.options;
                            xaxis.min = null;
                            xaxis.max = null;
                            chart.setupGrid();
                            chart.draw();
                        });
                        div.addClass('chart');
                        div.addClass('angular-flot-chart');
                        chart = $.plot(div, newData, opts);
                        div.resizable({});
                        div.show();
                    } else {
                        var axes = chart.getAxes();
                        var xaxis = axes.xaxis.options;
                        xaxis.min = null;
                        xaxis.max = null;
                        chart.setData(newData);
                        chart.setupGrid();
                        chart.draw();
                    }
                });
            }
        };
    });
})();