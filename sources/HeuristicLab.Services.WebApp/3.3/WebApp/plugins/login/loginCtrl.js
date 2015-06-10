(function () {
    var module = appLoginPlugin.getAngularModule();
    module.controller('app.login.ctrl',
        ['$scope', '$window', 'app.authentication.service', function ($scope, $window, authService) {
            $scope.login = function () {
                authService.login($scope.user, function (data) {
                    if (!isDefined(data["Username"]) && !isDefined(data["Password"])) {
                        $window.location.hash = "";
                        $window.location.reload();
                    } else {
                        $scope.result = "error logging in";
                    }
                });
            };
        }]
    );
})();