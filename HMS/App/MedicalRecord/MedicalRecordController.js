/// <reference path="../../Lib/ag-grid-community/dist/ag-grid-community.js" />
(function () {

    let MedicalRecordController = function ($scope, MedicalRecordService, $timeout, toaster, ngDialog) {

        let vm = this;
        vm.DocMaster = {};
        vm.DocDetail = [];
        vm.selectedMaster = 'N';
        let toastLiveExample = document.getElementById('alert');
        ///====================================   Private  =========================================

        /// Private Fields
        let docMasterColumnDefs = [
            { headerName: "DocMstId*", field: "DocMstId", width: 150, sortable: true, resizable: true },
            { headerName: "IPDNo", field: "IPDNo", width: 180, sortable: true, resizable: true },
            { headerName: "OPDNo", field: "OPDNo", width: 100, sortable: true, resizable: true },
            { headerName: "Category", field: "Category.Name", width: 280, sortable: true, resizable: true },
            { headerName: "DocType", field: "DocType", width: 150, sortable: true, resizable: true },
            { headerName: "UserName", field: "UserName", width: 150, sortable: true, resizable: true }
        ];

        let docDetailColumnDefs = [
            //{ headerName: "AttributeId", field: "AttributeId", width: 150 },
            { headerName: "DocDetailId", field: "DocDetailId", width: 150, sortable: true, resizable: true },
            { headerName: "DocMstId", field: "DocMstId", width: 150, sortable: true, resizable: true },
            { headerName: "DocPath", field: "DocPath", width: 150, sortable: true, resizable: true },
            { headerName: "Remark", field: "Remark", width: 150, sortable: true, resizable: true },
            { headerName: "UserName", field: "UserName", width: 150, sortable: true, resizable: true }
        ];
        /// Private Methods

        function initialise() {
            vm.selectedMaster = 'N';
            PopulateData(null, null);
        }

        function docMasterRowSelected() {
            let rows = vm.docMasterGridOptions.api.getSelectedRows();
            vm.docMasterModel = rows[0];
            vm.selectedMaster = 'Y';
            $scope.$apply(function () {
                if (vm.docMasterBlockMode === 'Edit') {
                }
                else if (vm.docMasterBlockMode === 'Add' && vm.isDocMasterVisible) {
                    vm.docMasterModel = { Detail: rows[0].Detail };
                    clearAutoCompleteByClass('DocMaster', $scope);
                }
            });
        };

        function docDetailRowSelected() {
            let rows = vm.docDetailGridOptions.api.getSelectedRows();
            $scope.$apply(function () {
                if (vm.docDetailBlockMode === 'Edit') {
                    vm.docDetailModel = rows[0];
                    //if (rows[0].ToDate)
                    //    vm.attributeDetailModel.ToDate = new Date(rows[0].ToDate);
                }
                else {
                    vm.docDetailModel = {};
                    clearAutoCompleteByClass('DocDetail', $scope);
                }
            });
        };

        function editableBlockHandler(row, mode) {
            let table = row.GridName || row;
            switch (table) {
                case "DocMaster":
                    if (mode === 'Close')
                        vm.isDocMasterVisible = false;
                    else
                        vm.isDocMasterVisible = true;
                    if (mode === 'Add') {
                        vm.docMasterModel = {};
                        //if (vm.attributeGroup) {
                        //    vm.attributeMasterModel = { Group: vm.attributeGroup };
                        //}
                        clearAutoCompleteByClass(table, $scope);
                    }
                    else {
                        mode = 'Edit';
                        vm.docMasterModel = row;
                        setInputValue('Category', vm.docMasterModel.Category);
                    }
                    vm.docMasterBlockMode = mode;
                    break;

                case "DocDetail":
                    if (mode === 'Close')
                        vm.isDocDetailVisible = false;
                    else
                        vm.isDocDetailVisible = true;
                    if (mode === 'Add') {
                        vm.docDetailModel = {};
                        clearAutoCompleteByClass(table, $scope);
                    }
                    else {
                        mode = 'Edit';
                        vm.docDetailModel = row;
                        //if (row.AttributeDate)
                        //    vm.attributeDetailModel.AttributeDate = new Date(row.AttributeDate);
                    }
                    vm.docDetailBlockMode = mode;
                    break;
            }
            if (mode === 'Close')
                vm.scrollUp();
        };

        function saveBlockChanges(blockName, mode, data) {
            let gridOption;
            let model;
            let index = 0;
            let obj = {};
            let vav = {};
            data.GridName = blockName;
            switch (blockName) {
                case "DocMaster":
                    gridOption = vm.docMasterGridOptions;
                    UpdateRowState(function () {
                        if (!vm.docMaster)
                            vm.docMaster = [];
                        index = findObjectIndex(vm.docMaster, 'GUID', data.GUID);
                        if (!vm.docMasterModel.Category) {
                            vm.toastDisplay(toastLiveExample, 'Please Enter Category', 'Warning');
                            return;
                        }
                        if (!data.Detail) {
                            data.Detail = [];
                        }
                        if (index >= 0) {
                            vm.docMaster[index] = data;
                        }
                        else {
                            vm.docMaster.push(data);
                        }
                        gridOption.api.setRowData(vm.docMaster);
                        vm.isDocMasterVisible = false;
                        filterDeletedRow(gridOption);
                        let allControls = 'form-control';
                        clearAutoCompleteByClass(allControls, $scope);
                        vm.save();
                    });
                    break;

                case "DocDetail":
                    gridOption = vm.docDetailGridOptions;
                    UpdateRowState(function () {
                        index = findObjectIndex(vm.docMasterModel.Detail, 'GUID', data.GUID);
                        data.DocMstId = vm.docMasterModel.DocMstId;
                        if (index >= 0) {
                            vm.docMasterModel.Detail[index] = data;
                        }
                        else {
                            vm.docMasterModel.Detail.push(data);
                        }
                        gridOption.api.setRowData(vm.docMasterModel.Detail);
                        vm.isDocDetailVisible = false;
                        filterDeletedRow(gridOption);
                        vm.save();
                    });
                    break;
            }

            function UpdateRowState(callback) {
                switch (mode) {
                    case "Add":
                        data.RowState = DataRowState.Added;
                        data.GUID = createGuId();
                        break;
                    case "Edit":
                        data.RowState = DataRowState.Modified;
                        break;
                    case "Delete":
                        data.RowState = DataRowState.Deleted;
                        break;
                }
                callback();
            }
        };

        function filterDeletedRow(gridOptions) {
            gridOptions.api.onFilterChanged();
        };

        function setInputValue(id, value) {
            $timeout(function () {
                $scope.$broadcast('angucomplete-alt:changeInput', id, value);
            }, 200);
        };

        function PopulateData(IPDNo, OPDNo) {
            MedicalRecordService.MRDQuery('DocMaster', IPDNo, OPDNo).then(
                function (result) {
                    vm.docMaster = result.data;
                    if (vm.selectedDoc) {
                        $timeout(function () {
                            vm.docMasterGridOptions.api.forEachNode(function (node, index) {
                                if (node.data.DocMstId === vm.selectedDoc.DocMstId) {
                                    node.setSelected(true, false);
                                    vm.docMasterModel = vm.selectedDoc;
                                }
                            }, 5000, true);
                        });
                    }
                });
        };

        ///====================================    Public   =========================================
        vm.docMasterGridOptions = new gridOptions(docMasterColumnDefs, docMasterRowSelected, null);
        vm.docDetailGridOptions = new gridOptions(docDetailColumnDefs, docDetailRowSelected, null);

        vm.editableBlockHandler = editableBlockHandler;
        vm.saveBlockChanges = saveBlockChanges;

        vm.filterGrid = function (searchText, grid) {
            grid.api.setQuickFilter(searchText);
        };

        vm.filterValue = function (searchText, gridOptions) {
            let ageFilterComponent = gridOptions.api.getFilterInstance('AttributeValue');
            ageFilterComponent.setModel({
                type: 'startsWith',
                filter: searchText
            });
            gridOptions.api.onFilterChanged();
        };

        vm.scrollDown = function () {
            $("html,body").animate({ scrollTop: $(document).height() }, "slow");
        };

        vm.scrollUp = function () {
            $("html,body").animate({ scrollTop: 0 }, "slow");
        };

        vm.scrollDown = function (Doc) {
            $("html,body").animate({ scrollTop: $('#' + Doc).height() }, "slow");
        };

        /// Public Fields
        vm.GetData = function (IPD, OPD) {
            PopulateData(IPD, OPD);
        };

        vm.save = function () {
            let data = vm.docMaster;
            if (vm.docMasterGridOptions.api.getSelectedNodes().length > 0)
                vm.selectedDoc = vm.docMasterGridOptions.api.getSelectedNodes()[0].data;
            MedicalRecordService.SaveData(data)
                .then(function (result) {
                    toaster.pop(result.data);
                    let allControls = 'form-control';
                    clearAutoCompleteByClass(allControls, $scope);
                    vm.selectedMaster = 'N';
                    vm.toastDisplay(toastLiveExample, result.data.body, 'Success');
                    initialise();
                    //window.location.reload();
                }, function (error) {
                    vm.toastDisplay(toastLiveExample, error.data.body, 'Error');
                });
        };

        vm.toastDisplay = function (toastLiveExample, body, title) {
            let toast = new bootstrap.Toast(toastLiveExample);
            vm.Body = body;
            vm.Title = title;
            toast.show();
        };

        vm.clearAll = function () {
            window.location.reload();
        };

        vm.SearchType = function (searchString, type) {
            return MedicalRecordService.GetListOfMRD(type, searchString).$promise;
        };

        /// Public Methods

        //=====================================    Watch    =========================================
        $scope.$watchCollection('vm.docMasterModel.Detail', function (newValue, oldValue) {
            vm.docDetailGridOptions.api.setRowData(newValue);
            $timeout(function () {
                vm.docDetailGridOptions.api.sizeColumnsToFit();
            }, 500, true);
        });

        $scope.$watchCollection('vm.docMaster', function (newValue, oldValue) {
            vm.docMasterGridOptions.api.setRowData(newValue);
            $timeout(function () {
                vm.docMasterGridOptions.api.sizeColumnsToFit();
            }, 500, true);
        });

        //$scope.$watch('vm.attributeGroup', function (newValue, oldValue) {
        //    if (newValue) {
        //        PopulateData(newValue.Name);
        //        vm.attributeDetailGridOptions.api.setRowData([]);
        //    }
        //});

        ///==================================== Code To Run =========================================
        initialise();
    };
    hospitalApp.controller("MedicalRecordController", ['$scope', 'MedicalRecordService', '$timeout', 'toaster', 'ngDialog', MedicalRecordController]);
})();

