var IndividualProvidersController = function() {
    //private properties
    var button,
        individualProviderItemToEdit,
        actionStatus;

    //private methods
    var init = function (container) {
        $(container).on("click", ".js-individualprovider-addbtn", displayFormWindow);
    }

    var displayFormWindow = function(e) {
        button = $(e.target);

        var doctorId = button.attr("data-doctor-id");
        var listView = $("#IndividualProvider_" + doctorId).data("kendoListView");
        listView.add();
        e.preventDefault();
    }

    var createDoctorIndividualProviderViewModel = function(doctor) {
        var viewModel = kendo.observable({
            InsuranceId: null,
            
            insuranceDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: "InsuranceId",
                        fields: {
                            InsuranceId: "InsuranceId",
                            Name: "Name"
                        }
                    }
                },
                transport: {
                    read: function(options) {
                        $.ajax({
                            url: domainName + "/Insurances/GetAvailableInsurance",
                            dataType: "json",
                            method: 'GET',
                            data: { doctorId: doctor.DoctorId },
                            success: function(response) {
                                if (individualProviderItemToEdit != null && actionStatus === "Edit") {
                                    
                                    response.push({
                                        InsuranceId: individualProviderItemToEdit.InsuranceId,
                                        Name: individualProviderItemToEdit.InsuranceName
                                    });

                                    var insuranceDrp = $("#" + individualProviderItemToEdit.DoctorIndividualProviderId)
                                        .data("kendoDropDownList");

                                    insuranceDrp.value(individualProviderItemToEdit.InsuranceId);

                                    console.log("drp: ", insuranceDrp, "value: ", insuranceDrp.value());
                                }
                                options.success(response);
                            },
                            error: function(response) {
                                options.error(response);
                            }
                        });
                    }
                }
            }),

            individualProviderDataSource: new kendo.data.DataSource({
                schema: {
                    model: {
                        id: 'DoctorIndividualProviderId',
                        fields: {
                            DoctorIndividualProviderId: 'DoctorIndividualProviderId',
                            DoctorId: {
                                defaultValue: doctor.DoctorId
                            },
                            InsuranceId: 'InsuranceId',
                            InsuranceName: 'InsuranceName',
                            ProviderNumber: 'ProviderNumber',
                            IndividualProviderEffectiveDate: 'IndividualProviderEffectiveDate'
                        }
                    }
                },
                transport: {
                    read: function (options) {
                        // make JSONP request to https://demos.telerik.com/kendo-ui/service/products
                        $.ajax({
                            url: domainName + "/IndividualProviders/GetIndividualProviders",
                            dataType: "json", // "jsonp" is required for cross-domain requests; use "json" for same-domain requests
                            data: {
                                doctorId: doctor.DoctorId
                            },
                            success: function (result) {
                                // notify the data source that the request succeeded
                                options.success(result);
                            },
                            error: function (result) {
                                // notify the data source that the request failed
                                options.error(result);
                            }
                        });
                    },
                    create: function(options) {
                        console.log(options);
                        $.ajax({
                            url: "/api/IndividualProviders/SaveIndividualProvider",
                            method: 'POST',
                            dataType: 'json',
                            data: {
                                DoctorId: options.data.DoctorId,
                                InsuranceId: options.data.InsuranceId,
                                ProviderNumber: options.data.ProviderNumber,
                                IndividualProviderEffectiveDate: moment(options.data.IndividualProviderEffectiveDate).format('L')
                            },
                            success: function (response) {
                                console.log("save individual provider success: ", response);
                                response = {
                                    DoctorIndividualProviderId: response.doctorIndividualProviderId,
                                    DoctorId: response.doctorId,
                                    InsuranceId: response.insuranceId,
                                    InsuranceName: response.insuranceName,
                                    ProviderNumber: response.providerNumber,
                                    IndividualProviderEffectiveDate: moment(response.individualProviderEffectiveDate).format('L')
                                };
                               
                                options.success(response);
                            },
                            error: function (response) {
                                console.log("save individual provider fails: ", response);
                                viewModel.get("individualProviderDataSource").cancelChanges();
                                options.error(response);
                                switch (response.status) {
                                    case 400: //Bad Request.
                                        var errorMessage = "";
                                        var modelstate = response.responseJSON.modelState;
                                        if (modelstate) {
                                            debugger;
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
                            }
                        });
                    },
                    update: function (options) {
                       
                        $.ajax({
                            url: "/api/IndividualProviders/SaveIndividualProvider",
                            dataType: "json", // "jsonp" is required for cross-domain requests; use "json" for same-domain requests
                            method: "POST",
                            // send the updated data items as the "models" service parameter encoded in JSON
                            data: {
                                DoctorIndividualProviderId: options.data.DoctorIndividualProviderId,
                                DoctorId: options.data.DoctorId,
                                InsuranceId: options.data.InsuranceId,
                                ProviderNumber: options.data.ProviderNumber,
                                IndividualProviderEffectiveDate: moment(options.data.IndividualProviderEffectiveDate).format('L')
                            },
                            success: function (response) {
                               
                                response = {
                                    DoctorIndividualProviderId: response.doctorIndividualProviderId,
                                    DoctorId: response.doctorId,
                                    InsuranceId: response.insuranceId,
                                    InsuranceName: response.insuranceName,
                                    ProviderNumber: response.providerNumber,
                                    IndividualProviderEffectiveDate: moment(response.individualProviderEffectiveDate).format('L')
                                };
                                // notify the data source that the request succeeded
                                options.success(response);
                            },
                            error: function (response) {
                                // notify the data source that the request failed
                                console.log("save individual provider fails: ", response);
                                viewModel.get("individualProviderDataSource").cancelChanges();
                                options.error(response);
                                switch (response.status) {
                                case 400: //Bad Request.
                                    var errorMessage = "";
                                    var modelstate = response.responseJSON.modelState;
                                    if (modelstate) {
                                        debugger;
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
                            }
                        });
                    }
                }
            }),

            onEditIndividualProviderItem: function (e) {

                if (!e.model.isNew()) {
                    individualProviderItemToEdit = e.model;
                    actionStatus = "Edit";
                    this.get('insuranceDataSource').read();
                    //var insuranceDrp = $("#" + e.model.DoctorIndividualProviderId)
                    //    .data("kendoDropDownList");
                    //console.log("datasource before: ", insuranceDrp.dataSource.view());
                    //insuranceDrp.dataSource.pushCreate({
                    //    InsuranceId: e.model.InsuranceId,
                    //    Name: e.model.InsuranceName
                    //});
                    //console.log("datasource after: ", insuranceDrp.dataSource.view());
                    //insuranceDrp.value({
                    //    InsuranceId: e.model.InsuranceId,
                    //    Name: e.model.InsuranceName
                    //});
                    //insuranceDrp.trigger("change");
                } else {
                    individualProviderItemToEdit = null;
                    actionStatus = "Create";
                }
            },
            
            onAddIndividualProvider: function() {
                var listView = $("#IndividualProvider_" + doctor.DoctorId).data("kendoListView");
                listView.add();
            },
            
        });
        kendo.bind($("#individualprovider_" + doctor.DoctorId), viewModel);
    }

    //Access to privte members
    return {
        init: init,
        createDoctorIndividualProviderViewModel: createDoctorIndividualProviderViewModel,
        individualProviderItemToEdit: individualProviderItemToEdit
    }
}()