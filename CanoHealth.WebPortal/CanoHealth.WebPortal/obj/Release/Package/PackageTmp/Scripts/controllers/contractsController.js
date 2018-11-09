var ContractsController = function (contractService) {
    //Private Fields
    var contractWindowComponent = $(".js-contractform-window").data('kendoWindow');
    var contractUploadComponent = $("#Addendum").data('kendoUpload');
    var contractExpandedRow;
    var addendumFilesFromKendoUpload = [];

    //Private Methods
   
    var getInsuranceWithContractsSuccess = function (response) {
        
        if (response.name != null && response.active === false) {
            var inactiveInsuranceNotification = $(".js-inactiveinsurance-notification").kendoWindow({
                modal: true,
                title: "Alert!",
                width: 300
            }).data('kendoWindow');
            inactiveInsuranceNotification.open().center();
            $(".js-activate-insurance").attr('data-insurance-id', response.insuranceId);
            $(".js-inactiveinsurance-notification").on("click", ".js-activate-insurance", function () {
                response.active = true;
                contractService.activateInsurance(response);
                inactiveInsuranceNotification.close();
            });
            $(".js-inactiveinsurance-notification").on("click", ".js-cancel-insurance", function () {
                inactiveInsuranceNotification.close();
                $(".js-insurance-name").val("");
            });
        }
    };

    var getInsuranceWithContractsFail = function (response) {
        console.log("getInsuranceWithContractsFail: ", response);
    };

    var createContractViewModel = function () {
        var contractViewModel = kendo.observable({
            CorporationId: null,
            corporationsDataSource: new kendo.data.DataSource({
                type: 'aspnetmvc-ajax',
                serverFiltering: true,
                schema: {
                    data: 'Data',
                    total: 'Total',
                    errors: 'Errors',
                    model: { id: 'CorporationId' }
                },
                transport: {
                    read: {
                        url: domainName + "/Corporation/ReadCorporations"
                    }
                }
            }),
            insuranceDataSource: new kendo.data.DataSource({
                type: 'aspnetmvc-ajax',
                serverFiltering: true,
                schema: {
                    data: 'Data',
                    total: 'Total',
                    errors: 'Errors',
                    model: { id: 'InsuranceId' }
                },
                transport: {
                    read: {
                        url: domainName + "/Insurances/ReadInsurances"
                    }
                }
            }),
            onOpenCorporationDropdownComponent: function() {
                contractViewModel.get('corporationsDataSource').read();
            },
            InsuranceName: null,
            GroupNumber: null,
            onChangeInsuranceName: function(e) {
                var currentInsurance = this.get('InsuranceName');
                if (!isEmptyValue(currentInsurance)) {
                    contractService.getInsuranceWithContracts(currentInsurance, getInsuranceWithContractsSuccess, getInsuranceWithContractsFail);
                }
            },
            isValidContract: function() {
                var isValid = true,
                    corporationmessage = "",
                    insurancemessage = "",
                    groupnumbermessage = "";
                if (isEmptyValue(this.get('CorporationId'))) {
                    isValid = false;
                    corporationmessage = "<li>The Corporation field is required.</li>";
                }
                if (isEmptyValue(this.get('InsuranceName'))) {
                    isValid = false;
                    insurancemessage = "<li>The Insurance field is required.</li>";
                }
                if (isEmptyValue(this.get('GroupNumber'))) {
                    isValid = false;
                    groupnumbermessage = "<li>The Group Number field is required.</li>";
                }
                if (!isValid)
                    toastr.error(`<ul>${corporationmessage}${insurancemessage}${groupnumbermessage}</ul>`);
                return isValid;
            },
            onSaveContract: function() {
                if (!formDataSupport() || !this.isValidContract()) {
                    return;
                } else {
                    var formData = new FormData();
                    if (this.get('CorporationId'))
                        formData.append("CorporationId", this.get('CorporationId'));
                    if (this.get('InsuranceName'))
                        formData.append("InsuranceName", this.get('InsuranceName'));
                    if (this.get('GroupNumber'))
                        formData.append("GroupNumber", this.get('GroupNumber'));
                    formData.append("Active", true);

                    if ($("#Addendum").data('kendoUpload')) {
                        var goodFiles = processFormsFiles($("#Addendum").data('kendoUpload'));
                        if (goodFiles.length > 0) {
                            formData.append("OriginalFileName", goodFiles[0].name);
                            formData.append("FileExtension", goodFiles[0].extension);
                            formData.append("FileSize", goodFiles[0].size);
                            formData.append("ContentType", goodFiles[0].rawFile.type);
                            formData.append("FileActive", true);
                            formData.append("Fichero", goodFiles[0].rawFile);
                        }
                    }

                    var createContractsuccess = function(response) {
                        console.log("CreateContract success");
                        var grid = $("#Contracts").data('kendoGrid'),
                            contractWindow = $(".js-contractform-window").data("kendoWindow");
                        grid.dataSource.pushCreate({
                            Active: response.Active,
                            ContentType: response.ContentType,
                            ContractId: response.ContractId,
                            CorporationId: response.CorporationId,
                            FileActive: response.FileActive,
                            FileExtension: response.FileExtension,
                            FileSize: response.FileSize,
                            GroupNumber: response.GroupNumber,
                            InsuranceId: response.InsuranceId,
                            InsuranceName: response.InsuranceName,
                            OriginalFileName: response.OriginalFileName
                        });
                        contractWindow.close();
                    };
                    var createContracterror= function(response) {
                        console.log("CreateContract fail: ", response);
                        if (response.status === 400) { //Bad Request
                            toastr.error(response.statusText);
                        } else {
                            toastr.error('Something failed. Please contact your system administrator.');
                        }
                    };
                    contractService.createContract(formData, createContractsuccess, createContracterror);
                }
            },
            onCancelContractOperation: function() {
                if ($(".js-contractform-window").data('kendoWindow'))
                    $(".js-contractform-window").data('kendoWindow').close();
                if ($("#Addendum").data('kendoUpload'))
                    $("#Addendum").data('kendoUpload').removeAllFiles();
            }
        });
        kendo.bind($(".js-contractform-window"), contractViewModel);
    };

    var createAddendumViewModel = function(contractId, isContractActive) {
        var addendumViewModel = kendo.observable({
            enableButtons: isContractActive,

            onAddAddendum: function() {
                var listView = $("#addendum-listview-" + contractId).data("kendoListView");
                listView.add();
            },

            onEditAddendumItem: function (e) {
                if (!e.model.isNew()) {
                    e.model.dirty = true;                    
                }
            },

            onDownloadContractAddendum: function(e) {
                if (e.data.uniqueFileName === null) {
                    toastr.error("There is not file to download");
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
                        "&container=ContractAddendums";
                }
            },

            onRemoveAddendumItem: function(e) {
                e.preventDefault();

                var releaseAddendumTemplate = kendo.template($("#inactive-addendum-template").html());

                var window = $(".js-notification-dialog").kendoWindow({
                    title: "Confirmation",
                    modal: true,
                    visible: false, //the window will not appear before its .open method is called
                    width: "400px"
                }).data("kendoWindow");

                window.content(releaseAddendumTemplate(e.model)); //send the row data object to the template and render it
                window.center().open();

                $("#js-releaseAddendum-yesButton").click(function () {
                    addendumViewModel.get('addendumDataSource').remove(e.model); //prepare a "destroy" request
                    addendumViewModel.get('addendumDataSource').sync(); //actually send the request (might be ommited if the autoSync option is enabled in the dataSource)
                    window.close();
                });
                $("#js-releaseAddendum-noButton").click(function () {
                    window.close();
                });
            },

            addendumDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: 'contractAddendumId',
                        fields: {
                            contractAddendumId: {
                                nullable: true,
                                editable: false
                            },
                            contractId: {
                                defaultValue: contractId,
                                required: true
                            },
                            uniqueFileName: "uniqueFileName",
                            originalFileName: {
                                nullable: true,
                                editable: false
                            },
                            fileExtension: "fileExtension",
                            fileSize: "fileSize",
                            contentType: "contentType",
                            serverLocation: "serverLocation",
                            uploadDateTime: "uploadDateTime",
                            uploadBy: "uploadBy",
                            description: "description",
                            active: "active"
                        }
                    }
                },
                transport: {
                    read: function (options) {
                        $.ajax({
                            url: "/api/Addendums",
                            data: { contractId: contractId },
                            dataType: 'json',
                            success: function (response) {
                                response = response.map(function(addendum){
                                    return {
                                        contractAddendumId: addendum.contractAddendumId,
                                        contractId: addendum.contractId,
                                        uniqueFileName: addendum.uniqueFileName,
                                        originalFileName: addendum.originalFileName,
                                        fileExtension: addendum.fileExtension,
                                        fileSize: addendum.fileSize,
                                        contentType: addendum.contentType,
                                        serverLocation: addendum.serverLocation,
                                        uploadDateTime: moment(addendum.uploadDateTime).format('L h:mm:ss a'),
                                        uploadBy: addendum.uploadBy,
                                        description: addendum.description,
                                        active: addendum.active
                                    };
                                });
                                options.success(response);
                            },
                            error: function (response) {
                                toastr.error("Something failed trying to get data from our server.");
                                options.error(response);
                            }
                        });
                    },
                    create: function (options) {
                        if (!formDataSupport()) {
                            addendumViewModel.get('addendumDataSource').cancelChanges();
                            return;
                        } else {
                            var addemdumFile = ContractsController.addendumFilesFromKendoUpload.find(function (fileItem) {
                                return fileItem.kendoUploadId.indexOf(contractId) !== -1;
                            });
                            if (!addemdumFile || !addemdumFile.kendoUploadFiles) {
                                toastr.error("Please select a valid file.");
                                addendumViewModel.get('addendumDataSource').cancelChanges();
                                return;
                            }
                            var formData = new FormData();
                            formData.append("Description", options.data.description);
                            formData.append("ContractId", contractId);
                            formData.append("Active", true);

                            var goodFiles = processArrayOfFiles(addemdumFile.kendoUploadFiles);
                            if (goodFiles.length > 0) {
                                formData.append("OriginalFileName", goodFiles[0].name);
                                formData.append("FileExtension", goodFiles[0].extension);
                                formData.append("FileSize", goodFiles[0].size);
                                formData.append("ContentType", goodFiles[0].rawFile.type);
                                formData.append("fichero", goodFiles[0].rawFile, goodFiles[0].name);
                            }

                            $.ajax({
                                type: "POST",
                                url: domainName + '/Addendums/CreateAddendum/',
                                contentType: false,
                                processData: false,
                                data: formData,
                                success: function (response) {                                   
                                    response = {
                                        active: true,
                                        contentType: response.ContentType,
                                        contractAddendumId: response.ContractAddendumId,
                                        contractId: response.ContractId,
                                        description: response.Description,
                                        fileExtension: response.FileExtension,
                                        fileSize: response.FileSize,
                                        originalFileName: response.OriginalFileName,
                                        serverLocation: response.ServerLocation,
                                        uniqueFileName: response.UniqueFileName,
                                        uploadBy: response.UploadBy,
                                        uploadDateTime: moment(response.UploadDateTime).format("L h:mm:ss a")
                                    };
                                    toastr.success("A new file was uploaded.");
                                    // notify the data source that the request succeeded
                                    options.success(response);
                                },
                                error: function (response) {
                                    console.log('Upload Addendum fails:', response);
                                    toastr.error(response.statusText);
                                    addendumViewModel.get('addendumDataSource').cancelChanges();
                                    ContractsController.addendumFilesFromKendoUpload = [];
                                    // notify the data source that the request failed
                                    options.error(response);
                                }
                            });
                        }
                    },
                    update: function (options) {
                        
                        if (!formDataSupport()) {
                            addendumViewModel.get('addendumDataSource').cancelChanges();
                            return;
                        } else {
                            var formData = new FormData();
                            formData.append("ContractAddendumId", options.data.contractAddendumId);
                            formData.append("Description", options.data.description);
                            formData.append("ContractId", contractId);
                            formData.append("Active", options.data.active);
                            formData.append("ServerLocation", options.data.serverLocation);
                           
                            var addemdumFile = ContractsController.addendumFilesFromKendoUpload.find(function (fileItem) {
                                return fileItem.kendoUploadId.indexOf(contractId) !== -1;
                            });

                            if (addemdumFile && addemdumFile.kendoUploadFiles) {
                                var goodFiles = processArrayOfFiles(addemdumFile.kendoUploadFiles);
                                if (goodFiles.length === 0) {
                                    addendumViewModel.get('addendumDataSource').cancelChanges();
                                    return;
                                }
                                formData.append("OriginalFileName", goodFiles[0].name);
                                formData.append("FileExtension", goodFiles[0].extension);
                                formData.append("FileSize", goodFiles[0].size);
                                formData.append("ContentType", goodFiles[0].rawFile.type);
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
                                if (options.data.contentType)
                                    formData.append("ContentType", options.data.contentType);
                                if (options.data.uploadDateTime)
                                    formData.append("UploadDateTime", options.data.uploadDateTime);
                                if (options.data.uploadBy)
                                    formData.append("UploadBy", options.data.uploadBy);
                            }

                            $.ajax({
                                type: "POST",
                                url: domainName + '/Addendums/UpdateAddendum/',
                                contentType: false,
                                processData: false,
                                data: formData,
                                success: function (response) {                                    
                                    response = {
                                        active: response.Active,
                                        contentType: response.ContentType,
                                        contractAddendumId: response.ContractAddendumId,
                                        contractId: response.ContractId,
                                        description: response.Description,
                                        fileExtension: response.FileExtension,
                                        fileSize: response.FileSize,
                                        originalFileName: response.OriginalFileName,
                                        serverLocation: response.ServerLocation,
                                        uniqueFileName: response.UniqueFileName,
                                        uploadBy: response.UploadBy,
                                        uploadDateTime: moment(response.UploadDateTime).format("L h:mm:ss a")
                                    };
                                    toastr.success("The contract was successfuly updated.");
                                    // notify the data source that the request succeeded
                                    options.success(response);
                                },
                                error: function (response) {
                                    console.log('Upload Addendum fails:', response);
                                    toastr.error(response.statusText);
                                    addendumViewModel.get('addendumDataSource').cancelChanges();
                                    ContractsController.addendumFilesFromKendoUpload = [];
                                    // notify the data source that the request failed
                                    options.error(response);
                                }
                            });
                        }
                    },
                    destroy: function(options) {
                        $.ajax({
                            method: "DELETE",
                            url: '/api/Addendums',
                            data: options.data,
                            success: function (response) {
                                toastr.success("The file was released successfully.");
                                options.success(response);
                            },
                            error: function (response) {
                                console.log('Release Addendum fails:', response);
                                toastr.error(response.statusText);
                                addendumViewModel.get('addendumDataSource').cancelChanges();
                                ContractsController.addendumFilesFromKendoUpload = [];
                                options.error(response);
                            }
                        });
                    }
                }
            })
        });
        kendo.bind($(".js-addendum-" + contractId), addendumViewModel);
    };

    var createContractBusinessLineViewModel = function(contractId, corporationId, isContractActive, insuranceId) {
        var contractBusinessLineViewModel = kendo.observable({
            enableBusinessLineButtons: isContractActive,
            //Business Lines Section
            selectedBusinessLine: null,
            businessLinesDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: "planTypeId"
                    }
                },
                transport: {
                    read: {
                        method: 'GET',
                        //url: '/api/businesslines', //Original Code shows all Line of Business in Database
                        //data: {contractId: contractId}, //Original Code
                        url: '/api/InsuranceBusinessLines',
                        data: {
                            contractId: contractId,
                            insuranceId: insuranceId
                        },
                        dataType: 'json'
                    }
                }
            }),
            
            //Place of Service Section
            selectedPlaceOfService: null,
            placeOfServiceDataSource: new kendo.data.DataSource({
                transport: {
                    read: {
                        method: 'GET',
                        url: '/api/placeofservices',
                        data: { corporationId: corporationId },
                        dataType: 'json'
                    }
                }
            }),
            
            //Contract-BusinessLine Section
            addBtnVisible: null,
            editBtnVisible: null,
            originalContractBusinessLineItem: null,
            isContractBusinessLineChanging: function() {
                if (this.get('editBtnVisible') === true && this.get('originalContractBusinessLineItem') &&
                (this.get('selectedBusinessLine').planTypeId !== this.get('originalContractBusinessLineItem').planTypeId
                )) {                    
                    return true;
                } else
                    return false;
            },
            launchContractBusinessLineFormWnd: function () {
                var formWindow = $(".js-businesslineform-window-" + contractId).kendoWindow({
                    width: 600,
                    modal: true,
                    actions: []
                }).data("kendoWindow");
                formWindow.open().center();
            },
            isValidBusinessContractModel: function () {
                var valid = true,
                    selectedBusinessLineMessage = "",
                    selectedPlaceOfServiceMessage = "";

                if (!this.get('selectedBusinessLine')) {
                    valid = false;
                    selectedBusinessLineMessage = "<li>The Line of Business field is required.</li>" ;
                }

                if (!this.get('selectedPlaceOfService') || (this.get('selectedPlaceOfService') != null && this.get('selectedPlaceOfService').length === 0)) {
                    valid = false;
                    selectedPlaceOfServiceMessage = "<li>The Location field is required.</li>";
                }

                if (!valid) {
                    toastr.error(`<ul>${selectedBusinessLineMessage}${selectedPlaceOfServiceMessage}</ul>`);
                }
                return valid;
            },
            onAddBusinessLineToContract: function () {
                this.launchContractBusinessLineFormWnd();
                this.set('addBtnVisible', true);
                this.set('editBtnVisible', false);
            },
            addBusinessLineToContract: function () {
                if (this.isValidBusinessContractModel()) {
                    var contractBusinessLineItem = {
                        ContractId: contractId,
                        PlanTypeId: this.get('selectedBusinessLine').planTypeId,
                        Name: this.get('selectedBusinessLine').name,
                        Clinics: this.get('selectedPlaceOfService').map(function (pos) {
                            return {
                                PlaceOfServiceId: pos.placeOfServiceId,
                                Name: pos.name
                            }
                        })
                    };
                    //add a data item to a remote data source
                    var addBusinessLineSuccess = function (response) {
                        console.log("Add new Line of Business to the contract: success");
                        contractBusinessLineViewModel.get('contractBusinesslinesDataSource').pushCreate(response);
                        contractBusinessLineViewModel.get('businessLinesDataSource').pushDestroy({ planTypeId: response.planTypeId, name: response.name });
                        contractBusinessLineViewModel.canceladdBusinessLineToContract();
                        toastr.success("Line of Business successfuly added to the Contract.");
                    };
                    var addBusinessLineFails = function (response) {
                        console.log('Post request to ContractBusinessLines fails: ', response);
                        toastr.error("Internal Server Error. Please contact your system administrator.");
                    };
                    AjaxCallPost("/api/ContractBusinessLines", JSON.stringify(contractBusinessLineItem), addBusinessLineSuccess, addBusinessLineFails);
                }
            },
            convertToContractBusinessLineObject: function(data) {
                var contractBusinessLineItem = {
                    contractLineofBusinessId: data.contractLineofBusinessId,
                    contractId: data.contractId,
                    planTypeId: data.planTypeId,
                    name: data.name,
                    clinics: data.clinics.map(function (clinic) {
                        return {
                            id: clinic.id,
                            contractLineofBusinessId: clinic.contractLineofBusinessId,
                            placeOfServiceId: clinic.placeOfServiceId,
                            name: clinic.name
                        }
                    })
                };
                return contractBusinessLineItem;
            },
            onEditContractBusinessLineItem: function (e) {
                
                var contractBusinessLineItem = this.convertToContractBusinessLineObject(e.data);
                var businessLineItem = {
                    planTypeId: e.data.planTypeId,
                    name: e.data.name
                };
                var placeOfServiceItems = e.data.clinics.map(function(clinic) {
                    return {
                        placeOfServiceId: clinic.placeOfServiceId,
                        name: clinic.name
                    }
                });
                //this.get('placeOfServiceDataSource').pushCreate(contractBusinessLineItem.clinics);
                this.get('businessLinesDataSource').pushCreate(businessLineItem);
                //this.set('selectedPlaceOfService', contractBusinessLineItem.clinics);
                this.set('selectedPlaceOfService', placeOfServiceItems);
                this.set('selectedBusinessLine', businessLineItem);
                this.set('addBtnVisible', false);
                this.set('editBtnVisible', true);
                this.set('originalContractBusinessLineItem', contractBusinessLineItem);
                this.launchContractBusinessLineFormWnd();
            },
            editContractBusinessLineItem: function () {
                if (this.isValidBusinessContractModel()) {
                    
                    var originalContractBusinessLineItem = this.get("originalContractBusinessLineItem"),
                        currentBusinessLineItem = this.get('selectedBusinessLine'),
                        currentPlaceOfServiceItems = this.get('selectedPlaceOfService'),
                        currentClinicContractBusinessLinesItems = currentPlaceOfServiceItems.map(function (pos) {
                            
                            var searchItem = originalContractBusinessLineItem.clinics.find(function (item) {
                                return item.placeOfServiceId === pos.placeOfServiceId;
                            });
                            return {
                                Id: searchItem == undefined ? generateGuid() : searchItem.id,
                                PlaceOfServiceId: pos.placeOfServiceId,
                                Name: pos.name,
                                ContractLineofBusinessId: originalContractBusinessLineItem.contractLineofBusinessId
                            };
                        });
                    
                   
                    var contractBusinessLineItem = {
                        ContractLineofBusinessId: originalContractBusinessLineItem.contractLineofBusinessId,
                        ContractId: contractId,
                        PlanTypeId: currentBusinessLineItem.planTypeId,
                        Name: currentBusinessLineItem.name,
                        Clinics: currentClinicContractBusinessLinesItems
                        
                    };
                    var updateContractBusinessLineSucess = function (response) {
                        console.log("Change line of business of the contract: success");

                        var previousLineOfBusiness = contractBusinessLineViewModel.get("originalContractBusinessLineItem").planTypeId;
                        var currentLineOfBusiness = response.planTypeId;
                        if (currentLineOfBusiness !== previousLineOfBusiness) {
                            contractBusinessLineViewModel.get('businessLinesDataSource').pushCreate({
                                planTypeId: previousLineOfBusiness,
                                name: contractBusinessLineViewModel.get("originalContractBusinessLineItem").name
                            });
                            contractBusinessLineViewModel.get('businessLinesDataSource').pushDestroy({
                                planTypeId: response.planTypeId,
                                name: response.name
                            });
                        }

                        contractBusinessLineViewModel.get('contractBusinesslinesDataSource').read();
                        contractBusinessLineViewModel.canceladdBusinessLineToContract();
                        contractBusinessLineViewModel.set('originalContractBusinessLineItem', null);
                        toastr.success("Contract successfuly updated.");
                    };
                    AjaxCallPut("/api/ContractBusinessLines", JSON.stringify(contractBusinessLineItem), updateContractBusinessLineSucess);
                }
            },
            onReleaseContractBusinessLineItem: function(e) {
                var contractBusinessLineItem = this.convertToContractBusinessLineObject(e.data);

                var releaseContractBusinessLineTemplate = kendo.template($("#release-businessLine-template").html());

                var window = $(".js-notification-dialog").kendoWindow({
                    title: "Confirmation",
                    modal: true,
                    visible: false, //the window will not appear before its .open method is called
                    width: "400px"
                }).data("kendoWindow");

                window.content(releaseContractBusinessLineTemplate(contractBusinessLineItem)); //send the row data object to the template and render it
                window.center().open();

                $("#js-releasebusinessline-yesButton").click(function () {
                    var releaseContractBusinessLineSuccess = function (response) {
                        //Release the ContractBusinessLine item from the DataSource
                        contractBusinessLineViewModel.get('contractBusinesslinesDataSource').pushDestroy(response);
                        //Put the Line of business item available to use for new ContractBusinessLine
                        contractBusinessLineViewModel.get('businessLinesDataSource').pushCreate({planTypeId: response.planTypeId, name: response.name});
                        toastr.success("Line of Business succesfully released.");
                        window.close();
                    }
                    AjaxCallDelete("/api/ContractBusinessLines", JSON.stringify(contractBusinessLineItem), releaseContractBusinessLineSuccess);
                });
                $("#js-releasebusinessline-noButton").click(function () {
                    window.close();
                });
            },
            canceladdBusinessLineToContract: function () {
                if (this.get('editBtnVisible') === true && this.get('originalContractBusinessLineItem')) {
                    this.get('businessLinesDataSource').pushDestroy({
                        planTypeId: this.get('originalContractBusinessLineItem').planTypeId,
                        name: this.get('originalContractBusinessLineItem').name
                    });
                }
                this.set('selectedBusinessLine', null);
                this.set('selectedPlaceOfService', null);
                var formWindow = $(".js-businesslineform-window-" + contractId).data("kendoWindow");
                if (formWindow)
                    formWindow.close();
            },
            contractBusinesslinesDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: "contractLineofBusinessId",
                        fields: {
                            contractLineofBusinessId: "contractLineofBusinessId",
                            contractId: {
                                defaultValue: contractId,
                                required: true
                            },
                            planTypeId: "planTypeId",
                            name: "name",
                            clinics: "clinics"
                        }
                  }  
                },
                transport: {
                    read: function(options) {
                        $.ajax({
                            method: "GET",
                            url: "/api/ContractBusinessLines",
                            data: {contractId: contractId},
                            dataType: "json",
                            success: function(response) {
                                options.success(response);
                            },
                            error: function(response) {
                                options.error(response);
                            }
                        });
                    }
                }
            }),

            //Clinic-Contract-BusinessLine Section
            deleteClinicContractBusinessLineItem: function(e) {               
                var clinicContractBusinessLineItem = {
                    Id: e.data.id,
                    ContractLineofBusinessId: e.data.contractLineofBusinessId,
                    PlaceOfServiceId: e.data.placeOfServiceId,
                    Name: e.data.name
                };

                var releaseClinicContractBusinessLineTemplate = kendo.template($("#release-cliniccontractbusinessLine-template").html());

                var window = $(".js-notification-dialog").kendoWindow({
                    title: "Confirmation",
                    modal: true,
                    visible: false, //the window will not appear before its .open method is called
                    width: "400px"
                }).data("kendoWindow");

                window.content(releaseClinicContractBusinessLineTemplate(clinicContractBusinessLineItem)); //send the row data object to the template and render it
                window.center().open();

                $("#js-cliniccontractbusinessLine-yesButton").click(function () {
                    var suceessrelease = function () {
                        $(e.currentTarget).closest("tr").remove();
                        window.close();
                        toastr.success("The Location was successfully released.");
                        /*Si no descomento esta linea el location desaparece,
                         * sin embargo si doy click en el edit este location
                         * sigue apareciendo seleccionada en el multiselect
                         */
                        //contractBusinessLineViewModel.contractBusinesslinesDataSource.read();
                    }
                    AjaxCallDelete("/api/ClinicContractBusinessLines", JSON.stringify(clinicContractBusinessLineItem), suceessrelease);
                });
                $("#js-cliniccontractbusinessLine-noButton").click(function() {
                    window.close();
                });
            }
        });
        kendo.bind($(".js-businesslines-" + contractId), contractBusinessLineViewModel);
    }

    //Access to private members
    return {
        contractWindowComponent: contractWindowComponent,
        contractUploadComponent: contractUploadComponent,
        createContractViewModel: createContractViewModel,
        contractExpandedRow: contractExpandedRow,
        createAddendumViewModel: createAddendumViewModel,
        addendumFilesFromKendoUpload: addendumFilesFromKendoUpload,
        createContractBusinessLineViewModel: createContractBusinessLineViewModel
    }
}(ContractService);