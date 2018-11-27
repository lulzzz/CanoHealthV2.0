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
    //from https://docs.telerik.com/kendo-ui/knowledge-base/grid-hide-expand-icon-based-on-field-value
   
    var items = e.sender.items();
    items.each(function () {
        var row = $(this);
        var dataItem = e.sender.dataItem(row);
        var activateButton = row.find(".js-active");
        var inactivateButton = row.find(".js-inactive");
        if (!dataItem.Active) {
            row.find(".k-hierarchy-cell").html("");
            activateButton.show();
            inactivateButton.hide();
        } else {
            activateButton.hide();
            inactivateButton.show();
        }
    });
    //old implementation
    //var grid = $("#Insurances").data("kendoGrid");
    //var gridData = grid.dataSource.view();
    //for (var i = 0; i < gridData.length; i++) {
    //    var currentUid = gridData[i].uid;
    //    var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
    //    var activateButton = $(currenRow).find(".js-active");
    //    var inactivateButton = $(currenRow).find(".js-inactive");
    //    activateButton.width(inactivateButton.width());
    //    if (gridData[i].Active === true) {
    //        activateButton.hide();
    //        inactivateButton.show();
    //    } else {
    //        activateButton.show();
    //        inactivateButton.hide();
    //    }
    //}
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

function onRemoveInsuranceLineofBusinessRecord(e) {
    e.preventDefault();

    var releaseTemplate = kendo.template($("#inactive-lineofbusiness-confirmation-template").html());

    var window = $(".js-notification-dialog").kendoWindow({
        title: "Confirmation",
        modal: true,
        visible: false, //the window will not appear before its .open method is called
        width: "400px"
    }).data("kendoWindow");

    window.content(releaseTemplate(e.model)); //send the row data object to the template and render it
    window.center().open();
    
    var listview = $("#IsuranceBusinesLine_" + e.model.InsuranceId).data("kendoListView");

    $("#js-releaseinsurancelineofbusiness-yesButton").click(function () {
        listview.dataSource.remove(e.model); //prepare a "destroy" request
        listview.dataSource.sync(); //actually send the request (might be ommited if the autoSync option is enabled in the dataSource)
        window.refresh().close();
    });
    $("#js-releaseinsurancelineofbusiness-noButton").click(function () {
        window.refresh().close();
    });
};

function addNewLineofBusiness(widgetId, value) {
    var widget = $("#" + widgetId).getKendoMultiSelect();

    var newLineofBusiness = {
        Name: value,
        Code: value,
        Active: true
    };
    var createLineofBusinessSuccess = function (response) {
        newLineofBusiness.PlanTypeId = response.planTypeId;
        widget.dataSource.pushCreate(newLineofBusiness);

        widget.value(widget.value().concat(response.planTypeId));
        widget.trigger("change");
    };
    AjaxCallPost("/api/BusinessLines/", JSON.stringify(newLineofBusiness), createLineofBusinessSuccess);
}

$(document).ready(function () {
    InsuranceController.init(".container-fluid");
});