(function () {
    var module = appAboutPlugin.getAngularModule();
    module.controller('app.about.ctrl', ['$scope', function($scope) {
        $scope.mailToSupport = function () {
            location.href = decryptString('U2FsdGVkX1/pCOITUzzsN36hx4sHh11FeVXkVyQ5b2KeZebFQ3KaNN8G9bKL3lU9');
        };

        $scope.mailToStefanWagner = function () {
            location.href = decryptString('U2FsdGVkX1/Lzu8UFltiBl6VmBf9E0lmGna0+7o7cavCPwiCytBpSLsJoyhO9tl5hHCvJPVgsSndWdmGEuWrXw==');
        };
    }]);
})();