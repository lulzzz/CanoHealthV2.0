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

    LinkedContractDetailController.createObservable(contract.DoctorId, contract.ContractId, contract.InsuranceName);

}