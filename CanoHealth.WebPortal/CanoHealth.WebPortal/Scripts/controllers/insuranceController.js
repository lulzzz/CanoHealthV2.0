var InsuranceController = function (insuranceService) {
    //private properties
    var insuranceExpandedRow = {};
    var button;

    //private methods
    var init = function (container) {
        $(container).on("click", ".js-insurancebusinessline-addbtn", displayFormWindow);
    };

    var displayFormWindow = function (e) {
        button = $(e.target);

        var insuranceId = button.attr("data-insurance-id");

        var window = $(".js-insurancebusinessline-formwnd").kendoWindow({
            modal: true,
            //width: 600,
            resizable: true,
            draggable: true
        }).data("kendoWindow");

        if (screen.width < 400)
            window.setOptions({ width: "100%" });
        else
            window.setOptions({ width: "40%" });

        window.open().center();

        var saveInsuranceBusinessLinesSuccess = function (response) {
            response = response.map(function (item) {
                return {
                    InsuranceBusinessLineId: item.insuranceBusinessLineId,
                    InsuranceId: item.insuranceId,
                    PlanTypeId: item.planTypeId,
                    Code: item.code,
                    Name: item.name
                };
            });
            window.refresh().close();
            var listView = $('#IsuranceBusinesLine_' + insuranceId).data("kendoListView");

            listView.dataSource.pushCreate(response);

            console.log("insurance business line success: ", response);
        };

        var saveInsuranceBusinessLinesFails = function (response) {
            tostr.error("We are sorry, but something went wrong. Please try again.");
            console.log("insurance business line fails: ", response);
        };

        var viewModel = kendo.observable({
            selectedBusinessLines: null,

            businessLinesDataSource: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: domainName + "/BusinessLines/GetBusinessLinesJson",
                        data: { insuranceId: insuranceId },
                        dataType: "json"
                    }
                }
            }),

            onAddBusinessLinesToInsurance: function () {
                var selectedBusinessLine = this.get("selectedBusinessLines");

                var insuranceBusinessLines = selectedBusinessLine.map(function (businessline) {
                    return {
                        PlanTypeId: businessline.PlanTypeId,
                        Code: businessline.Code,
                        Name: businessline.Name,
                        InsuranceId: insuranceId
                    };
                });

                insuranceService.saveInsuranceBusinessLines(insuranceBusinessLines, saveInsuranceBusinessLinesSuccess, saveInsuranceBusinessLinesFails);
            }
        });
        kendo.bind($(".js-insurancebusinessline-formwnd"), viewModel);
    };

    var checkIfThereAreBusinessLinesToSelect = function (insurance) {
        var success = function (response) {
            if (response.length === 0)
                $(".js-insurancebusinessline-addbtn").hide();
            else
                $(".js-insurancebusinessline-addbtn").show();
        };

        var fail = function (response) {
            $(".js-insurancebusinessline-addbtn").show();
        };
        insuranceService.availableBusinessLinesForTheInsurance(insurance.InsuranceId, success, fail);
    };

    //https://docs.telerik.com/aspnet-mvc/helpers/grid/how-to/editing/show-command-buttons-conditionally
    var showInactiveButton = function (dataItem) {
        // show the Inactive button for the item with Active=true
        return dataItem.Active;
    };

    var showActiveButton = function (dataItem) {
        // show the Active button for the item with Active=true
        return !dataItem.Active;
    };

    var onAddEditInsurance = function (e) {
        var currentUid = e.model.uid;
        var grid = e.sender;
        var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
        var activateButton = $(currenRow).find(".js-active");
        var inactivateButton = $(currenRow).find(".js-inactive");

        if (e.model.isNew()) {
            activateButton.hide();
            inactivateButton.show();
            //muestra el multiselect cuando se crea un insurance
            $(".js-lineofbusiness-multiselect").show();
        } else {
            //oculta el multiselect cuando se edite un record de insurance
            $(".js-lineofbusiness-multiselect").hide();

            if (e.model.Active === true) {
                activateButton.hide();
                inactivateButton.show();
            } else {
                activateButton.show();
                inactivateButton.hide();
            }
        }
    };   

    //function invoke when active/inactive insurance succesfully
    var onSuccessInsurance = function (response) {
        response = {
            InsuranceId: response.insuranceId,
            Code: response.code,
            Name: response.name,
            PhoneNumber: response.phoneNumber,
            Address: response.address,
            Active: response.active
        };
        var insuranceGrid = $("#Insurances").data('kendoGrid');
        insuranceGrid.dataSource.pushUpdate(response);
    };

    //function invoke when active/inactive insurance fails
    var onFailInsurance = function (response) {
        console.log("Save insurance fails: ", response);
        toastr.error(response.statusText);
    };

    var onClickInactivateInsuranceButton = function (e) {
        e.preventDefault();
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));        

        var releaseInsuranceTemplate = kendo.template($("#inactive-insurance-confirmation-template").html());

        var window = $(".js-notification-dialog").kendoWindow({
            title: "Confirmation",
            modal: true,
            visible: false, //the window will not appear before its .open method is called
            width: "400px"
        }).data("kendoWindow");

        window.content(releaseInsuranceTemplate(dataItem));

        window.center().open();

        $("#js-releaseinsurance-yesButton").click(function () {            
            AjaxCallDelete("/api/insurances/", JSON.stringify(dataItem), onSuccessInsurance, onFailInsurance);
            window.close();
        });
        $("#js-releaseinsurance-noButton").click(function () {
            window.close();
        });
    };

    var onClickActivateInsuranceButton = function (e) {
        e.preventDefault();
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        dataItem.Active = true;
        
        AjaxCallPut("/api/insurances/", JSON.stringify(dataItem), onSuccessInsurance, onFailInsurance);
    };

    //access to private members
    return {
        insuranceExpandedRow: insuranceExpandedRow,
        checkIfThereAreBusinessLinesToSelect: checkIfThereAreBusinessLinesToSelect,
        init: init,
        onAddEditInsurance: onAddEditInsurance,        
        onClickInactivateInsuranceButton: onClickInactivateInsuranceButton,
        onClickActivateInsuranceButton: onClickActivateInsuranceButton,
        showInactiveButton: showInactiveButton,
        showActiveButton: showActiveButton
    };
}(InsuranceService);