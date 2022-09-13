//import { Array } from "core-js/library/web/timers";

"use strict";
hospitalApp.directive('fileModel', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            element.on('change', function (e) {
                var files;
                files = [];
                if (attrs.asFile !== void 0) {
                    files = element[0].files;
                } else {
                    angular.forEach(element[0].files, function (item) {
                        var file;
                        file = {
                            name: item.name,
                            size: item.size,
                            type: item.type,
                            lastModified: item.lastModified,
                            lastModifiedDate: item.lastModifiedDate,
                            url: URL.createObjectURL(item),
                            _file: item
                        };
                        files.push(file);
                    });
                }
                ngModel.$setViewValue(files);
            });
        }
    };
});

hospitalApp.directive('agNavigableTable', function () {
    return {
        restrict: 'A',
        scope: {
            agNavigableTable: '@'
        },
        link: function (scope, el) {
            var firstVisibleInput = function (node, tdIndex) {
                
                if (tdIndex !== undefined) {
                    node = node.children[tdIndex] //todo what if there are varying colspans?          
                }
                if (node) {
                    if (node.getElementsByTagName('input').length > 1) {
                        let list = Array.from(node.getElementsByTagName('input'));
                        return list[list.length - 1];
                    }
                    var descendants = node.getElementsByTagName('*')
                    for (var i = 0; i < descendants.length; i++) {
                        if (descendants[i].nodeName === 'INPUT'
                            && descendants[i].offsetHeight > 0 && descendants[i].offsetWidth > 0 //the input is actually visible
                        ) {
                            return descendants[i]
                        }
                    }
                }
                return undefined
            };


            var disableLeftRight = false

            if (scope.agNavigableTable === 'excel') {
                el.bind('dblclick', function () {
                    disableLeftRight = true
                    document.activeElement.onblur = function () {
                        disableLeftRight = false
                    }
                })
            }


            el.bind('keydown', function (event) {
                var keys = { left: 37, up: 38, right: 39, down: 40 }
                var key = event.which

                if (scope.agNavigableTable === 'excel') {
                    if (key === 13) {
                        key = 40 //in excel mode, treat the enter key like the down arrow
                    }
                } else {
                    if (key === 13) {
                        disableLeftRight = !disableLeftRight //in standard mode, the enter key disables the left/right arrows
                        document.activeElement.onblur = function () {
                            disableLeftRight = false
                        }
                    }
                }

                //start at the currently focused element, must be an input for this to continue
                var startInput = document.activeElement
                if (startInput.nodeName !== 'INPUT') return;


                var destinationInput
                var node = startInput

                //walk along the DOM, looking for the destination input 

                //look for the startInput's TD
                do {
                    node = node.parentNode
                } while (node && node.nodeName !== 'TD')
                if (!node) return //ill-formed html

                if (!disableLeftRight && (key === keys.left || key === keys.right)) {
                    event.preventDefault()
                    //walk along the TDs until we find one that has a visible input within it         
                    do {
                        if (node.getElementsByTagName('input').length == 1) {
                            node = (key === keys.left ? node.previousElementSibling : node.nextElementSibling);
                            if (node && node.nodeName === 'TD') {
                                destinationInput = firstVisibleInput(node);
                            }
                            else return //no more TDs available - we're done here
                        }
                        else if (node.getElementsByTagName('input').length > 1) {
                            
                            let list = Array.from(node.getElementsByTagName('input'));
                            let index = list.indexOf(document.activeElement);
                            let newIndex = (key === keys.left ? index - 1 : index + 1);
                            if (list.length == newIndex) {
                                node = (key === keys.left ? node.previousElementSibling : node.nextElementSibling);
                                if (node && node.nodeName === 'TD') {
                                    destinationInput = firstVisibleInput(node);
                                }
                            }
                            else {
                                newIndex = newIndex < 0 ? 0 : newIndex;
                                destinationInput = list[newIndex];
                            }
                        }
                        else return;
                        
                    } while (!destinationInput)
                }
                else if (key === keys.up || key === keys.down) {
                    event.preventDefault()
                    var tdIndex = node.cellIndex;

                    do { //find the TR
                        node = node.parentNode
                    } while (node && node.nodeName !== 'TR')
                    if (!node) return //ill-formed html

                    do {
                        if (node.getElementsByTagName('input').length == 1) {
                            node = (key === keys.up ? node.previousElementSibling : node.nextElementSibling);
                            if (node && node.nodeName === 'TR') {
                                destinationInput = firstVisibleInput(node, tdIndex)
                            }
                            else return //no more rows or ill-formed html
                        }
                        else if (node.getElementsByTagName('input').length > 1) {
                            let list = Array.from(node.getElementsByTagName('input'));
                            let index = list.indexOf(document.activeElement);
                            node = (key === keys.up ? node.previousElementSibling : node.nextElementSibling);
                            list = Array.from(node.getElementsByTagName('input'));
                            destinationInput = list[index];
                        }
                    } while (!destinationInput)

                }

                if (destinationInput) {
                    destinationInput.focus()
                }


            })
        }
    }
});