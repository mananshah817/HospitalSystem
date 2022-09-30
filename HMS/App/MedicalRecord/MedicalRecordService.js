(function () {
    hospitalApp.service('MedicalRecordService', function ($resource, $http) {
        return {
            MRDQuery: function (QuertType, Pval1, Pval2, Pval3, Pval4, Pval5, Pval6) {
                var config = {
                    headers: {
                        'Pval1': Pval1,
                        'Pval2': Pval2,
                        'Pval3': Pval3,
                        'Pval4': Pval4,
                        'Pval5': Pval5,
                        'Pval6': Pval6
                    }
                };
                return $http.get(`/api/Home/MedicalRecord/${QuertType}`, config);
            },
            GetListOfMRD: function (type, search) {
                if (type) {
                    var List = $resource('/api/Home/MedicalRecord/ListOfValue/:type', { type: '@type' }, {
                        query: {
                            method: 'get',
                            isArray: true,
                            headers: {
                                'filter': search
                            }
                        }
                    });
                    return List.query({ type: type });
                }
            },
            SaveData: function (data) {
                return $http.post('/api/Home/MedicalRecord/SaveData/', data);
            },
            uploadDoc: function (item, files) {
                var config = {
                    transformRequest: angular.identity,
                    headers: {
                        'MetaData': JSON.stringify(item),
                        'Content-Type': undefined
                    }
                };
                return $http.post('/api/Home/MedicalRecord/Upload/', files, config);
            },
            //delete: function (patient) {
            //    return $http.post('/api/Home/MedicalRecord/Delete/Patient', patient);
            //},
            //search: function (patient) {
            //    return $http.post('/api/home/MedicalRecord/Search', patient);
            //}
        };
    });
})();