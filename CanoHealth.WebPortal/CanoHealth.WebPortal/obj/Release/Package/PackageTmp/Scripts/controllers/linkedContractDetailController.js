var LinkedContractDetailController = function() {
    var createObservable = function (doctorId, contractGroupNumber, insuranceName) {
        var doctor = DoctorEventHandler.getSelectedDoctor();
        
        var viewModel = kendo.observable({
            //Doctor info to display
            firstName: null,
            lastName: null,
            dateOfBirth: null,
            degree: null,
            socialSecurityNumber: null,
            npiNumber: null,
            caqhNumber: null,
            active: null,

            //If expanded doctor is true display all buttons
            enableDoctorLinkedContractButtons: doctor.Active,

            onRemoveLinkedContractItem: function(e) {
                e.preventDefault();

                var releaseContractTemplate = kendo.template($("#linkedcontractplusdata-release-template").html());

                var window = $(".js-notification-dialog").kendoWindow({
                    title: "Confirmation",
                    modal: true,
                    visible: false, //the window will not appear before its .open method is called
                    width: "400px"
                }).data("kendoWindow");

                window.content(releaseContractTemplate(e.model)); //send the row data object to the template and render it
                window.center().open();

                $("#js-releaselinkedcontract-yesButton").click(function () {
                    viewModel.get('linkedContractDetailsDataSource').remove(e.model); //prepare a "destroy" request
                    viewModel.get('linkedContractDetailsDataSource').sync(); //actually send the request (might be ommited if the autoSync option is enabled in the dataSource)
                    window.refresh().close();
                });
                $("#js-releaselinkedcontract-noButton").click(function () {
                    window.refresh().close();
                });
            },

            linkedContractDetailsDataSource: new kendo.data.DataSource({
                schema: {
                  model: {
                      id: 'doctorCorporationContractLinkId'
                  }  
                },
                transport: {
                    read: function(options) {
                        var onSuccessReadLinkedContracts = function(response) {
                            options.success(response);
                        };
                        var onFailLinkedContracts = function(response) {
                            options.error(response);
                        };

                        AjaxCallGet("/api/linkedcontracts/GetLinkedContractByDoctorAndInsurance",
                        { doctorId: doctorId, insuranceName: insuranceName },
                        onSuccessReadLinkedContracts,
                        onFailLinkedContracts);
                    },
                    update: function (options) {
                        
                        var onSuccessUpdate = function (response) {
                            console.log("update linked contract succed: ");
                            options.success(response);
                            toastr.success("Contract was successfully updated.");
                        };
                        var onFailUpdate = function(response) {
                            options.error(response);
                            console.log("update linked contract failed: ", response);
                            toastr.error("We are sorry, but something went wrong. Plase try again.");
                            viewModel.get('linkedContractDetailsDataSource').cancelChanges();
                        };
                        AjaxCallPost("/api/linkedcontracts/",
                            JSON.stringify(options.data),
                            onSuccessUpdate,
                            onFailUpdate);
                    },
                    destroy: function(options) {
                        var removelinkedContractSuccess = function (response) {
                            toastr.success("Contract was successfully released.");
                            options.success(response);
                        };
                        var removelinkedContractFail = function (response) {
                            console.log('Release Linked contract failed:', response);
                            toastr.error(response.statusText);
                            viewModel.get('linkedContractDetailsDataSource').cancelChanges();
                            options.error(response);
                        };

                        AjaxCallDelete("/api/linkedcontracts/",
                            JSON.stringify(options.data),
                            removelinkedContractSuccess,
                            removelinkedContractFail);
                    }
                }
            }),

            onClickLocationHyperlink: function(e) {
                var linkedContractItem = e.data;
                var success = function (response) {
                    linkedContractItem.firstName = response.firstName;
                    linkedContractItem.lastName = response.lastName;
                    linkedContractItem.degree = response.degree;
                    linkedContractItem.dateOfBirth = moment(response.dateOfBirth).format('L');
                    linkedContractItem.socialSecurityNumber = response.socialSecurityNumber;
                    linkedContractItem.npiNumber = response.npiNumber;
                    linkedContractItem.caqhNumber = response.caqhNumber;
                    linkedContractItem.active = response.active === true ? 'YES' : 'NO';

                    //Display a grid with the previous updated list above
                    var providerLocationWindow = $(".js-providerlocation-dialog").kendoWindow({
                        modal: true,
                        width: "90%"
                    }).data("kendoWindow");

                    var kendoTemplate = kendo
                        .template($("#providerbylocations-detail-template")
                        .html());

                    //send the row data object to the template and render it
                    providerLocationWindow.content(kendoTemplate(linkedContractItem)); 
                    providerLocationWindow.center().open();

                    if (doctor.Active)
                        $(".k-button k-button-icontext k-grid-edit").show();
                    else
                        $(".k-button k-button-icontext k-grid-edit").hide();
                };

                var error = function (response) {
                    toastr.error("Something unexpected happened. Please contact your system administrator.");
                    console.log("HTTP Post Fail!:", JSON.stringify(response));
                };

                /*Actualiza la lista de posibles providers by locations*/
                AjaxCallGet("/api/ProviderByLocations/GetActualProviderByLocation",
                {
                    doctorCorporationContractLinkId: linkedContractItem.doctorCorporationContractLinkId,
                    contractLineofBusinessId: linkedContractItem.contractLineofBusinessId,
                    doctorId: linkedContractItem.doctorId
                }, success, error);
            }
        });
        kendo.bind($("#linkedcontractplusdata_" + doctorId + "_" + contractGroupNumber), viewModel);
    };

    return {
        createObservable: createObservable
    };
}();