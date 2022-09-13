"use strict";
hospitalApp.directive('isNumber', [function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            element.on('keypress', function (e) {
                var char = e.char || String.fromCharCode(e.charCode);
                if (isNaN(char)) {
                    e.preventDefault();
                    return false;
                }
            });
        }
    };
}]);

hospitalApp.directive('numbers', [function () {
    return {
        require: 'ngModel',
        restrict: 'A',
        link: function (scope, element, attrs, ctrl) {
            function inputValue(val) {
                if (val) {
                    var digits = val.replace(/[^0-9].[^0-9]/g, '');
                    if (digits !== val) {
                        ctrl.$setViewValue(digits);
                        ctrl.$render();
                    }
                    return parseFloat(digits);
                }
                return undefined;
            }
            ctrl.$parsers.push(inputValue);
        }
    };
}]);

hospitalApp.directive('isDate', [function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            var model = {
                scope, element, attrs, ctrl
            };
            element.on('blur', function (e) {
                var dt = $(this).val();
                if (!dt.match(/^([0-2][0-9|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$/i)) {
                    $(this).addClass('ag-row-error');
                    $(this).val('');
                    model.ctrl.$setViewValue(null);
                }
                else {
                    $(this).removeClass('ag-row-error');
                }
            });
        }
    };
}]);