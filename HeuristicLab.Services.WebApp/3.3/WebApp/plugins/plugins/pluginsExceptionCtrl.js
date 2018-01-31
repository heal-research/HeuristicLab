(function () {
    var module = appPluginsPlugin.getAngularModule();
    module.controller('app.plugins.pluginsExceptionCtrl',
        ['$scope', '$modalInstance', 'pluginName', 'exception',
            function ($scope, $modalInstance, pluginName, exception) {
                $scope.pluginName = pluginName;
                $scope.exception = exception;
                $scope.close = function () {
                    $modalInstance.dismiss('cancel');
                };
            }
        ]
    );
})();