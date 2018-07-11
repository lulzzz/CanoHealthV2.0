var InsuranceController = function (insuranceService) {
    //private properties
    var insuranceExpandedRow = {};
    var button;

    //private methods
    var init = function(container) {
        $(container).on("click", ".js-insurancebusinessline-addbtn", displayFormWindow);
    }

    var displayFormWindow = function(e) {
        button = $(e.target);

        var insuranceId = button.attr("data-insurance-id");

        var window = $(".js-insurancebusinessline-formwnd").kendoWindow({
            modal: true,
            width: 600
        }).data("kendoWindow");

        window.open().center();

        var saveInsuranceBusinessLinesSuccess = function (response) {
            response = response.map(function(item) {
                return {
                    InsuranceBusinessLineId: item.insuranceBusinessLineId,
                    InsuranceId: item.insuranceId,
                    PlanTypeId: item.planTypeId,
                    Code: item.code,
                    Name: item.name
                 }
            });
            window.refresh().close();
            var listView = $('#IsuranceBusinesLine_' + insuranceId).data("kendoListView");

            listView.dataSource.pushCreate(response);

            console.log("insurance business line success: ", response);
        }

        var saveInsuranceBusinessLinesFails = function (response) {
            tostr.error("We are sorry, but something went wrong. Please try again.");
            console.log("insurance business line fails: ", response);
        }

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
               
                var insuranceBusinessLines = selectedBusinessLine.map(function(businessline) {
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
    }

    var checkIfThereAreBusinessLinesToSelect = function(insurance) {
        var success = function (response) {
            if (response.length === 0)
                $(".js-insurancebusinessline-addbtn").hide();
            else
                $(".js-insurancebusinessline-addbtn").show();
        }

        var fail = function(response) {
            $(".js-insurancebusinessline-addbtn").show();
        }
        insuranceService.availableBusinessLinesForTheInsurance(insurance.InsuranceId, success, fail);
    }  

    var onAddEditInsurance = function (e) {
        var currentUid = e.model.uid;
        var grid = e.sender;
        var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
        var activateButton = $(currenRow).find(".js-active");
        var inactivateButton = $(currenRow).find(".js-inactive");

        if (e.model.isNew()) {            
            activateButton.hide();
            inactivateButton.show();           
        } else {
            if (e.model.Active === true) {
                activateButton.hide();
                inactivateButton.show();
            } else {
                activateButton.show();
                inactivateButton.hide();
            }
        }
    };

    //access to private members
    return {
        insuranceExpandedRow: insuranceExpandedRow,
        checkIfThereAreBusinessLinesToSelect: checkIfThereAreBusinessLinesToSelect,
        init: init,
        onAddEditInsurance: onAddEditInsurance
    }
}(InsuranceService)