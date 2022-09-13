"use strict";
hospitalApp.directive('datetimepicker', [function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            console.log('call datetimepicker link..');
            var picker = element.datetimepicker({
                dateFormat: 'dd/MM/yyyy hh:mm'
            });
            ctrl.$render(function () {
                console.log('ctrl.$viewValue@' + ctrl.$viewValue);
                picker.setDate(ctrl.$viewValue || '');
            });
            picker.on('dp.change', function (e) {
                console.log('dp.change' + e.date);
                scope.$apply(function () {
                    ctrl.$setViewValue(e.date);
                });
            });
        }
    };
}]);

"use strict";
hospitalApp.directive('psDatetimePicker', [function (moment) {
    var format = 'MM/DD/YYYY hh:mm';
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            element.datetimepicker({
                format: format
            });
            var picker = element.date("DateTimePicker");
            ctrl.$formatters.push(function (value) {
                var date = moment(value);
                if (date.isValid()) {
                    return date.format(format);
                }
                return '';
            });
            element.on('change', function () {
                var date = picker.getDate();
                ctrl.$setViewValue(date.valueOf());
            });
        }
    };
}]);
