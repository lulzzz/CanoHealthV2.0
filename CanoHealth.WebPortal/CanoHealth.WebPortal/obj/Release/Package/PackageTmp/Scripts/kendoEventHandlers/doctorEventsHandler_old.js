﻿var doctorvalidationMessageTmpl = kendo.template($("#serverside-errorhandler-Template").html());

function doctorshowMessage(container, name, errors) {
    //add the validation message to the form
    if (name) {
        console.log($("#" + name).parent());
        $("#" + name).parent().append(doctorvalidationMessageTmpl({ field: name, message: errors[0] }));

        container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
                 .replaceWith(doctorvalidationMessageTmpl({ field: name, message: errors[0] }));
    } else {
        toastr.error(errors[0]);
        var grid = $("#Doctors").data("kendoGrid");
        grid.cancelChanges();
    }
}

function doctorserverSideErrorHandlers(args) {
    if (args.errors) {
        var grid = $("#Doctors").data("kendoGrid");
        grid.one("dataBinding", function (e) {
            e.preventDefault();   // cancel grid rebind if error occurs 
            for (var error in args.errors) {
                doctorshowMessage(grid.editable.element, error, args.errors[error].errors);
            }
        });
    }
}

function inactivateDoctor(e) {
    e.preventDefault(); //prevent page scroll reset
    var tr = $(e.target).closest("tr"); //get the row for deletion
    var doctor = this.dataItem(tr); //get the row data so it can be referred later

    var releaseDoctorTemplate = kendo.template($("#release-doctor-template").html());

    var doctorWindow = $(".js-release-doctor-wnd").kendoWindow({
        title: "Confirmation!",
        visible: false, //the window will not appear before its .open method is called
        modal: true,
        width: "400px"
    }).data("kendoWindow");

    doctorWindow.content(releaseDoctorTemplate(doctor)); //send the row data object to the template and render it
    doctorWindow.center().open();

    $("#js-yes-btn").click(function () {
        var grid = $("#Doctors").data('kendoGrid');
        grid.dataSource.remove(doctor); //prepare a "destroy" request
        grid.dataSource.sync(); //actually send the request (might be ommited if the autoSync option is enabled in the dataSource)
        doctorWindow.close();
    });
    $("#js-no-btn").click(function () {
        doctorWindow.close();
    });
}

function onExpandDoctorRow(e) {
    //Get the current Doctor info
    var doctor = e.sender.dataItem(e.masterRow);

    // Only one doctor row open at a time
    if (DoctorsController.doctorExpandedRow != null && DoctorsController.doctorExpandedRow[0] != e.masterRow[0]) {
        var grid = $('#Doctors').data('kendoGrid');
        grid.collapseRow(DoctorsController.doctorExpandedRow); //colapsa la fila expandida anteriormente
        DoctorsController.medicalLicenseFilesFromKendoUpload = []; //reinicia el arreglo de ficheros en caso de que hubiesen ficheros seleccionados de la fila anterior
    }
    DoctorsController.doctorExpandedRow = e.masterRow; //actualiza doctorExpandedRow a la actual fila expandida.

    
    if (!doctor.Active) {
        //if doctor is inactive hide all k-edit-button related to those list view components
        $(".k-edit-button").hide();
        $(".k-delete-button").hide();
        //hide the add linked contract button.
        $(".js-addLinkedContractBtn_" + doctor.DoctorId).attr("disabled", true);

        $("#doctor-detail-template").on("click", "#linkedcontract_" + doctor.DoctorId, function () {
            alert("doctor inactivo.");
        });
    }        
    else {
        $(".k-edit-button").show();
        $(".k-delete-button").show();
        $(".js-addLinkedContractBtn_" + doctor.DoctorId).attr("disabled", false);

        $("#doctor-detail-template").on("click", "#linkedcontract_" + doctor.DoctorId, function () {
            alert("doctor activo.");
        });
    }
        

    DoctorsController.createDoctorMedicalLicenseViewModel(doctor);

    PersonalFilesController.createDoctorPersonalFileViewModel(doctor);

    IndividualProvidersController.createDoctorIndividualProviderViewModel(doctor);
}

