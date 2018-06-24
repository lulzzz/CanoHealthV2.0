function onEnableContractFormWindow() {
    var contractWindow = $(".js-contractform-window").kendoWindow({
        modal: true,
        title: "CONTRACT",
        actions: ["Minimize", "Maximize", "Close"],
        width: 500,
        close: function(e) {
            var contractUploadComponent = $("#Addendum").data('kendoUpload');
            if (contractUploadComponent)
                contractUploadComponent.removeAllFiles();
        }
    }).data('kendoWindow');
    contractWindow.open().center();
    
    ContractsController.createContractViewModel();
}

function onExpandContractRow(e) {
    // Only one contract row open at a time
    if (ContractsController.contractExpandedRow != null &&
        ContractsController.contractExpandedRow[0] != e.masterRow[0]) {
        var grid = $('#Contracts').data('kendoGrid');
        grid.collapseRow(ContractsController.contractExpandedRow); //colapsa la fila expandida anteriormente
        ContractsController.addendumFilesFromKendoUpload = []; //reinicia el arreglo de ficheros en caso de que hubiesen ficheros seleccionados de la fila anterior
    }
    ContractsController.contractExpandedRow = e.masterRow;

    var contract = e.sender.dataItem(e.masterRow);
    
    ContractsController.createAddendumViewModel(contract.ContractId, contract.Active);
    ContractsController.createContractBusinessLineViewModel(contract.ContractId, contract.CorporationId, contract.Active);
}

function onDataBoundContractGrid(e) {
    var grid = $("#Contracts").data("kendoGrid");
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

function onCancelEditContractItem(e) {
    e.preventDefault();
    e.sender.dataSource.cancelChanges();
    if (!e.model.isNew() && e.model.Active) {
        e.container.find(".js-active").hide();
        e.container.find(".js-inactive").show();
    } else if (!e.model.isNew() && e.model.Active) {
        e.container.find(".js-active").show();
        e.container.find(".js-inactive").hide();
    }
}

function onClickInactivateContractButton(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    var releaseContractTemplate = kendo.template($("#inactive-contract-template").html());

    var window = $(".js-notification-dialog").kendoWindow({
        title: "Confirmation",
        modal: true,
        visible: false, //the window will not appear before its .open method is called
        width: "400px"
    }).data("kendoWindow");

    window.content(releaseContractTemplate(dataItem));

    window.center().open();

    $("#js-releasecontract-yesButton").click(function () {
        var onSuccessInactiveContract = function (response) {
            response = {
                Active: response.active,
                ContractId: response.contractId,
                CorporationId: response.corporationId,
                GroupNumber: response.groupNumber,
                InsuranceId: response.insuranceId
            };
            
            var contractGrid = $("#Contracts").data('kendoGrid');
            contractGrid.dataSource.pushUpdate(response);
        };
        var onFailIncativeContract = function(response) {
            console.log("Inactivate contract call fails: ", response);
            toastr.error(response.statusText);
        }
        AjaxCallDelete("/api/contracts/", JSON.stringify(dataItem), onSuccessInactiveContract, onFailIncativeContract);
        window.close();
    });
    $("#js-releasecontract-noButton").click(function () {
        window.close();
    });
}

function onClickActivateContractButton(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    dataItem.Active = true;

    var onSuccessActivateContract = function (response) {
        response = {
            Active: response.active,
            ContractId: response.contractId,
            CorporationId: response.corporationId,
            GroupNumber: response.groupNumber,
            InsuranceId: response.insuranceId
        };

        var contractGrid = $("#Contracts").data('kendoGrid');
        contractGrid.dataSource.pushUpdate(response);
    };
    var onFailActivateContract = function (response) {
        console.log("Inactivate contract call fails: ", response);
        toastr.error(response.statusText);
    }
    AjaxCallPost("/api/contracts/", JSON.stringify(dataItem), onSuccessActivateContract, onFailActivateContract);
}

//CONTRACT ADDENDUMS
function onSelectContractAddendumFile(e) {
    /*The Addendum File Kendo Upload component does not allow multiple files.*/

    if (ContractsController.addendumFilesFromKendoUpload.length === 0) {
        ContractsController.addendumFilesFromKendoUpload.push({
            kendoUploadId: e.sender.name,
            kendoUploadFiles: e.files
        });
    } else {
        var itemIndex = ContractsController.addendumFilesFromKendoUpload.findIndex(function (file) {
            return file.kendoUploadId === e.sender.name; //Closures
        });
        
        if (itemIndex === -1) {
            ContractsController.addendumFilesFromKendoUpload.push({
                kendoUploadId: e.sender.name,
                kendoUploadFiles: e.files
            });
        } else
            ContractsController.addendumFilesFromKendoUpload[itemIndex].kendoUploadFiles = e.files;
    }
}

function onRemoveContractAddendumFile(e) {
    var filesToDelete = e.files;
    var searchIndexInTheArray = ContractsController.addendumFilesFromKendoUpload.findIndex(function (itemArr) {
        return itemArr.kendoUploadId === e.sender.name;
    });
   

    if (searchIndexInTheArray !== -1) {
        var licenseFilesFromArray = ContractsController.addendumFilesFromKendoUpload[searchIndexInTheArray].kendoUploadFiles;
        var aux = licenseFilesFromArray.filter(function (el) {
            return !filesToDelete.includes(el);
        });
        ContractsController.addendumFilesFromKendoUpload[searchIndexInTheArray].kendoUploadFiles = aux;
    }
}