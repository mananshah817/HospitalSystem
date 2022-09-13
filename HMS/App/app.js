agGrid.initialiseAgGridWithAngular1(angular);

hospitalApp = angular.module('hospitalApp', ['ngResource', 'ngSanitize', 'agGrid', 'ngCookies', 'ngAnimate', 'toaster', 'ngDialog', '720kb.datepicker', 'angularjs-datetime-picker', 'angularjs-dropdown-multiselect']);

const ErrorType = {
    DotNet: 101,
    Oracle: 102
};
function getParameterByName(name) {
    var url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)');
    var results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}