function onDataBoundDoctorGrid(e) {
    var grid = $("#Doctors").data("kendoGrid");
    var gridData = grid.dataSource.view();

    for (var i = 0; i < gridData.length; i++) {
        var currentUid = gridData[i].uid;
        var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
        var activateButton = $(currenRow).find(".js-active-doctor");
        var inactivateButton = $(currenRow).find(".js-inactive-doctor");
        activateButton.width(inactivateButton.width());
        if (gridData[i].Active === true) {
            activateButton.hide();
            inactivateButton.show();
        } else {
            activateButton.show();
            inactivateButton.hide();
        }
    }
}

function onCancelEditDoctorItem(e) {
    e.preventDefault();
    e.sender.dataSource.cancelChanges();
    if (!e.model.isNew() && e.model.Active) {
        e.container.find(".js-active-doctor").hide();
        e.container.find(".js-inactive-doctor").show();
    } else if (!e.model.isNew() && e.model.Active) {
        e.container.find(".js-active-doctor").show();
        e.container.find(".js-inactive-doctor").hide();
    }
}

function onClickInactivateDoctorButton(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    var releaseDoctorTemplate = kendo.template($("#release-doctor-template").html());

    var window = $(".js-release-doctor-wnd").kendoWindow({
        title: "Confirmation",
        modal: true,
        visible: false, //the window will not appear before its .open method is called
        width: "400px"
    }).data("kendoWindow");

    window.content(releaseDoctorTemplate(dataItem));

    window.center().open();

    $("#js-releasedoctor-yesButton").click(function () {
        var onSuccessInactiveDoctor = function (response) {
            response = {
                DoctorId: response.doctorId,
                FirstName: response.firstName,
                LastName: response.lastName,
                DateOfBirth: moment(response.dateOfBirth).format('L'),
                Degree: response.degree,
                SocialSecurityNumber: response.socialSecurityNumber,
                NpiNumber: response.npiNumber,
                CaqhNumber: response.caqhNumber,
                Active: response.active
            };

            var doctorsGrid = $("#Doctors").data('kendoGrid');
            doctorsGrid.dataSource.pushUpdate(response);
        };
        var onFailIncativeDoctor = function (response) {
            console.log("Inactivate doctor call fails: ", response);
            toastr.error(response.statusText);
        }
        
        AjaxCallDelete("/api/doctors/", JSON.stringify(dataItem), onSuccessInactiveDoctor, onFailIncativeDoctor);
        window.close();
    });
    $("#js-releasedoctor-noButton").click(function () {
        window.close();
    });
}

function onClickActivateDoctorButton(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    dataItem.Active = true;

    var onSuccessActivateDoctor = function (response) {
        response = {
            DoctorId: response.doctorId,
            FirstName: response.firstName,
            LastName: response.lastName,
            DateOfBirth: moment(response.dateOfBirth).format('L'),
            Degree: response.degree,
            SocialSecurityNumber: response.socialSecurityNumber,
            NpiNumber: response.npiNumber,
            CaqhNumber: response.caqhNumber,
            Active: response.active
        };

        var doctorGrid = $("#Doctors").data('kendoGrid');
        doctorGrid.dataSource.pushUpdate(response);
    };
    var onFailActivateDoctor = function (response) {
        console.log("Activate contract call fails: ", response);
        toastr.error(response.statusText);
    };
    AjaxCallPost("/api/doctors/", JSON.stringify(dataItem), onSuccessActivateDoctor, onFailActivateDoctor);
}

