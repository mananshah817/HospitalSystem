/// <reference path="../app.js" />
(function () {
    hospitalApp.service('DocumentService', function ($resource, $http) {
        return {
            Query: function (QuertType, Pval1, Pval2, Pval3) {
                var config = {
                    headers: {
                        'Pval1': Pval1,
                        'Pval2': Pval2,
                        'Pval3': Pval3
                    }
                };
                return $http.get(`/api/Yojana/${QuertType}`, config);
            },
            saveData: function (patient, type) {
                var config = {
                    headers: {
                        'Type': type
                    }
                };
                return $http.post('/api/Yojana/SaveData/', patient, config);
            },
            uploadDoc: function (item, folder, files) {
                var config = {
                    transformRequest: angular.identity,
                    headers: {
                        'MetaData': JSON.stringify(item),
                        'Folder': folder,
                        'Content-Type': undefined
                    }
                };
                return $http.post('/api/Yojana/UploadDoc/', files, config);
            },
            Delete: function (patient) {
                return $http.post('/api/Yojana/Delete/Patient', patient);
            }
        };
    });
})();
