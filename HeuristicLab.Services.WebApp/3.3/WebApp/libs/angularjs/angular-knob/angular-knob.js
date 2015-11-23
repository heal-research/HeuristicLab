angular.module('ui.knob', []).directive('knob', ['$timeout', function ($timeout) {
    'use strict';

    return {
        restrict: 'EA',
        replace: true,
        template: '<input value="{{ knobData }}"/>',
        scope: {
            knobData: '=',
            knobOptions: '&'
        },
        link: function ($scope, $element) {
            var knobInit = $scope.knobOptions() || {};

            knobInit.release = function (newValue) {
                $timeout(function () {
                    $scope.knobData = newValue;
                    $scope.$apply();
                });
            };

            $scope.IsAnimating = false;
            $scope.$watch('knobData', function (newValue, oldValue) {
                if (newValue != oldValue && $scope.IsAnimating == false) {
                    $scope.IsAnimating = true;
                    $($element).val(oldValue);
                    $($element).animate({
                        value: parseInt(newValue)
                    }, {
                        duration: 500,
                        easing: 'swing',
                        progress: function () {
                            $(this).val(Math.round(this.value)).change();
                        },
                        complete: function () {
                            $scope.IsAnimating = false;
                        }
                    });
                }
            });

            $($element).val($scope.knobData).knob(knobInit);
        }
    };
}]);