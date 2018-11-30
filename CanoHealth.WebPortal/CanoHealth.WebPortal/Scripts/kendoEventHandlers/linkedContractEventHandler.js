function onEnableLinkedContractFormWindow(doctorId) {     
    LinkedContractController.createLinkedContractViewModel(doctorId);
    var linkedContractFormWindow = $(".js-linked-contract-form-window_" + doctorId)
        .kendoWindow({
            modal: true,
            width: 600
        }).data("kendoWindow");
    linkedContractFormWindow.open().center();   
}

function onExpandContractDataRow(e) {
    //Get the current Contract info
    var contract = e.sender.dataItem(e.masterRow);
    console.log("contract: ", contract);

    //Allow Single Row in Master Grid to be Expanded https://docs.telerik.com/kendo-ui/controls/data-management/grid/how-to/Layout/allow-a-single-expanded-row-only
    e.sender.tbody.find('.k-detail-row').each(function (idx, item) {
        if (item !== e.detailRow[0]) {
            e.sender.collapseRow($(item).prev());
        }
    });

    LinkedContractDetailController.createObservable(contract.DoctorId, contract.ContractId, contract.InsuranceName);

}