function onSaveDoctorItem(e) {
    if (e.model.isNew())
        e.model.Active = true;
    console.log(e.model);

    var ssnMaskedtextbox = $("#SocialSecurityNumber").data("kendoMaskedTextBox");

    var npiMaskedtextbox = $("#NpiNumber").data("kendoMaskedTextBox");

    ssnMaskedtextbox.bind("change", function () {
        var value = this.value();
        console.log(value); //value is the selected date in the maskedtextbox
        if (value.indexOf(this.options.promptChar) !== -1) {
            toastr.error("Invalid SSN.");
            //means the ssn is incomplete hide the edit button
            $(".k-grid-update").hide();
        } else {
            //means the ssn is completed but we have to check if npi is complete
            if (npiMaskedtextbox.value().indexOf(this.options.promptChar) === -1)
                $(".k-grid-update").show();
        }
    });

    npiMaskedtextbox.bind("change", function () {
        var value = this.value();
        console.log(value); //value is the selected date in the maskedtextbox
        if (value.indexOf(this.options.promptChar) !== -1) {
            toastr.error("Invalid NPI.");
            //means the npi is incomplete hide the edit button
            $(".k-grid-update").hide();
        } else {
            //means the npi is completed
            DoctorService.getProviderInfo("https://npiregistry.cms.hhs.gov/api/?number=" + value + '&pretty=on', getProviderInfoSuccessWhenNpiChange, getProviderInfoFailWhenNpiChange);
            var getProviderInfoSuccessWhenNpiChange = function (response) {
                console.log("get Provider Info Success When Npi Change: ", response);
            };
            var getProviderInfoFailWhenNpiChange = function (response) {
                console.log("get Provider Info Fail When Npi Change: ", response);
            };

            //we have to check if ssn is complete in order to display the edit button
            if (ssnMaskedtextbox.value().indexOf(this.options.promptChar) === -1)
                $(".k-grid-update").show();
        }
    });
}

//Doctor Medical Licenses Section
function onSelectMedicalLicenseFile(e) {
    /*By convention The MedicalLicense File Kendo Upload component does not allow multiple files, because each Doctor License
     just contain one file.*/
    if (DoctorsController.medicalLicenseFilesFromKendoUpload.length === 0) {
        DoctorsController.medicalLicenseFilesFromKendoUpload.push({
            kendoUploadId: e.sender.name,
            kendoUploadFiles: e.files
        });
    }else {
        var itemIndex = DoctorsController.medicalLicenseFilesFromKendoUpload.findIndex(function (file) {
            return file.kendoUploadId === e.sender.name; //Closures
        });
        
        if (itemIndex === -1) {
            DoctorsController.medicalLicenseFilesFromKendoUpload.push({
                kendoUploadId: e.sender.name,
                kendoUploadFiles: e.files
            });
        } else
            DoctorsController.medicalLicenseFilesFromKendoUpload[itemIndex].kendoUploadFiles = e.files;
    }
}

function onRemoveMedicalLicenseFile(e) {
    var filesToDelete = e.files;
    var searchIndexInTheArray = DoctorsController.medicalLicenseFilesFromKendoUpload.findIndex(function (itemArr) {
        return itemArr.kendoUploadId === e.sender.name;
    });
    //var searchIndexInTheArray = PlaceOfServiceController.getLicenseFileIndex(e.sender.name);

    if (searchIndexInTheArray !== -1) {
        var licenseFilesFromArray = DoctorsController.medicalLicenseFilesFromKendoUpload[searchIndexInTheArray].kendoUploadFiles;
        var aux = licenseFilesFromArray.filter(function (el) {
            return !filesToDelete.includes(el);
        });
        DoctorsController.medicalLicenseFilesFromKendoUpload[searchIndexInTheArray].kendoUploadFiles = aux;
    }
}

//Linked Contracts
function locationProvidersErrorHandler(args) {
    if (args.errors) {
        var message = "Errors:\n";
        $.each(args.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        toastr.error(message);
        args.sender.cancelChanges();
    }
}