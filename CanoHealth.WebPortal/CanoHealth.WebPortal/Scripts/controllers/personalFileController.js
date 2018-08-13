var PersonalFilesController = function (personalFileService) {
    //Private Fields
    var doctorPersonalFilesFromKendoUpload = [];

    var createDoctorPersonalFileViewModel = function(doctor) {
        var personalFileViewModel = kendo.observable({
            enableNewPersonalFileButton: doctor.Active,

            onAddPersonalFiles: function () {
                var perosnalFileListViewComponents = $("#personalfiles-listview-" + doctor.DoctorId).data("kendoListView");
                perosnalFileListViewComponents.add();
            },

            onEditDoctorPersonalFileItem: function (e) {
                if (!e.model.isNew()) {
                    e.model.dirty = true;
                    console.log(e.model);
                }
            },

            onDownloadDoctorPersonalFile: function (e) {
                if (e.data.uniqueFileName === null) {
                    toastr.error("There is not file to download.");
                    return;
                } else {
                    //Download the file
                    //window.location = domainName + '/GlobalFiles/Download?originalFileName=' + e.data.originalFileName +
                    //    '&uniqueFileName=' + e.data.uniqueFileName +
                    //    '&contentType=' + e.data.contentType +
                    //    '&serverLocation=' + e.data.serverLocation;

                    //Download from Azure Storage Account
                    window.location = domainName + "/api/Files/Download?originalFileName=" + e.data.originalFileName +
                        "&uniqueFileName=" + e.data.uniqueFileName +
                        "&contentType=" + e.data.contentType +
                        "&container=DoctorPersonalFiles";
                }
            },

            personalFilesDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: "doctorFileId",
                        fields: {
                            doctorFileId: "doctorFileId",
                            doctorId: {
                                defaultValue: doctor.DoctorId
                            },
                            doctorFileTypeId: "doctorFileTypeId",
                            doctorFileTypeName: "doctorFileTypeName",
                            originalFileName: "originalFileName",
                            uniqueFileName: "uniqueFileName",
                            fileExtension: "fileExtension",
                            fileSize: "fileSize",
                            contentType: "contentType",
                            uploadDateTime: "uploadDateTime",
                            uploadBy: "uploadBy",
                            serverLocation: "serverLocation",
                            active: "active"
                        }                       
                    }
                },
                transport: {
                    read: function (options) {
                        var getPersonalFileSuccess = function (response) {
                            //Map the response to convert the date to mm/dd/yyyy
                            response = response.map(function(file) {
                                return {
                                    doctorFileId: file.doctorFileId,
                                    doctorId: file.doctorId,
                                    doctorFileTypeId: file.doctorFileTypeId,
                                    doctorFileTypeName: file.doctorFileTypeName,
                                    originalFileName: file.originalFileName,
                                    uniqueFileName: file.uniqueFileName,
                                    fileExtension: file.fileExtension,
                                    fileSize: file.fileSize,
                                    contentType: file.contentType,
                                    uploadDateTime: moment(file.uploadDateTime).format('L h:mm:ss a'),
                                    uploadBy: file.uploadBy,
                                    serverLocation: file.serverLocation,
                                    active: file.active                                     
                                };
                            });

                            options.success(response);
                        };
                        var getPersonalFileError = function (response) {

                            options.error(response);
                        };

                        personalFileService.getDoctorPersonalFiles(doctor.DoctorId, getPersonalFileSuccess, getPersonalFileError);
                    },
                    create: function (options) {
                        //if browser does not support form data notify to the user.
                        if (!formDataSupport()) {
                            personalFileViewModel.get('personalFilesDataSource').cancelChanges();
                            return;
                        } else {
                            //get from doctorPersonalFilesFromKendoUpload array the item that contains the current doctorid
                            var personalFiles = doctorPersonalFilesFromKendoUpload.find(function (fileItem) {
                                return fileItem.kendoUploadId.indexOf(doctor.DoctorId) !== -1;
                            });

                            if (!personalFiles || !personalFiles.kendoUploadFiles) {
                                toastr.error("Please select a valid file.");
                                personalFileViewModel.get('personalFilesDataSource').cancelChanges();
                                return;
                            }

                            var formData = new FormData();

                            //Active by default is going to be true when create
                            formData.append("Active", true);
                            formData.append("DoctorId", options.data.doctorId);
                            formData.append("DoctorFileTypeName", options.data.doctorFileTypeName);
                            
                            //if the item exist and has files
                            var goodFiles = processArrayOfFiles(personalFiles.kendoUploadFiles);
                            if (goodFiles.length > 0) {
                                formData.append("OriginalFileName", goodFiles[0].name);
                                formData.append("FileExtension", goodFiles[0].extension);
                                formData.append("FileSize", goodFiles[0].size);
                                formData.append("ContentType", goodFiles[0].rawFile.type);
                                formData.append("fichero", goodFiles[0].rawFile, goodFiles[0].name);
                            }
                            
                            var createPersonalFileSuccess = function (response) {
                                console.log('Create Personal File succeed:', response);
                                toastr.success("File successfully created.");
                                response = {
                                    doctorFileId: response.DoctorFileId,
                                    doctorId: response.DoctorId,
                                    doctorFileTypeId: response.DoctorFileTypeId,
                                    doctorFileTypeName: response.DoctorFileTypeName,
                                    originalFileName: response.OriginalFileName,
                                    uniqueFileName: response.UniqueFileName,
                                    fileExtension: response.FileExtension,
                                    fileSize: response.FileSize,
                                    contentType: response.ContentType,
                                    uploadDateTime: moment(response.UploadDateTime).format('L h:mm:ss a'),
                                    uploadBy: response.UploadBy,
                                    serverLocation: response.ServerLocation,
                                    active: response.Active
                                };
                                PersonalFilesController.doctorPersonalFilesFromKendoUpload = [];                                
                                options.success(response);
                            };
                            var createPersonalFileFails = function (response) {
                                console.log('Create Personal File fails:', response);
                                toastr.error(response.statusText);
                                personalFileViewModel.get('personalFilesDataSource').cancelChanges();
                                PersonalFilesController.doctorPersonalFilesFromKendoUpload = [];
                                options.error(response);
                            };
                            personalFileService.createPersonalFile(formData, createPersonalFileSuccess, createPersonalFileFails)                      
                        }
                    },
                    update: function (options) {
                        //if browser does not support form data notify to the user.
                        if (!formDataSupport()) {
                            personalFileViewModel.get('personalFilesDataSource').cancelChanges();
                            return;
                        } else {
                            var formData = new FormData();

                            formData.append("DoctorFileId", options.data.doctorFileId);
                            formData.append("DoctorId", options.data.doctorId);
                            formData.append("DoctorFileTypeId", options.data.doctorFileTypeId);
                            formData.append("DoctorFileTypeName", options.data.doctorFileTypeName);
                            formData.append("Active", options.data.active);
                            formData.append("ServerLocation", options.data.serverLocation);
                                   
                            //get from doctorPersonalFilesFromKendoUpload array the item that contains the current doctorid
                            var personalFiles = doctorPersonalFilesFromKendoUpload.find(function (fileItem) {
                                return fileItem.kendoUploadId.indexOf(doctor.DoctorId) !== -1;
                            });

                            //if the item exist and has files
                            if (personalFiles && personalFiles.kendoUploadFiles) {
                                var goodFiles = processArrayOfFiles(personalFiles.kendoUploadFiles);
                                if (goodFiles.length > 0){
                                    formData.append("OriginalFileName", goodFiles[0].name);
                                    formData.append("FileExtension", goodFiles[0].extension);
                                    formData.append("FileSize", goodFiles[0].size);
                                    formData.append("ContentType", goodFiles[0].rawFile.type);
                                    formData.append("fichero", goodFiles[0].rawFile, goodFiles[0].name);
                                }
                            } else {
                                if (options.data.originalFileName)
                                    formData.append("OriginalFileName", options.data.originalFileName);
                                if (options.data.fileExtension)
                                    formData.append("FileExtension", options.data.fileExtension);
                                if (options.data.fileSize)
                                    formData.append("FileSize", options.data.fileSize);
                                if (options.data.contentType)
                                    formData.append("ContentType", options.data.contentType);
                                if (options.data.uniqueFileName)
                                    formData.append("UniqueFileName", options.data.uniqueFileName);
                                if (options.data.uploadDateTime)
                                    formData.append("UploadDateTime", options.data.uploadDateTime);
                                if (options.data.uploadBy)
                                    formData.append("UploadBy", options.data.uploadBy);
                            }

                            var updatePersonalFileSuccess = function (response) {
                                console.log('Update Personal File succeed:', response);
                                toastr.success("File successfully updated.");
                                //Reset the array of files
                                PersonalFilesController.doctorPersonalFilesFromKendoUpload = [];
                                response = {
                                    doctorFileId: response.DoctorFileId,
                                    doctorId: response.DoctorId,
                                    doctorFileTypeId: response.DoctorFileTypeId,
                                    doctorFileTypeName: response.DoctorFileTypeName,
                                    originalFileName: response.OriginalFileName,
                                    uniqueFileName: response.UniqueFileName,
                                    fileExtension: response.FileExtension,
                                    fileSize: response.FileSize,
                                    contentType: response.ContentType,
                                    uploadDateTime: moment(response.UploadDateTime).format('L h:mm:ss a'),
                                    uploadBy: response.UploadBy,
                                    serverLocation: response.ServerLocation,
                                    active: response.Active
                                };
                                options.success(response);
                            };
                            var updatePersonalFileFails = function (response) {
                                console.log('Update Personal File fails:', response);
                                toastr.error(response.statusText);
                                personalFileViewModel.get('personalFilesDataSource').cancelChanges();
                                 //Reset the array of files
                                PersonalFilesController.doctorPersonalFilesFromKendoUpload = [];
                                options.error(response);
                            };
                            personalFileService.updatePersonalFile(formData, updatePersonalFileSuccess, updatePersonalFileFails)  
                        }
                    }
                }
            }),

            doctorFileTypeDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: "doctorFileTypeId",
                        fields: {
                            doctorFileTypeId: "doctorFileTypeId",
                            doctorFileTypeName: "doctorFileTypeName"
                        }                        
                    }
                },
                transport: {
                    read: {
                        method: 'GET',
                        url: "/api/PersonalFileTypes",
                        dataType: "json"
                    }
                }
            })
        });
        kendo.bind($("#personalprofile_" + doctor.DoctorId), personalFileViewModel);
    };

    //Personal File Kendo Upload
    var onSelectDoctorPersonalFile = function(e) {
        /*By convention The Personal File Kendo Upload component does not allow multiple files*/
        if (doctorPersonalFilesFromKendoUpload.length === 0) {
            doctorPersonalFilesFromKendoUpload.push({
                //insert the id of the kendo upload component
                kendoUploadId: e.sender.name,
                //insert the current file selected by the kendo upload
                kendoUploadFiles: e.files
            });
        } else {
            //if the array has items find the index of the current kendo upload component
            var itemIndex = doctorPersonalFilesFromKendoUpload.findIndex(function (file) {
                return file.kendoUploadId === e.sender.name; //Closures
            });
            //if the kendo upload is not found in the array 
            if (itemIndex === -1) {
                doctorPersonalFilesFromKendoUpload.push({
                    //insert the id of the kendo upload component
                    kendoUploadId: e.sender.name,
                    //insert the current file selected by the kendo upload
                    kendoUploadFiles: e.files
                });
            } else {
                //replace the files of the kendo upload component
                doctorPersonalFilesFromKendoUpload[itemIndex].kendoUploadFiles = e.files;
            }
        }
    };

    var onRemoveDoctorPersonalFile = function (e) {
        //get the files to delete
        var filesToDelete = e.files;
        //get the index of the kendo upload component store in the array
        var searchIndexInTheArray = doctorPersonalFilesFromKendoUpload.findIndex(function (itemArr) {
            return itemArr.kendoUploadId === e.sender.name;
        });
        
        //if the kendo upload was found
        if (searchIndexInTheArray !== -1) {
            var personalFilesFromArray = doctorPersonalFilesFromKendoUpload[searchIndexInTheArray].kendoUploadFiles;
            var aux = personalFilesFromArray.filter(function (el) {
                return !filesToDelete.includes(el);
            });
            doctorPersonalFilesFromKendoUpload[searchIndexInTheArray].kendoUploadFiles = aux;
        }
    };

    return {
        createDoctorPersonalFileViewModel: createDoctorPersonalFileViewModel,
        onSelectDoctorPersonalFile: onSelectDoctorPersonalFile,
        onRemoveDoctorPersonalFile: onRemoveDoctorPersonalFile
    };
}(PersonalFileService);