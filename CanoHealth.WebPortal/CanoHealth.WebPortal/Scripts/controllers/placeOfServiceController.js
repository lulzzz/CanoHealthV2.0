var PlaceOfServiceController = function (pos) {
    //PRIVATE FIELDS
    var licenseFilesFromKendoUpload = [];

    var placeOfServiceExpandedRow;

    //PRIVATE METHODS
    var getLicenseFileIndex = function(itemKey) {
        return licenseFilesFromKendoUpload.findIndex(function(fileItem) {
            return fileItem.kendoUploadId === itemKey;
        });
    }

    var getPlaceOfServiceItemSuccess = function (response) {
        response.posLicenses = response.posLicenses.map(function (license) {
            return {
                posLicenseId: license.posLicenseId, //PK
                licenseTypeId: license.licenseTypeId, //FK
                placeOfServiceId: license.placeOfServiceId, //FK
                licenseName: license.licenseName,
                licenseNumber: license.licenseNumber,
                effectiveDate: moment(license.effectiveDate).format('L'),
                expireDate: moment(license.expireDate).format('L'),
                note: license.note,
                serverLocation: license.serverLocation,
                uniqueFileName: license.uniqueFileName,
                originalFileName: license.originalFileName
            }
        });
        var placeOfServiceViewModel = kendo.observable({
            posLicenses: response.posLicenses,
           
        });
        kendo.bind($(".js-license-" + response.placeOfServiceId), placeOfServiceViewModel);
    }

    var getPlaceOfServiceItem = function(placeOfServiceId) {
        pos.getPlaceOfServices(placeOfServiceId, getPlaceOfServiceItemSuccess);
    }

    var processFiles = function (files) {
        //var files = kendoUploadComponent.getFiles();

        var goodFiles = files.filter(function (file) {
            return isValidFile(file.extension, file.size);
        });

        var badFiles = files.filter(function (file) {
            return !isValidFile(file.extension, file.size);
        });

        if (badFiles.length > 0)
            toastr.error("The file you are trying to upload is not valid!");

        return goodFiles;
    }

    var mapLicensesAfterReadDatasource = function(response) {
        return response.map(function (license) {
            return {
                posLicenseId: license.posLicenseId, //PK
                licenseTypeId: license.licenseTypeId, //FK
                placeOfServiceId: license.placeOfServiceId, //FK
                licenseName: license.licenseName,
                licenseNumber: license.licenseNumber,
                effectiveDate: moment(license.effectiveDate).format('L'),
                expireDate: moment(license.expireDate).format('L'),
                note: license.note,
                serverLocation: license.serverLocation,
                uniqueFileName: license.uniqueFileName,
                originalFileName: license.originalFileName,
                fileExtension: license.fileExtension,
                fileSize: license.fileSize,
                contentType: license.contentType
            }
        });
    }

    var createLicenseViewModel = function (placeOfServiceId) {
        var licenseViewModel = kendo.observable({
            onAddLicense: function() {
                var listView = $(".js-" + placeOfServiceId).data("kendoListView");
                listView.add();
            },

            onEditLicenseItem: function(e) {
                if (!e.model.isNew()) {
                    e.model.dirty = true;
                    console.log(e.model);
                }
            },

            onRemovePosLicense: function(e) {
                e.preventDefault();
               
                var releaseLicenseTemp = kendo.template($("#inactive-license-template").html());

                var window = $(".js-notification-dialog").kendoWindow({
                    title: "Confirmation",
                    modal: true,
                    visible: false, //the window will not appear before its .open method is called
                    width: "600px"
                }).data("kendoWindow");

                window.content(releaseLicenseTemp(e.model)); //send the row data object to the template and render it
                window.center().open();

                $("#js-releaseLicense-yesButton").click(function () {
                    licenseViewModel.get('licensesDataSource').remove(e.model); //prepare a "destroy" request
                    licenseViewModel.get('licensesDataSource').sync(); //actually send the request (might be ommited if the autoSync option is enabled in the dataSource)
                    window.close();
                });
                $("#js-releaseLicense-noButton").click(function () {
                    window.close();
                });

            },

            onDownloadLicenseFile: function (e) {
                if (e.data.uniqueFileName === null) {
                    toastr.error("There is not file to download");
                    return;
                } else {
                    //Download the file
                    window.location = domainName + '/GlobalFiles/Download?originalFileName=' + e.data.originalFileName +
                        '&uniqueFileName=' + e.data.uniqueFileName +
                        '&contentType=' + e.data.contentType +
                        '&serverLocation=' + e.data.serverLocation;
                }
            },

            licenseTypeDataSource: new kendo.data.DataSource({
               schema: {
                    model: {
                        id: "licenseTypeId",
                        fields: {
                            licenseTypeId: 'licenseTypeId',
                            licenseName: 'licenseName'
                        }
                    }
                },
               transport: {
                   read: {
                       url: "/api/LicenseTypes/",
                       dataType: 'json'
                   }
               }
            }),

            licensesDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: 'posLicenseId',
                        fields: {
                            posLicenseId: { editable: false },
                            placeOfServiceId: {
                                defaultValue: placeOfServiceId,
                                required: true
                            },
                            licenseTypeId: 'licenseTypeId',
                            licenseName: { required: true },
                            licenseNumber: { required: true },
                            effectiveDate: { required: true },
                            expireDate: { required: true },
                            note: 'note',
                            serverLocation: 'serverLocation',
                            uniqueFileName: 'uniqueFileName',
                            originalFileName: 'originalFileName',
                            fileExtension: 'fileExtension',
                            fileSize: 'fileSize',
                            contentType: 'contentType'
                        }
                    }
                },
                transport: {
                    read: function (options) {
                        var readPlaceOfServiceLicensesSuccess = function(response) {
                            response = mapLicensesAfterReadDatasource(response);
                            options.success(response);
                        };
                        var readPlaceOfServiceLicensesFail = function(response) {
                            options.error(response);
                        };
                        pos.readPlaceOfServiceLicenses(placeOfServiceId, readPlaceOfServiceLicensesSuccess, readPlaceOfServiceLicensesFail);
                    },
                    create: function (options) {
                        if (!formDataSupport()) {
                            licenseViewModel.get('licensesDataSource').cancelChanges();
                            return;
                        } else {
                            var formData = new FormData();
                            
                            formData.append("PlaceOfServiceId", placeOfServiceId);
                            formData.append("LicenseNumber", options.data.licenseNumber);
                            formData.append("LicenseTypeName", options.data.licenseName);
                            formData.append("EffectiveDate", moment(options.data.effectiveDate).format('L'));
                            formData.append("ExpireDate", moment(options.data.expireDate).format('L'));
                            formData.append("Note", options.data.note);
                            formData.append("Active", true);
                           
                            var placeOfServicesLicenseFiles = PlaceOfServiceController.licenseFilesFromKendoUpload.find(function (fileItem) {
                                return fileItem.kendoUploadId.indexOf(placeOfServiceId) !== -1;
                            });

                            if (placeOfServicesLicenseFiles && placeOfServicesLicenseFiles.kendoUploadFiles) {
                                var goodFiles = processFiles(placeOfServicesLicenseFiles.kendoUploadFiles);
                                if (goodFiles.length > 0) {
                                    formData.append("OriginalFileName", goodFiles[0].name);
                                    formData.append("FileExtension", goodFiles[0].extension);
                                    formData.append("FileSize", goodFiles[0].size);
                                    formData.append("fichero", goodFiles[0].rawFile, goodFiles[0].name);
                                }
                            }
                            var createPlaceOfServiceLicensesSuccess = function(response) {
                                response = {
                                    active: response.Active,
                                    contentType: response.ContentType,
                                    effectiveDate: moment(response.EffectiveDate).format('L'),
                                    expireDate: moment(response.ExpireDate).format('L'),
                                    fileExtension: response.FileExtension,
                                    FileSize: response.FileSize,
                                    licenseNumber: response.LicenseNumber,
                                    licenseName: response.LicenseTypeName,
                                    note: response.Note,
                                    originalFileName: response.OriginalFileName,
                                    placeOfServiceId: response.PlaceOfServiceId,
                                    posLicenseId: response.PosLicenseId,
                                    serverLocation: response.ServerLocation,
                                    uniqueFileName: response.UniqueFileName,
                                    uploaDateTime: response.UploaDateTime,
                                    uploadBy: response.UploadBy
                                };
                                PlaceOfServiceController.licenseFilesFromKendoUpload = [];
                                options.success(response);
                            };
                            var createPlaceOfServiceLicensesFail = function(response) {
                                console.log('Create License fails:', response);
                                toastr.error(response.statusText);
                                licenseViewModel.get('licensesDataSource').cancelChanges();
                                PlaceOfServiceController.licenseFilesFromKendoUpload = [];
                                options.error(response);
                            };
                            pos.createPlaceOfServiceLicenses(formData, createPlaceOfServiceLicensesSuccess, createPlaceOfServiceLicensesFail);
                            
                        }
                    },
                    update: function (options) {
                        if (!formDataSupport()) {
                            licenseViewModel.get('licensesDataSource').cancelChanges();
                            return;
                        } else {
                            var formData = new FormData();

                            formData.append("PosLicenseId", options.data.posLicenseId);
                            formData.append("PlaceOfServiceId", placeOfServiceId);
                            formData.append("LicenseNumber", options.data.licenseNumber);
                            formData.append("LicenseTypeName", options.data.licenseName);
                            formData.append("EffectiveDate", moment(options.data.effectiveDate).format('L'));
                            formData.append("ExpireDate", moment(options.data.expireDate).format('L'));
                            formData.append("Note", options.data.note);
                            formData.append("Active", true);

                            var placeOfServicesLicenseFiles = PlaceOfServiceController.licenseFilesFromKendoUpload.find(function (fileItem) {
                                return fileItem.kendoUploadId.indexOf(placeOfServiceId) !== -1;
                            });

                            if (placeOfServicesLicenseFiles && placeOfServicesLicenseFiles.kendoUploadFiles) {
                                var goodFiles = processFiles(placeOfServicesLicenseFiles.kendoUploadFiles);
                                if (goodFiles.length > 0) {
                                    formData.append("OriginalFileName", goodFiles[0].name);
                                    formData.append("FileExtension", goodFiles[0].extension);
                                    formData.append("FileSize", goodFiles[0].size);
                                    formData.append("fichero", goodFiles[0].rawFile, goodFiles[0].name);
                                } else {
                                    if (options.data.originalFileName)
                                        formData.append("OriginalFileName", options.data.originalFileName);
                                    if (options.data.fileExtension)
                                        formData.append("FileExtension", options.data.fileExtension);
                                    if (options.data.fileSize)
                                        formData.append("FileSize", options.data.fileSize);
                                    if (options.data.uniqueFileName)
                                        formData.append("UniqueFileName", options.data.uniqueFileName);
                                }
                            } else {
                                if (options.data.originalFileName)
                                    formData.append("OriginalFileName", options.data.originalFileName);
                                if (options.data.fileExtension)
                                    formData.append("FileExtension", options.data.fileExtension);
                                if (options.data.fileSize)
                                    formData.append("FileSize", options.data.fileSize);
                                if (options.data.uniqueFileName)
                                    formData.append("UniqueFileName", options.data.uniqueFileName);
                            }
                            
                            var saveLicensesSuccess = function (response) {
                                response = {
                                    active: response.Active,
                                    contentType: response.ContentType,
                                    effectiveDate: moment(response.EffectiveDate).format('L'),
                                    expireDate: moment(response.ExpireDate).format('L'),
                                    fileExtension: response.FileExtension,
                                    FileSize: response.FileSize,
                                    licenseNumber: response.LicenseNumber,
                                    licenseName: response.LicenseTypeName,
                                    note: response.Note,
                                    originalFileName: response.OriginalFileName,
                                    placeOfServiceId: response.PlaceOfServiceId,
                                    posLicenseId: response.PosLicenseId,
                                    serverLocation: response.ServerLocation,
                                    uniqueFileName: response.UniqueFileName,
                                    uploaDateTime: response.UploaDateTime,
                                    uploadBy: response.UploadBy
                                };
                                PlaceOfServiceController.licenseFilesFromKendoUpload = [];
                                options.success(response);
                            }
                            var saveLicensesFails = function (response) {
                                console.log('Update License fails:', response);
                                toastr.error(response.statusText);
                                licenseViewModel.get('licensesDataSource').cancelChanges();
                                PlaceOfServiceController.licenseFilesFromKendoUpload = [];
                                options.error(response);
                            }
                            pos.saveLicenses(formData, saveLicensesSuccess, saveLicensesFails);
                        }
                    },
                    destroy: function (options) {
                        var releaseLicenseSuccess = function(response) {
                            options.success(response);
                        }
                        var releaseLicenseFail = function(response) {
                            console.log('Release License fails:', response);
                            toastr.error(response.statusText);
                            licenseViewModel.get('licensesDataSource').cancelChanges();
                            PlaceOfServiceController.licenseFilesFromKendoUpload = [];
                            options.error(response);
                        }
                        pos.inactivatePlaceOfServiceLicenses(options.data, releaseLicenseSuccess, releaseLicenseFail);
                    }
                }
            })
        });
        kendo.bind($(".js-license-" + placeOfServiceId), licenseViewModel);
    }


    return {
        getPlaceOfServiceItem: getPlaceOfServiceItem,
        createLicenseViewModel: createLicenseViewModel,
        licenseFilesFromKendoUpload: licenseFilesFromKendoUpload,
        getLicenseFileIndex: getLicenseFileIndex,
        placeOfServiceExpandedRow: placeOfServiceExpandedRow
    }
}(PlaceOfServiceManager);