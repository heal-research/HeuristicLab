(function () {
    var module = appMainPlugin.getAngularModule();
    module.controller('app.menu.ctrl',
        ['$scope', '$timeout', '$location', '$window', 'app.authentication.service',
        function ($scope, $timeout, $location, $window, authService) {
            $scope.modules = app.modules;
            $scope.menuEntries = app.getMenu().getMenuEntries();
            $scope.isActive = function (viewLocation) {
                var loc = viewLocation.toUpperCase().substr(1);
                var actualLocation = $location.path().toUpperCase();
                var splitLoc = loc.split("/");
                if (splitLoc.length <= 2) {
                    var actualLocationSplit = actualLocation.split("/");
                    if (actualLocationSplit.length > 1) {
                        if (splitLoc[1] == actualLocationSplit[1]) {
                            return true;
                        }
                    }
                }
                return $location.path().toUpperCase() == loc;
            };

            $scope.logout = function() {
                authService.logout({}, function() {
                    $window.location.hash = "";
                    $window.location.reload();
                });
            };

            $scope.hideMenu = function() {
                $(".navbar-collapse").collapse('hide');
            };
        }]
    );
})();