function clearAutoCompleteByClass(allControls, $scope) {
    $('.' + allControls).each(function (index, val) {
        let id = $(val).attr('id');
        if (id) {
            id = id.replace('_value', '');
            $scope.$broadcast('angucomplete-alt:clearInput', id);
        }
    });
}
class gridOptions {
    constructor(columnDefs, RowSelectedEvent, RowDoubleClickedEvent, isDocMaster = true) {
        var defaultCol = [
            { headerName: "Edit", field: "Edit", width: 100, cellRenderer: 'editButtonCellRenderer', pinned: 'right' },
            { headerName: "RowState", field: "RowState", hide: true },
            { headerName: "Id", field: "Id", hide: true },
            { headerName: "DocMstId", field: "DocMstId", hide: true },
            { headerName: "Guid", field: "GUID", hide: true },
            { headerName: "isValid", field: "IsValid", width: 200, hide: true, sort: 'asc' }
        ];

        $.each(defaultCol, function (index, val) {
            if (index === 0) {
                if (isDocMaster)
                    columnDefs.push(val);
            }
            else
                columnDefs.push(val);
        });
        this.animateRows = true;
        this.onFilterChanged = function () { 'onFilterChanged'; };
        this.columnDefs = columnDefs;
        this.rowData = null;
        this.angularCompileRows = true;
        this.angularCompileRows = true;
        this.allowContextMenuWithControlKey = true;
        this.onRowSelected = RowSelectedEvent;
        this.onRowDoubleClicked = RowDoubleClickedEvent;
        this.onCellFocused = function (param) {
            if (param) {
                var row = this.api.getModel().getRow(param.rowIndex);
                if (row) {
                    if (row.setSelected)
                        row.setSelected(true, false);
                }
            }
        };
        this.rowHeight = 28,
            this.components = {
                'dateCellRenderer': fromDateCellRenderer,
                'editButtonCellRenderer': EditButtonCellRenderer
            };
        this.defaultColDef = function () {
            return {
                sortable: true,
                suppressMovable: false,
                //filter: true,
                resizable: true
            };
        };
        this.rowClassRules = {
            // row style function
            'ag-row-error': function (params) {
                return params.data.IsValid === false;
            },
            'ag-row-parent-row': function (params) {
                return params.data.isParentRow === true;
            }
        };
        this.isExternalFilterPresent = isExternalFilterPresent;
        this.doesExternalFilterPass = doesExternalFilterPass;
    }
}
function isExternalFilterPresent() {
    return true;
}
function doesExternalFilterPass(node) {
    return node.data.RowState !== 8;
}
function fromDateCellRenderer() { }

