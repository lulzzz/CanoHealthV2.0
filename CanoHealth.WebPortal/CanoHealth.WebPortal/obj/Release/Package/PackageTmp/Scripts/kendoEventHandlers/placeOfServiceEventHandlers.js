//PLACE OF SERVICE SERVER SIDE GRID VALIDATIONS
var validationMessageTmpl = kendo.template($("#serverSideErrorHandlerTemp").html());
function showMessage(container, name, errors) {
    //add the validation message to the form
    if (name) {
        console.log($("#" + name).parent());
        $("#" + name).parent().append(validationMessageTmpl({ field: name, message: errors[0] }));

        container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
                 .replaceWith(validationMessageTmpl({ field: name, message: errors[0] }));
    } else {
        toastr.error(errors[0]);
        var grid = $("#PlaceOfService").data("kendoGrid");
        grid.cancelChanges();
    }
}
function serverSideErrorHandlers(args) {
    if (args.errors) {
        var grid = $("#PlaceOfService").data("kendoGrid");
        grid.one("dataBinding", function (e) {
            e.preventDefault();   // cancel grid rebind if error occurs 
            for (var error in args.errors) {
                showMessage(grid.editable.element, error, args.errors[error].errors);
            }
        });
    }
}

//PLACE OF SERVICE GRID EVENTS
function onExpandPlaceOfServiceRow(e) {
    // Only one place of service row open at a time
    if (PlaceOfServiceController.placeOfServiceExpandedRow != null &&
        PlaceOfServiceController.placeOfServiceExpandedRow[0] != e.masterRow[0]) {
        var grid = $('#PlaceOfService').data('kendoGrid');
        grid.collapseRow(PlaceOfServiceController.placeOfServiceExpandedRow); //colapsa la fila expandida anteriormente y
        PlaceOfServiceController.licenseFilesFromKendoUpload = []; //reinicia el arreglo de ficheros en caso de que hubiesen ficheros seleccionados de la fila anterior
    }
    PlaceOfServiceController.placeOfServiceExpandedRow = e.masterRow;

    var placeOfService = e.sender.dataItem(e.masterRow);

    PlaceOfServiceController.createLicenseViewModel(placeOfService.PlaceOfServiceId);
}

//PLACE OF SERVICE LICENSES
function onSelectPlaceOfServiceLicenseFile(e) {
    /*The License File Kendo Upload component does not allow multiple files, because each Place of Service License
      just contain one file.*/
   
    if (PlaceOfServiceController.licenseFilesFromKendoUpload.length === 0) {
        PlaceOfServiceController.licenseFilesFromKendoUpload.push({
            kendoUploadId: e.sender.name,
            kendoUploadFiles: e.files
        });
    } else {
        var itemIndex = PlaceOfServiceController.licenseFilesFromKendoUpload.findIndex(function (file) {
            return file.kendoUploadId === e.sender.name; //Closures
        });
        //var itemIndex = PlaceOfServiceController.getLicenseFileIndex(e.sender.name);
        if (itemIndex === -1) {
            PlaceOfServiceController.licenseFilesFromKendoUpload.push({
                kendoUploadId: e.sender.name,
                kendoUploadFiles: e.files
            });
        } else
            PlaceOfServiceController.licenseFilesFromKendoUpload[itemIndex].kendoUploadFiles = e.files;
    }
}

function onRemovePlaceOfServiceLicenseFile(e) {
    var filesToDelete = e.files;
    var searchIndexInTheArray = PlaceOfServiceController.licenseFilesFromKendoUpload.findIndex(function (itemArr) {
        return itemArr.kendoUploadId === e.sender.name;
    });
    //var searchIndexInTheArray = PlaceOfServiceController.getLicenseFileIndex(e.sender.name);

    if (searchIndexInTheArray !== -1) {
        var licenseFilesFromArray = PlaceOfServiceController.licenseFilesFromKendoUpload[searchIndexInTheArray].kendoUploadFiles;
        var aux = licenseFilesFromArray.filter(function (el) {
            return !filesToDelete.includes(el);
        });
        PlaceOfServiceController.licenseFilesFromKendoUpload[searchIndexInTheArray].kendoUploadFiles = aux;
    }
}

//PLACE OF SERVICE DOCTORS
function onAddDoctorToPlaceOfService(placeOfServiceId) {
    
    var doctorFormWindow = $(".js-doctor-form-window").kendoWindow({
        modal: true,
        title: "DOCTOR",
        width: 600
    }).data("kendoWindow");
    
    doctorFormWindow.open().center();

    DoctorsController.createDoctorFormViewModel(placeOfServiceId);
}

function onunAssignDoctorToPlaceOfService(e) {
    e.preventDefault(); //prevent page scroll reset
    var tr = $(e.target).closest("tr"); //get the row for deletion
    var doctor = this.dataItem(tr); //get the row data so it can be referred later
    var gridComponentId = this.content.context.id; // Doctors_17e47a90-697b-4d85-8a10-7b0adc70e3b7
    var gridInstance = this;
    var placeOfServiceId;
    if (gridComponentId.indexOf('_') !== -1) {
        var arraySplit = gridComponentId.split('_');
        placeOfServiceId = arraySplit.length > 1 ? arraySplit[1] : arraySplit[0];
    } else
        placeOfServiceId = gridComponentId;

    var notificationWindow = $(".js-notification-dialog").kendoWindow({
        title: "Confirmation!",
        visible: false, //the window will not appear before its .open method is called
        modal: true,
        width: "400px"
    }).data('kendoWindow');
    var unassignTemplate = kendo.template($("#unassign-doctor-from-placeofservice-template").html());

    notificationWindow.content(unassignTemplate(doctor)); //send the row data object to the template and render it
    notificationWindow.center().open();

    var unassigndoctorsuccess = function (response) {
        notificationWindow.close();
        gridInstance.removeRow(tr);
        toastr.success("Doctor released successfully.");
    }

    var unassigndoctorfails = function(response) {
        console.log("Unassign doctor request fails: ", response);
        toastr.error("We are sorry, but something went wrong. Please try again.");
    }

    $("#js-unassigndoctor-yesButton").click(function () {
        var doctorClinicAssociationToRelease = {
            PlaceOfServiceId: placeOfServiceId,
            DoctorId: doctor.DoctorId,
            ActiveDoctorClinicRelationship: false
        };
        AjaxCallDelete("/api/ClinicDoctorTeams", JSON.stringify(doctorClinicAssociationToRelease), unassigndoctorsuccess, unassigndoctorfails);
    });
    $("#js-unassigndoctor-noButton").click(function() {
        notificationWindow.close();
    });
}