/// <reference path="../../lib/angular/angular.js" />

"use strict";
accountApp.directive('vngMessageBox', ['$q', '$parse', '$http', '$sce', '$timeout', '$templateCache', '$interpolate', function ($q, $parse, $http, $sce, $timeout, $templateCache, $interpolate) {
    return {
        scope: {
            selectedObject: '=',
            selectedObjectData: '=',
            disableInput: '=',
            initialValue: '=',
            localData: '=',
            localSearch: '&',
            remoteUrlRequestFormatter: '=',
            remoteUrlRequestWithCredentials: '@',
            remoteUrlResponseFormatter: '=',
            remoteUrlErrorCallback: '=',
            remoteApiHandler: '=',
            remoteApiHandlerObject: '@',
            id: '@',
            type: '@',
            placeholder: '@',
            textSearching: '@',
            textNoResults: '@',
            remoteUrl: '@',
            remoteUrlDataField: '@',
            titleField: '@',
            descriptionField: '@',
            imageField: '@',
            inputClass: '@',
            pause: '@',
            searchFields: '@',
            minlength: '@',
            matchClass: '@',
            clearSelected: '@',
            overrideSuggestions: '@',
            fieldRequired: '=',
            fieldRequiredClass: '@',
            inputChanged: '=',
            autoMatch: '@',
            focusOut: '&',
            focusIn: '&',
            fieldTabindex: '@',
            inputName: '@',
            focusFirst: '@',
            parseInput: '&',
            addNewItem: '&',
            isAddNew: '='
        },
        templateUrl: '/app/Template/vng-auto-completeV2.html',
        compile: function (tElement) {
            var startSym = $interpolate.startSymbol();
            var endSym = $interpolate.endSymbol();
            if (!(startSym === '{{' && endSym === '}}')) {
                var interpolatedHtml = tElement.html()
                    .replace(/\{\{/g, startSym)
                    .replace(/\}\}/g, endSym);
                tElement.html(interpolatedHtml);
            }
            return link;
        }
    };
}]);