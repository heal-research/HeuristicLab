(function () {
    var module = appMainPlugin.getAngularModule();
    module.controller('app.menu.ctrl',
        ['$scope', '$timeout', '$location', '$window', 'app.authentication.service',
        function ($scope, $timeout, $location, $window, authService) {
            $scope.modules = app.modules;
            $scope.menuEntries = app.getMenu().getMenuEntries();
            $scope.isActive = function (viewLocation) {
                var linkLocation = viewLocation.toUpperCase().substr(1);
                var currentLocation = $location.path().toUpperCase();
                if (linkLocation == currentLocation) {
                    return true;
                }
                var linkLocationParts = linkLocation.split("/");
                var currentLocationParts = currentLocation.split("/");
                var linkLocationPartsLength = linkLocationParts.length;
                if (linkLocationPartsLength < currentLocationParts.length) {
                    for (var i = 0; i < linkLocationPartsLength; ++i) {
                        if (linkLocationParts[i] !== currentLocationParts[i]) {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            };

            $scope.logout = function () {
                authService.logout({}, function () {
                    $window.location.hash = "";
                    $window.location.reload();
                });
            };

            $scope.hideMenu = function () {
                $(".navbar-collapse").collapse('hide');
            };
        }]
    );
})();