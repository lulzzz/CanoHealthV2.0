var DoctorsController = function(doctorService) {
    //Private Properties
    var doctorExpandedRow;
    var medicalLicenseFilesFromKendoUpload = [];

    //Private Methods
    var createDoctorMedicalLicenseViewModel = function (doctor) {
        console.log("Doctor from Medical License ViewModel: ", doctor);
        
        var medicalLicenseViewModel = kendo.observable({
            //MedicalLicense Section
            //if doctor is Active enable New MedicalLicense button
            enableNewMedicalLicenseButton: doctor.Active,

            onAddMedicalLicense: function() {
                var medicalLicenseListViewComponents = $("#medicallicense-listview-" + doctor.DoctorId).data("kendoListView");
                medicalLicenseListViewComponents.add();
            },

            onEditMedicalLicenseItem: function (e) {
                if (!e.model.isNew()) {
                    e.model.dirty = true;
                    console.log(e.model);
                }
            },
            
            onDownloadMedicalLicenseFile: function(e) {
                if (e.data.uniqueFileName === null) {
                    toastr.error("There is not file to download.");
                    return;
                } else {
                    //Download the file
                    //window.location = domainName + '/GlobalFiles/Download?originalFileName=' + e.data.originalFileName +
                    //    '&uniqueFileName=' + e.data.uniqueFileName +
                    //    '&contentType=' + e.data.contentType +
                    //    '&serverLocation=' + e.data.serverLocation;

                    window.location = domainName + "/api/Files/Download?originalFileName=" + e.data.originalFileName +
                        "&uniqueFileName=" + e.data.uniqueFileName +
                        "&contentType=" + e.data.contentType +
                        "&container=DoctorMedicalLicenses";
                }
            },

            onRemoveMedicalLicenseItem: function (e) {
                e.preventDefault();

                var releaseMedicalLicenseTemplate = kendo.template($("#inactive-medicallicense-template").html());

                var window = $(".js-notification-dialog").kendoWindow({
                    title: "Confirmation",
                    modal: true,
                    visible: false, //the window will not appear before its .open method is called
                    width: "400px"
                }).data("kendoWindow");

                window.content(releaseMedicalLicenseTemplate(e.model)); //send the row data object to the template and render it
                window.center().open();

                $("#js-releaseMedicalLicense-yesButton").click(function () {
                    medicalLicenseViewModel.get('medicalLicenseDataSource').remove(e.model); //prepare a "destroy" request
                    medicalLicenseViewModel.get('medicalLicenseDataSource').sync(); //actually send the request (might be ommited if the autoSync option is enabled in the dataSource)
                    window.close();
                });
                $("#js-releaseMedicalLicense-noButton").click(function () {
                    window.close();
                });
            },

            onChangeEffectiveDate: function(e) {
                console.log("cambiando la fecha de effectividad de la licencia: ", e);
            },

            medicalLicenseDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: 'medicalLicenseId',
                        fields: {
                            medicalLicenseId: 'medicalLicenseId',
                            doctorId: {
                                defaultValue: doctor.DoctorId
                            },
                            medicalLicenseTypeId: 'medicalLicenseTypeId',
                            medicalLicenseType: 'medicalLicenseType',
                            licenseNumber: 'licenseNumber',
                            effectiveDate: 'effectiveDate',
                            expireDate: 'expireDate',
                            note: 'note',
                            serverLocation: 'serverLocation',
                            originalFileName: 'originalFileName',
                            uniqueFileName: 'uniqueFileName',
                            fileExtension: 'fileExtension',
                            fileSize: 'fileSize',
                            contentType: 'contentType',
                            uploadBy: 'uploadBy',
                            uploaDateTime: 'uploaDateTime',
                            active: 'active'
                        }
                    }
                },
                transport: {
                    read: function (options) {
                        var successCall = function (response) {
                           //map the response to convert the dates format
                            response = response.map(function(doctor) {
                                return {
                                    medicalLicenseId: doctor.medicalLicenseId,
                                    doctorId: doctor.doctorId,
                                    medicalLicenseTypeId: doctor.medicalLicenseTypeId,
                                    medicalLicenseType: doctor.medicalLicenseType,
                                    licenseNumber: doctor.licenseNumber,
                                    effectiveDate: moment(doctor.effectiveDate).format('L'),
                                    expireDate: moment(doctor.expireDate).format('L'),
                                    note: doctor.note,
                                    serverLocation: doctor.serverLocation,
                                    originalFileName: doctor.originalFileName,
                                    uniqueFileName: doctor.uniqueFileName,
                                    fileExtension: doctor.fileExtension,
                                    fileSize: doctor.fileSize,
                                    contentType: doctor.contentType,
                                    uploadBy: doctor.uploadBy,
                                    uploaDateTime: moment(doctor.uploaDateTime).format('L h:mm:ss a'),
                                    active: doctor.active
                                };
                            });
                            options.success(response);
                        }
                        var failCall = function(response) {
                            options.error(response);
                        }
                        doctorService.getMedicalLicenses(doctor.DoctorId, successCall, failCall);
                    },
                    create: function(options) {
                        if (!formDataSupport()) {
                            medicalLicenseViewModel.get('medicalLicenseDataSource').cancelChanges();
                            return;
                        } else {
                            var formData = new FormData();

                            formData.append("DoctorId", options.data.doctorId);
                            formData.append("MedicalLicenseTypeName", options.data.medicalLicenseType);
                            formData.append("LicenseNumber", options.data.licenseNumber);
                            formData.append("EffectiveDate", moment(options.data.effectiveDate).format('L'));
                            formData.append("ExpireDate", moment(options.data.expireDate).format('L'));
                            formData.append("Note", options.data.note);
                            formData.append("Active", true);

                            var doctorMedicalLicenseFiles = DoctorsController.medicalLicenseFilesFromKendoUpload.find(function (fileItem) {
                                return fileItem.kendoUploadId.indexOf(doctor.DoctorId) !== -1;
                            });

                            if (doctorMedicalLicenseFiles && doctorMedicalLicenseFiles.kendoUploadFiles) {
                                var goodFiles = processArrayOfFiles(doctorMedicalLicenseFiles.kendoUploadFiles);
                                if (goodFiles.length > 0) {
                                    formData.append("OriginalFileName", goodFiles[0].name);
                                    formData.append("FileExtension", goodFiles[0].extension);
                                    formData.append("FileSize", goodFiles[0].size);
                                    formData.append("ContentType", goodFiles[0].rawFile.type);
                                    formData.append("fichero", goodFiles[0].rawFile, goodFiles[0].name);
                                }
                            }
                            var createMedicalLicensesSuccess = function (response) {
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
                                DoctorsController.medicalLicenseFilesFromKendoUpload = [];
                                toastr.success("Medical License successfully created.");
                                options.success(response);
                            };
                            var createMedicalLicensesFail = function (response) {
                                console.log('Create License fails:', response);
                                toastr.error(response.statusText);
                                medicalLicenseViewModel.get('medicalLicenseDataSource').cancelChanges();
                                DoctorsController.medicalLicenseFilesFromKendoUpload = [];
                                options.error(response);
                            };
                            doctorService.createMedicalLicense(formData, createMedicalLicensesSuccess, createMedicalLicensesFail);
                        }
                    },
                    update: function(options) {
                        if (!formDataSupport()) {
                            medicalLicenseViewModel.get('medicalLicenseDataSource').cancelChanges();
                            return;
                        } else {
                            var formData = new FormData();

                            formData.append("MedicalLicenseId", options.data.medicalLicenseId);
                            formData.append("DoctorId", options.data.doctorId);
                            formData.append("MedicalLicenseTypeName", options.data.medicalLicenseType);
                            formData.append("LicenseNumber", options.data.licenseNumber);
                            formData.append("EffectiveDate", moment(options.data.effectiveDate).format('L'));
                            formData.append("ExpireDate", moment(options.data.expireDate).format('L'));
                            formData.append("Note", options.data.note);
                            formData.append("Active", options.data.active);
                            formData.append("ServerLocation", options.data.serverLocation);

                            var doctorMedicalLicenseFiles = DoctorsController.medicalLicenseFilesFromKendoUpload.find(function (fileItem) {
                                return fileItem.kendoUploadId.indexOf(doctor.DoctorId) !== -1;
                            });

                            if (doctorMedicalLicenseFiles && doctorMedicalLicenseFiles.kendoUploadFiles) {
                                var goodFiles = processArrayOfFiles(doctorMedicalLicenseFiles.kendoUploadFiles);
                                if (goodFiles.length > 0) {
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
                                if(options.data.uploadBy)
                                    formData.append("UploadBy", options.data.uploadBy);
                                if(options.data.uploaDateTime)
                                    formData.append("UploaDateTime", options.data.uploaDateTime);
                            }

                            var updateMedicalLicensesSuccess = function (response) {
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
                                DoctorsController.medicalLicenseFilesFromKendoUpload = [];
                                toastr.success("Medical License successfully updated.");
                                options.success(response);
                            };
                            var updateMedicalLicensesFail = function (response) {
                                console.log('Update License fails:', response);
                                toastr.error(response.statusText);
                                medicalLicenseViewModel.get('medicalLicenseDataSource').cancelChanges();
                                DoctorsController.medicalLicenseFilesFromKendoUpload = [];
                                options.error(response);
                            };
                            doctorService.updateMedicalLicense(formData, updateMedicalLicensesSuccess, updateMedicalLicensesFail);
                        }
                    },
                    destroy: function (options) {
                        var inactivateLicenseSuccess = function(response) {
                            toastr.success("Medical License successfully released.");
                            options.success(response);
                        };
                        var inactivateLicenseFail = function(response) {
                            console.log('Release Medical License fails:', response);
                            toastr.error(response.statusText);
                            medicalLicenseViewModel.get('medicalLicenseDataSource').cancelChanges();
                            options.error(response);
                        }
                        doctorService.inactivateLicense(options.data, inactivateLicenseSuccess, inactivateLicenseFail);
                    }
                }
            }),

            //MedicalLicenseType Section
            medicalLicenseTypeDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: "medicalLicenseTypeId",
                        fields: {
                            medicalLicenseTypeId: 'medicalLicenseTypeId',
                            classification: 'classification'
                        }
                    }
                },
                transport: {
                    read: {
                        method: 'GET',
                        url: "/api/MedicalLicenseTypes",
                        dataType: 'json'
                    }
                }
            })
        });
        kendo.bind($("#medicallicense_" + doctor.DoctorId), medicalLicenseViewModel);
    }

    var createDoctorFormViewModel = function(placeOfServiceId) {
        var doctorFormViewModel = kendo.observable({
            //Doctor Fields
            DoctorId: null,
            Degree: null,
            FirstName: null,
            LastName: null,
            DateOfBirth: null,
            SocialSecurityNumber: null,
            NpiNumber: null,
            CaqhNumber: null,
            Active: null,
            selectedDoctor: null,
            //Doctor events
            onSelectDoctor: function (e) {
                e.preventDefault();
                var doctor = e.dataItem;
                
                this.set('DoctorId', doctor.doctorId);
                this.set('Degree', doctor.degree);
                this.set('FirstName', doctor.firstName);
                this.set('LastName', doctor.lastName);
                this.set('DateOfBirth', doctor.dateOfBirth);
                this.set('SocialSecurityNumber', doctor.socialSecurityNumber);
                this.set('NpiNumber', doctor.npiNumber);
                this.set('CaqhNumber', doctor.caqhNumber);
                this.set('Active', doctor.active);
            },
            onChangeDoctorFirstName: function (e) {
                if (this.get('DoctorId')) {
                    this.set('DoctorId', null);
                    this.set('LastName', null);
                    this.set('DateOfBirth', null);
                    this.set('SocialSecurityNumber', null);
                    this.set('NpiNumber', null);
                    this.set('CaqhNumber', null);
                    this.set('Degree', null);
                    this.set('Active', null);
                }
                this.set('FirstName', e.sender.value());//$("#PatientFirstName").data("kendoAutoComplete")

                if (this.get('FirstName').trim().length > 0 &&
                    this.get('NpiNumber') &&
                    this.get('NpiNumber').trim().length > 0 &&
                    this.get('NpiNumber').indexOf('_') === -1) {
                    var baseUrl = this.get('npiRegistryThirdPartyBaseUrl');
                    if (this.get('LastName') && this.get('LastName').trim().length > 0) {
                        baseUrl = baseUrl + 'first_name=' + this.get('FirstName').trim() + '&last_name=' + this.get('LastName').trim() + '&number=' + this.get('NpiNumber') + '&pretty=on';
                    } else {
                        baseUrl = baseUrl + '&first_name=' + this.get('FirstName').trim() + '&number=' + this.get('NpiNumber').trim() + '&pretty=on';
                    }
                    DoctorService.getProviderInfo(baseUrl, this.getProviderInfoSuccess, this.getProviderInfoFail);
                }
            },
            onChangeLastName: function (e) {
                /*Si el usuario cambia el appellido del doctor y ya hay un npi establecido, si el primer nombre del doctor existe hay que chequear si
                 * el npi corresponde al doctor con ese nombre y appellido; en caso de que solamente el usuario haya puesto el appellido hay que chequear
                 * entonces que el npi pertenezca a un doctor con ese appellido.
                 */
                if (this.get('LastName').trim().length > 0 &&
                    this.get('NpiNumber') &&
                    this.get('NpiNumber').trim().length > 0 &&
                    this.get('NpiNumber').indexOf('_') === -1) {
                    var baseUrl = this.get('npiRegistryThirdPartyBaseUrl');
                    if (this.get('FirstName') && this.get('FirstName').trim().length > 0) {
                        baseUrl = baseUrl + 'first_name=' + this.get('FirstName').trim() + '&last_name=' + this.get('LastName').trim() + '&number=' + this.get('NpiNumber') + '&pretty=on';
                    } else {
                        baseUrl = baseUrl + '&last_name=' + this.get('LastName').trim() + '&number=' + this.get('NpiNumber').trim() + '&pretty=on';
                    }
                    DoctorService.getProviderInfo(baseUrl, this.getProviderInfoSuccess, this.getProviderInfoFail);
                }
            },
            onChangeNpiNumber: function (e) {
               
                if (this.get('NpiNumber').trim().length > 0 && this.get('NpiNumber').indexOf('_') === -1) {
                    var baseUrl = this.get('npiRegistryThirdPartyBaseUrl');
                   
                    if (this.get('FirstName') && this.get('FirstName').trim().length > 0 && this.get('LastName') && this.get('LastName').trim().length > 0)
                        baseUrl = baseUrl + 'first_name=' + this.get('FirstName').trim() + '&last_name=' + this.get('LastName').trim() + '&number=' + this.get('NpiNumber') + '&pretty=on';
                    else
                        baseUrl = baseUrl + 'number=' + this.get('NpiNumber').trim() + '&pretty=on';

                    
                    DoctorService.getProviderInfo(baseUrl, this.getProviderInfoSuccess, this.getProviderInfoFail);
                }
            },
            availableDoctorDataSource: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "/api/doctors",
                        data: { placeOfServiceId: placeOfServiceId },
                        dataType: "json"
                    }
                }
            }),
            isEnableAssignBtn: true,
            npiRegistryThirdPartyBaseUrl: 'https://npiregistry.cms.hhs.gov/api?',
            isValidModelState: function() {
                var isValid = true,
                    firstnamemessage = "",
                    lastnamemessage = "",
                    degreemessage = "",
                    dobmessage = "",
                    ssnmessage = "",
                    npimessage = "",
                    caqhmessage = "";
                if (!this.get('FirstName') || (this.get('FirstName') && this.get('FirstName').trim().length === 0)) {
                    isValid = false;
                    firstnamemessage = "<li> The FIRST NAME field is required. </li>";
                }

                if (!this.get('LastName') || (this.get('LastName') && this.get('LastName').trim().length === 0)) {
                    isValid = false;
                    lastnamemessage = "<li> The LAST NAME field is required. </li>";
                }

                if (!this.get('Degree') || (this.get('Degree') && this.get('Degree').trim().length === 0)) {
                    isValid = false;
                    degreemessage = "<li> The DEGREE field is required. </li>";
                }

                if (!this.get('DateOfBirth')) {
                    isValid = false;
                    dobmessage = "<li> The DOB field is required. </li>";
                }

                if (!this.get('SocialSecurityNumber') || (this.get('SocialSecurityNumber') && this.get('SocialSecurityNumber').indexOf('_') !== -1)) {
                    isValid = false;
                    ssnmessage = "<li> Invalid Social Security Number. </li>";
                }

                if (!this.get('NpiNumber') || (this.get('NpiNumber') && this.get('NpiNumber').indexOf('_') !== -1)) {
                    isValid = false;
                    npimessage = "<li> Invalid NPI number. </li>";
                }

                if (!this.get('CaqhNumber') || (this.get('CaqhNumber') && this.get('CaqhNumber').trim().length === 0)) {
                    isValid = false;
                    caqhmessage = "<li> The CAQH field is required. </li>";
                }

                if (!isValid)
                    toastr.error(`<ul>${firstnamemessage}${lastnamemessage}${degreemessage}${dobmessage}${ssnmessage}${npimessage}${caqhmessage}</ul>`);
                return isValid;
            },
            assignDoctorToPlaceOfService: function () {
                if (this.isValidModelState()) {
                    var doctorClinicRelation = {
                        PlaceOfServiceId: placeOfServiceId,
                        ActiveDoctorClinicRelationship: true,
                        DoctorId: this.get('DoctorId'),
                        FirstName: this.get('FirstName'),
                        LastName: this.get('LastName'),
                        DateOfBirth: moment(this.get('DateOfBirth')).format('L'),
                        Degree: this.get('Degree'),
                        SocialSecurityNumber: this.get('SocialSecurityNumber'),
                        NpiNumber: this.get('NpiNumber'),
                        CaqhNumber: this.get('CaqhNumber'),
                        Active: this.get('Active') == null ? true : this.get('Active')
                    };
                    var assignDoctorToPlaceOfServiceSuccess = function (response) {
                        var placeOfServiceDoctorGrid = $("#Doctors_" + placeOfServiceId).data('kendoGrid');
                        var doctorInfo = {
                            DoctorId: response.doctorId,
                            FirstName: response.firstName,
                            LastName: response.lastName,
                            DateOfBirth: moment(response.dateOfBirth).format('L'),
                            Degree: response.degree,
                            SocialSecurityNumber: response.socialSecurityNumber,
                            NpiNumber: response.npiNumber,
                            CaqhNumber: response.caqhNumber,
                            Active: response.active
                        }
                        placeOfServiceDoctorGrid.dataSource.pushCreate(doctorInfo);
                        var doctorFormWnd = $(".js-doctor-form-window").data('kendoWindow');
                        doctorFormWnd.refresh().close();
                        console.log("success: ", response);
                    };
                    var assignDoctorToPlaceOfServiceFails = function (response) {
                        switch (response.status) {
                            case 400: //BadRequest
                                console.log("responseJSON: ", response.responseJSON);
                                if (response.responseJSON && response.responseJSON.errorResponse) {
                                    var doctorWindow = $(".js-doctor-notification-window").kendoWindow({
                                        title: "Alert!",
                                        width: 600,
                                        modal: true
                                    }).data("kendoWindow");
                                    var doctorFoundTemplate = kendo.template($("#previous-doctor-found-template").html());
                                    doctorWindow.content(doctorFoundTemplate(response.responseJSON)); 
                                    doctorWindow.center().open();
                                }
                                break;
                            default:
                                console.log("Assign Doctor to Location request fails: ", response);
                                break;
                        }
                        
                    };

                    AjaxCallPost("/api/clinicdoctorteams/", JSON.stringify(doctorClinicRelation), assignDoctorToPlaceOfServiceSuccess, assignDoctorToPlaceOfServiceFails);
                }
            },
            cancelAssignationProcess: function() {
                var doctorFormWindow = $(".js-doctor-form-window").data("kendoWindow");
                doctorFormWindow.close();
            },
            //When a previous doctor is found scenario
            onSelectDoctorFound: function(e) {
                console.log("onSelectDoctorFound: ", e);
            },
            onUpdateDoctorFoundInfo: function(e) {
                console.log("onUpdateDoctorFoundInfo: ", e);
            },
            onCancelOperationWhenDoctorFound: function(e) {
                console.log('onCancelOperationWhenDoctorFound: ', e);
            },
            //Process AJAX requests responses
            getProviderInfoSuccess: function(response) {
                if (response.result_count === 0) {
                    toastr.error("Invalid NPI number. Please try again!");
                    doctorFormViewModel.set('isEnableAssignBtn', false);
                    doctorFormViewModel.set('NpiNumber', "");
                } else
                    doctorFormViewModel.set('isEnableAssignBtn', true);
            },
            getProviderInfoFail: function(response) {
                console.log("Fail Respuesta del 3rd party: ", response);
                doctorFormViewModel.set('isEnableAssignBtn', true);
            }            
        });
        kendo.bind($(".js-doctor-form-window"), doctorFormViewModel);
    };    
    
    //Access to private members
    return {
        doctorExpandedRow: doctorExpandedRow,
        medicalLicenseFilesFromKendoUpload: medicalLicenseFilesFromKendoUpload,
        createDoctorMedicalLicenseViewModel: createDoctorMedicalLicenseViewModel,
        createDoctorFormViewModel: createDoctorFormViewModel       
    };
}(DoctorService);