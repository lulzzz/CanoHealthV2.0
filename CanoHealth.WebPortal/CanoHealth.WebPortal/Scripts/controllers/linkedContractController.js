var LinkedContractController = function () {
    //private methods.
    var createLinkedContractViewModel = function (doctorId) {

        var linkedContractViewModel = kendo.observable({
            selectedContract: null,
            selectedBusinessLine: null,
            effectiveDate: null,
            note: null,

            //Out of Network contracts section
            outofnetwork: null,
            ooEffectiveDate: null,
            isOOVisible: function () {
                if (this.get('outofnetwork') === true || this.get('outofnetwork') == "true")
                    return true;
                else
                    return false;
            },
            onSaveOutOfNetworkContract: function () {
                if (!this.get('ooEffectiveDate')) {
                    toastr.error("The effective date field is required.");
                    return;
                } else {
                    var outOfNetworkContract = {
                        DoctorId: doctorId,
                        InsurnaceId: this.get('selectedContract').insuranceId,
                        EffectiveDate: this.get('ooEffectiveDate')
                    };
                    var saveOutOfNetworkContractSuccess = function (response) {
                        console.log("save out of network succeed: ");
                        linkedContractViewModel.resetFieldsToDefaultValues();
                        $(".js-individual-provider-notification").data("kendoWindow").close();
                        $(".js-linked-contract-form-window_" + doctorId)
                            .data("kendoWindow").close();
                        toastr.success("Out of Network contract was successfully created.");

                        linkedContractViewModel.get('contractDataSource').filter({ field: "insuranceId", operator: "neq", value: response.insurnaceId });
                    };
                    var saveOutOfNetworkContractFails = function (response) {
                        console.log("save out of network failed: ", response);
                        $(".js-individual-provider-notification").data("kendoWindow").close();
                        $(".js-linked-contract-form-window_" + doctorId)
                            .data("kendoWindow").close();
                        toastr.error("We are sorry, but something went wrong. Please try again.");
                    };
                    AjaxCallPost("/api/OutofNetworkContracts", JSON.stringify(outOfNetworkContract), saveOutOfNetworkContractSuccess, saveOutOfNetworkContractFails);
                }
            },

            //Individual Provider Section
            individualProviderNumber: null,
            individualproviderEffectiveDate: null,
            isIPVisible: function () {
                if (this.get('outofnetwork') === false || this.get('outofnetwork') == "false")
                    return true;
                else
                    return false;
            },
            checkRequiredFiedsForIndividualProviderSection: function () {
                var isValid = true,
                    providerN = "",
                    effDate = "";
                if (!this.get('individualProviderNumber')) {
                    isValid = false;
                    providerN = "<li>The Provider Number field is required.</li>";
                }
                if (!this.get('individualproviderEffectiveDate')) {
                    isValid = false;
                    effDate = "<li>The Effective Date field is required.</li>";
                }
                if (!isValid)
                    toastr.error(`<ul>${providerN}${effDate}</ul>`);
                return isValid;
            },
            onSaveIndividualProviderContract: function () {
                if (this.checkRequiredFiedsForIndividualProviderSection()) {
                    var insividualProvider = {
                        DoctorId: doctorId,
                        InsuranceId: this.get('selectedContract').insuranceId,
                        ProviderNumber: this.get("individualProviderNumber"),
                        IndividualProviderEffectiveDate: this.get("individualproviderEffectiveDate")
                    };
                    var saveIndividualProviderSucceed = function (response) {
                        console.log("save Individual Provider Succeed: ");
                        $(".js-individual-provider-notification_" + doctorId)
                            .data("kendoWindow").close();
                        linkedContractViewModel.set("individualProviderNumber", null);
                        linkedContractViewModel.set("individualproviderEffectiveDate", null);

                        //List view from Individual Provider tab
                        var individualProviderListView = $('#IndividualProvider_' + doctorId).
                            data('kendoListView');
                        response = {
                            DoctorIndividualProviderId: response.doctorIndividualProviderId,
                            DoctorId: response.doctorId,
                            InsuranceId: response.insuranceId,
                            InsuranceName: response.insuranceName,
                            ProviderNumber: response.providerNumber,
                            IndividualProviderEffectiveDate: moment(response.individualProviderEffectiveDate).format('L')
                        }
                        individualProviderListView.dataSource.pushCreate(response);

                        toastr.success("Individual Provider contract was successfully created.");
                    };
                    var saveIndividualProviderFailed = function (response) {
                        console.log("save Individual Provider failed: ", response);
                        $(".js-individual-provider-notification_" + doctorId)
                            .data("kendoWindow").close();
                        $(".js-linked-contract-form-window_" + doctorId)
                            .data("kendoWindow").close();
                        toastr.error("We are sorry, but something went wrong. Please try again.");
                    };
                    AjaxCallPost("/api/IndividualProviders/", JSON.stringify(insividualProvider), saveIndividualProviderSucceed, saveIndividualProviderFailed);
                }
            },

            //Global Linked contract section
            contractDataSource: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "/api/Contracts/GetActiveContractWithInsuranceCorporationInfo",
                        dataType: "json",
                        method: "GET",
                        data: { doctorId: doctorId }
                    }
                }
            }),
            displaySelectedContract: function () {
                var selectedContract = this.get("selectedContract");
                return JSON.stringify(selectedContract, null, 4);
            },
            onChangeLinkedContracts: function () {

                var individualProvider = {
                    doctorId: doctorId,
                    insuranceId: this.get("selectedContract").insuranceId
                };
                var getIndividualProviderSuccess = function (response) {
                    //si el doctor no tiene un individual provider con el seguro
                    if (!response) {
                        linkedContractViewModel.set("outofnetwork", true);

                        var window = $(".js-individual-provider-notification_" + doctorId)
                            .kendoWindow({
                                title: "Confirmation!",
                                visible: false, //the window will not appear before its .open method is called
                                width: "400px",
                                modal: true,
                                close: function (e) {
                                    linkedContractViewModel.resetFieldsToDefaultValues();
                                }
                            })
                            .data("kendoWindow");


                        window.center().open();
                    }

                };
                AjaxCallGet("api/IndividualProviders/GetIndividualProviderByDoctorAndInsurance",
                    individualProvider, getIndividualProviderSuccess);
            },
            businessLineDataSource: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "/api/ContractBusinessLines/GetContractBusinessLines",
                        dataType: "json",
                        method: "GET"
                    }
                }
            }),
            displaySelectedBusinessLine: function () {
                var selectedBusinessLine = this.get("selectedBusinessLine");
                return JSON.stringify(selectedBusinessLine, null, 4);
            },
            onChangeBusinessLines: function () {

            },
            checkRequiredFieldsForLinkedContracts: function () {
                var isValid = true,
                    contract = "",
                    businessline = "",
                    date = "";
                if (!this.get('selectedContract')) {
                    isValid = false;
                    contract = "<li>The Contract field is required.</li>"
                }
                if (!this.get('selectedBusinessLine')) {
                    isValid = false;
                    businessline = '<li>The Business Line field is required.</li>'
                }
                if (!this.get('effectiveDate')) {
                    isValid = false;
                    date = "<li>The Effective Date field is required.</li>"
                }
                if (!isValid)
                    toastr.error(`<ul>${contract}${businessline}${date}</ul>`);
                return isValid;
            },
            onLinkDoctorToContract: function () {
                if (this.checkRequiredFieldsForLinkedContracts()) {
                    var linkedContractObject = {
                        ContractLineofBusinessId: this.get('selectedBusinessLine').contractLineofBusinessId,
                        DoctorId: doctorId,
                        Note: this.get('note'),
                        EffectiveDate: moment(this.get('effectiveDate')).format('L'),
                        CorporationName: this.get('selectedContract').corporationName,
                        InsuranceName: this.get('selectedContract').name,
                        GroupNumber: this.get('selectedContract').groupNumber
                    };
                    var linkedContractSucceed = function (response) {
                        console.log("Link doctor to contract succeed: ");
                        var doctorContractGrid = $("#DoctorContract_" + doctorId)
                            .data('kendoGrid');
                        var contract = doctorContractGrid.dataSource.get(response.groupNumber);
                        if (!contract) {
                            doctorContractGrid.dataSource.pushCreate({
                                DoctorCorporationContractLinkId: response.doctorCorporationContractLinkId,
                                DoctorId: doctorId,
                                CorporationName: response.corporationName,
                                InsuranceName: response.insuranceName,
                                GroupNumber: response.groupNumber
                            });
                        }

                        var datailedLinkedContract = $("#contracts_" + response.groupNumber).data('kendoListView');
                        if (datailedLinkedContract)
                            datailedLinkedContract.dataSource.read();

                        linkedContractViewModel.resetFieldsToDefaultValues();
                        $(".js-linked-contract-form-window_" + doctorId).data('kendoWindow').close();
                    };
                    var linkedContractFailed = function (response) {
                        console.log("Link doctor to contract failed: ", response);
                        switch (response.status) {
                            case 400: //Bad Request.
                                var errorMessage = "";
                                var modelstate = response.responseJSON.modelState;
                                if (modelstate) {

                                    for (var prop in modelstate) {
                                        if (modelstate.hasOwnProperty(prop)) {
                                            errorMessage = errorMessage + " " + modelstate[prop].toString();
                                        }
                                    }
                                }
                                toastr.error(errorMessage);
                                break;
                            default:
                                toastr.error("We are sorry, but something went wrong. Please try again.");
                                break;
                        }
                        linkedContractViewModel.resetFieldsToDefaultValues();
                        $(".js-linked-contract-form-window_" + doctorId).data('kendoWindow').close();
                    };
                    AjaxCallPost("/api/LinkedContracts", JSON.stringify(linkedContractObject),
                        linkedContractSucceed, linkedContractFailed);
                }
            },
            onCloseLinkedContractFormWindow: function () {
                $(".js-linked-contract-form-window_" + doctorId).data("kendoWindow").close();
                this.resetFieldsToDefaultValues();
            },
            resetFieldsToDefaultValues: function () {
                this.set('selectedContract', null);
                this.set('selectedBusinessLine', null);
                this.set('effectiveDate', null);
                this.set('note', null);

                //Out of Network contracts section
                this.set('outofnetwork', null);
                this.set('ooEffectiveDate', null);

                //Individual Provider Section
                this.set('individualProviderNumber', null);
                this.set('individualproviderEffectiveDate', null);
            }
        });
        kendo.bind($("#js-linked-contract-form-section_" + doctorId), linkedContractViewModel);
    };

    //access to private members
    return {
        createLinkedContractViewModel: createLinkedContractViewModel
    };
}();