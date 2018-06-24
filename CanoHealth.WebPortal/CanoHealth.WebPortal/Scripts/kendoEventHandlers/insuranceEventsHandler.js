var validationMessageTmpl = kendo.template($("#serverSideErrorHandlerTemp").html());

function serverSideErrorHandlers(args) {
    if (args.errors) {
        var grid = $("#Insurances").data("kendoGrid");
        grid.one("dataBinding", function (e) {
            e.preventDefault();   // cancel grid rebind if error occurs 
            for (var error in args.errors) {
                showMessage(grid.editable.element, error, args.errors[error].errors, grid);
            }
        });
    }
}

function showMessage(container, name, errors, grid) {
    //add the validation message to the form
    if (name) {
        console.log($("#" + name).parent());
        $("#" + name).parent().append(validationMessageTmpl({ field: name, message: errors[0] }));

        container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
                 .replaceWith(validationMessageTmpl({ field: name, message: errors[0] }));
    } else {
        toastr.error(errors[0]);
        grid.cancelChanges();
    }
}

function onDataBoundInsuranceGrid(e) {
    var grid = $("#Insurances").data("kendoGrid");
    var gridData = grid.dataSource.view();

    for (var i = 0; i < gridData.length; i++) {
        var currentUid = gridData[i].uid;
        var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
        var activateButton = $(currenRow).find(".js-active");
        var inactivateButton = $(currenRow).find(".js-inactive");
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

function onExpandInsuranceDetails(e) {
    //Get the current Insurance info
    var insurance = e.sender.dataItem(e.masterRow);

    // Only one insurance row open at a time.
    if (InsuranceController.insuranceExpandedRow != null && InsuranceController.insuranceExpandedRow[0] != e.masterRow[0]) {
        var grid = $('#Insurances').data('kendoGrid');
        grid.collapseRow(InsuranceController.insuranceExpandedRow); //colapsa la fila expandida anteriormente
    }
    InsuranceController.insuranceExpandedRow = e.masterRow; //actualiza doctorExpandedRow a la actual fila expandida.

    InsuranceController.checkIfThereAreBusinessLinesToSelect(insurance);
}

$(document).ready(function () {
    InsuranceController.init(".container-fluid");
});