fromDateCellRenderer.prototype.init = function (params) {
    if (params.data.FromDate) {
        this.eGui = document.createElement('span');
        let text = getFormattedDate(params.data.FromDate);
        this.eGui.innerHTML = text;
    }
    if (params.data.ToDate) {
        this.eGui = document.createElement('span');
        let text = getFormattedDate(params.data.ToDate);
        this.eGui.innerHTML = text;
    }
};
function EditButtonCellRenderer(params) {
    var vm = params.$scope.$parent.$parent.vm;
    return "<button class=\"btn btn-primary float-right mb-1  btn-sm\" ng-disabled=\"vm.global.formMode == 'View'\" ng-click=\"vm.editableBlockHandler(data)\"><span class=\"fa fa-pencil\"> Edit</span></button>";
}

function getFormattedDate(dateText) {
    var date = new Date(dateText);
    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    var yyyy = date.getFullYear();
    if (dd < 10)
        dd = '0' + dd;
    if (mm < 10)
        mm = '0' + mm;
    return dd + '/' + mm + '/' + yyyy;
}
const DataRowState = {
    Detached: 1,
    Unchanged: 2,
    Added: 4,
    Deleted: 8,
    Modified: 16
};
const DocType = {
    Predefined: 100,
    Additional: 200
};
function findObjectByKey(array, key, value) {
    if (array) {
        for (var i = 0; i < array.length; i++) {
            if (array[i][key] === value) {
                return array[i];
            }
        }
    }
    return null;
}
function findObjectIndex(array, key, value) {
    if (array) {
        for (var i = 0; i < array.length; i++) {
            if (array[i][key] === value) {
                return i;
            }
        }
    }
    return -1;
}
function createGuId() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return `${s4()}${s4()}-${s4()}-${s4()}-${s4()}-${s4()}${s4()}${s4()}`;
}