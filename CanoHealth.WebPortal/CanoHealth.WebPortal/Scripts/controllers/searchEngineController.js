var SearchEngine = function() {
    //private fields
    var doctor,isTheDoctorSelected;

    //private methods
    var getLocationInfoSuccess = function (response) {
        console.log("location info success: ");

        if (response.length === 0) {
            var nodatatemplate = kendo.template($("#no-data-found").html());

            //Show the notification section
            $("#js-notification-window").show();

            //Set the content of the notification section
            $("#js-notification-window").html(nodatatemplate);

            //Hide the search result section
            $("#js-grid-result").hide();
            return;
        }
        //Show the searh result section
        $("#js-grid-result").show();

        //Clean the notification window
        $("#js-notification-window").html('');

        //Hide the notification window
        $("#js-notification-window").hide();

        //Get an instance of the grid that shows the result search.
        var grid = $("#js-grid-result").data('kendoGrid');
        //If the grid does not exist we have to create it
        if (!grid) {
            $("#js-grid-result").kendoGrid({
                dataSource: {
                    data: response,
                    schema: {
                        model: {
                            fields: {
                                placeOfServiceId: { type: "string" },
                                location: { type: "string" },
                                address: { type: "string" },
                                phoneNumber: { type: "string" },
                                faxNumber: { type: "string" },
                                providerNumberByInsurance: "providerNumberByInsurance",
                                lineOfBusiness: "lineOfBusiness"
                            }
                        }
                    },
                    pageSize: 20
                },
                scrollable: true,
                sortable: true,
                filterable: true,
                pageable: {
                    refresh: true,
                    previousNext: true,
                    pageSizes: [25, 50, 100]
                },
                detailTemplate: kendo.template($("#lineofbusiness-detail-template").html()),
                columns: [
                    { field: "location", title: "LOCATION", width: "210px" },
                    { field: "address", title: "ADDRESS", width: "210px" },
                    { field: "phoneNumber", title: "PHONE NUMBER", width: "210px" },
                    { field: "faxNumber", title: "FAX NUMBER", width: "210px" },
                    { field: "providerNumberByInsurance", title: "Provider by Insurance", width: "210px" }
                ]
            });
        } else {
            //If the grid is already created, we just need to update the datasource and the columns.
            var newDataSource = new kendo.data.DataSource({
                data: response,
                schema: {
                    model: {
                        fields: {
                            placeOfServiceId: { type: "string" },
                            location: { type: "string" },
                            address: { type: "string" },
                            phoneNumber: { type: "string" },
                            faxNumber: { type: "string" },
                            lineOfBusiness: "lineOfBusiness"
                        }
                    }
                },
                pageSize: 20
            });

            grid.setOptions({
                columns: [
                    { field: "location", title: "LOCATION", width: "210px" },
                    { field: "address", title: "ADDRESS", width: "210px" },
                    { field: "phoneNumber", title: "PHONE NUMBER", width: "210px" },
                    { field: "faxNumber", title: "FAX NUMBER", width: "210px" },
                    { field: "providerNumberByInsurance", title: "Provider by Insurance", width: "210px" }
                ],
                detailTemplate: kendo.template($("#lineofbusiness-detail-template").html()),
            });

            grid.setDataSource(newDataSource);
        }

    };

    var getServerInfoFails = function (response) {
        console.log("location info fails: ", response);
        var errorMessage;
        switch (response.status) {
            case 404: //Not Found
                errorMessage = response.responseJSON + " " + "Please try again.";
                break;
            default:
                errorMessage = "We are sorry, but something went wrong. Please try again.";
                break;
        }
        toastr.error(errorMessage);
    };

    var checkRequiredFields = function() {
        var isValid = true,
            corporationMessage = '',
            insuranceMessage = '';
        var corporation = $("#corporations").data("kendoDropDownList"),
            insurance = $("#insurances").data("kendoDropDownList");

        if (!corporation.value()) {
            corporationMessage = "<li>The corporation field is required.</li>";
            isValid = false;
        }
        if (!insurance.value()) {
            insuranceMessage = "<li>The insurance field is required.</li>";
            isValid = false;
        }   
        if (!isValid)
            toastr.error(`<ul>${corporationMessage}${insuranceMessage}</ul>`);
        return isValid;
    };

    var performSearch = function () {
        if (checkRequiredFields()) {
            var corporationDrp = $("#corporations").data("kendoDropDownList"),
                insuranceDrp = $("#insurances").data("kendoDropDownList"),
                locationDrp = $("#locations").data("kendoDropDownList"),
                doctorAuto = $("#doctors").data("kendoAutoComplete");            

            //When Corporation -> Insurance are set
            if (!locationDrp.value() && !doctorAuto.value()) {
                console.log("When Corporation -> Insurance are set");
                var paramsToServer = {
                    corporationId: corporationDrp.value(),
                    insuranceId: insuranceDrp.value()                   
                };
                var getLineOfBusinessInfoSuccess = function (response) {
                    if (response.length === 0) {
                        var nodatatemplate = kendo.template($("#no-data-found").html());

                        //Show the notification section
                        $("#js-notification-window").show();

                        //Set the content of the notification section
                        $("#js-notification-window").html(nodatatemplate);

                        //Hide the search result section
                        $("#js-grid-result").hide();
                        return;
                    }

                    //Show the searh result section
                    $("#js-grid-result").show();

                    //Clean the notification window
                    $("#js-notification-window").html('');

                    //Hide the notification window
                    $("#js-notification-window").hide();

                    var grid = $("#js-grid-result").data('kendoGrid');
                    if (!grid) {

                        $("#js-grid-result").kendoGrid({
                            dataSource: {
                                data: response,
                                schema: {
                                    model: {
                                        id: "contractLineofBusinessId",
                                        fields: {
                                            contractLineofBusinessId: { type: "string" },
                                            groupNumber: { type: "string" },
                                            code: { type: "string" },
                                            name: { type: "string" },
                                            insuranceId: "insuranceId"
                                        }
                                    }
                                },
                                pageSize: 20
                            },
                            scrollable: true,
                            sortable: true,
                            filterable: true,
                            pageable: {
                                refresh: true,
                                previousNext: true,
                                pageSizes: [25, 50, 100]
                            },
                            detailTemplate: kendo.template($("#locations-detail-template").html()),
                            columns: [
                                { field: "groupNumber", title: "CONTRACT", width: "210px" },
                                { field: "code", title: "CODE", width: "210px" },                               
                                { field: "name", title: "LINE OF BUSINESS", width: "210px" }                               
                            ]
                        });

                    } else {
                        //If the grid is already created, we just need to update the datasource and the columns.
                        var newDataSource = new kendo.data.DataSource({
                            data: response,
                            schema: {
                                model: {
                                    id: "contractLineofBusinessId",
                                    fields: {
                                        contractLineofBusinessId: { type: "string" },
                                        groupNumber: { type: "string" },
                                        code: { type: "string" },
                                        name: { type: "string" },
                                        insuranceId: "insuranceId"
                                    }
                                }
                            },
                            pageSize: 20
                        });
                        grid.setOptions({
                            columns: [
                                { field: "groupNumber", title: "CONTRACT", width: "210px" },
                                { field: "code", title: "CODE", width: "210px" },
                                { field: "name", title: "LINE OF BUSINESS", width: "210px" }
                            ],
                            detailTemplate: kendo.template($("#locations-detail-template").html()),
                        });
                        grid.setDataSource(newDataSource);
                    }
                };
               
                AjaxCallGet("/api/searchengine/GetResultByCorporationAndInsurance", paramsToServer, getLineOfBusinessInfoSuccess, getServerInfoFails);
            }                

            //When Corporation -> Insurance -> Location -> Doctor are set
            if (locationDrp.value() && doctorAuto.value()) {
                console.log("When Corporation -> Insurance -> Location -> Doctor are set");
                var paramsToServerii = {
                    corporationId: corporationDrp.value(),
                    insuranceId: insuranceDrp.value(),
                    locationId: locationDrp.value(),
                    doctorId: doctor.DoctorId
                };
                AjaxCallGet("/api/searchengine/GetSearchResultByInsuranceLocationAndDoctorSql", paramsToServerii, getLocationInfoSuccess, getServerInfoFails);
            }               

            //When Corporation -> Insurance -> Location are set
            if (locationDrp.value() && !doctorAuto.value()) {
                console.log("When Corporation -> Insurance -> Location are set");
                var paramsToServeriii = {
                    corporationId: corporationDrp.value(),
                    insuranceId: insuranceDrp.value(),
                    locationId: locationDrp.value()
                };
                var getDoctorInfoSuccess = function (response) {
                    console.log("doctor info success: ");
                    if (response.length === 0) {
                        var nodatatemplate = kendo.template($("#no-data-found").html());  

                        //Show the notification section
                        $("#js-notification-window").show();

                        //Set the content of the notification section
                        $("#js-notification-window").html(nodatatemplate);

                        //Hide the search result section
                        $("#js-grid-result").hide();
                        return;
                    }

                    
                    //Show the searh result section
                    $("#js-grid-result").show();

                    //Clean the notification window
                    $("#js-notification-window").html('');

                    //Hide the notification window
                    $("#js-notification-window").hide();

                    //Get an instance of the grid that shows the result search.
                    var grid = $("#js-grid-result").data('kendoGrid');

                    //If the grid does not exist we have to create it
                    if (!grid) {                       
                        $("#js-grid-result").kendoGrid({
                            dataSource: {
                                data: response,
                                schema: {
                                    model: {
                                        fields: {
                                            degree: { type: "string" },
                                            fullName: { type: "string" },
                                            dateOfBirth: { type: "date" },
                                            npiNumber: { type: "string" },
                                            caqhNumber: { type: "string" },
                                            providerNumberByInsurance: "providerNumberByInsurance",
                                            lineOfBusiness: "lineOfBusiness"
                                        }
                                    }
                                },
                                pageSize: 20
                            },
                            scrollable: true,
                            sortable: true,
                            filterable: true,
                            pageable: {
                                refresh: true,
                                previousNext: true,
                                pageSizes: [25, 50, 100]
                            },
                            detailTemplate: kendo.template($("#lineofbusiness-detail-template").html()),
                            columns: [
                                { field: "degree", title: "DEGREE", width: "210px" },
                                { field: "fullName", title: "NAME", width: "210px" },
                                { field: "dateOfBirth", title: "DOB", format: "{0:MM/dd/yyyy}", width: "210px" },
                                { field: "npiNumber", title: "NPI NUMBER", width: "210px" },
                                { field: "caqhNumber", title: "CAQH NUMBER", width: "210px" },
                                { field: "providerNumberByInsurance", title: "Provider By Insurance", width: "210px", headerAttributes: { "title": "Provider Number By Insurance" } }
                            ]
                        });
                    } else {
                        //If the grid is already created, we just need to update the datasource and the columns.
                        var newDataSource = new kendo.data.DataSource({
                            data: response,
                            schema: {
                                model: {
                                    fields: {
                                        degree: { type: "string" },
                                        fullName: { type: "string" },
                                        dateOfBirth: { type: "date" },
                                        npiNumber: { type: "string" },
                                        caqhNumber: { type: "string" },
                                        providerNumberByInsurance: "providerNumberByInsurance",
                                        lineOfBusiness: "lineOfBusiness"
                                    }
                                }
                            },
                            pageSize: 20
                        });
                        grid.setOptions({
                            columns: [
                                { field: "degree", title: "DEGREE", width: "210px" },
                                { field: "fullName", title: "NAME", width: "210px" },
                                { field: "dateOfBirth", title: "DOB", format: "{0:MM/dd/yyyy}", width: "210px" },
                                { field: "npiNumber", title: "NPI NUMBER", width: "210px" },
                                { field: "caqhNumber", title: "CAQH NUMBER", width: "210px" },
                                { field: "providerNumberByInsurance", title: "Provider By Insurance", width: "210px" }
                            ],
                            detailTemplate: kendo.template($("#lineofbusiness-detail-template").html()),
                        });
                        grid.setDataSource(newDataSource);
                    }                    
                }
               
                AjaxCallGet("/api/searchengine/GetSearchResultByInsuranceLocation", paramsToServeriii, getDoctorInfoSuccess, getServerInfoFails);
            }                

            //When Corporation -> Insurance -> Doctor are set
            if (!locationDrp.value() && doctorAuto.value()) {
                console.log("When Corporation -> Insurance -> Doctor are set");
                var paramsToServeriv = {
                    corporationId: corporationDrp.value(),
                    insuranceId: insuranceDrp.value(),
                    doctorId: doctor.DoctorId
                };
               
                AjaxCallGet("/api/searchengine/GetSearchByInsuranceDoctor", paramsToServeriv, getLocationInfoSuccess, getServerInfoFails);
            }                
        }
    };

    var init = function(container) {
        $(container).on("click", ".js-searchengine-button", performSearch);
        $(container).on("click", ".js-refresh-button", resetSearchParamenters);             
    };

    var showDoctorSchedule = function (e) {
        var doctorId = e;

        var scheduler = $("#js-doctor-calendar-scheduler").kendoScheduler({
            date: new Date(),
            views: [
                { type: "day" },
                { type: "month", selected: true },
                { type: "agenda", selectedDateFormat: "{0:ddd, M/dd/yyyy} - {1:ddd, M/dd/yyyy}" }
            ],
            timezone: "Etc/UTC",
            dataSource: {
                //type: 'aspnetmvc-ajax',
                transport: {
                    read: {
                        url: domainName + "/Schedulers/ReadScheduleJson",
                        dataType: "json",
                        data: { doctorId: doctorId }
                    }
                    //read: function (options) {
                    //    $.ajax({
                    //        url: domainName + "/Schedulers/ReadScheduleJson",
                    //        dataType: "json",
                    //        data: { doctorId: doctorId },
                    //        success: function (response) {
                    //            console.log("success response from read scheduler:", response);
                    //            options.success(response);
                    //        },
                    //        error: function (response) {
                    //            console.log("fail response from read scheduler:", response);
                    //            options.error(response);
                    //        }
                    //    });                        
                    //}
                }
            },
            schema:{
                model: {
                    id: "ScheduleId",
                    fields: {                       
                        ScheduleId: { field: "ScheduleId" },
                        //title: { field: "Title" },
                        Start: { type: "date", field: "Start" },
                        End: { type: "date", field: "End" },
                        ////startTimezone: { field: "StartTimezone" },
                        ////endTimezone: { field: "EndTimezone" },
                        //description: { field: "Description" },
                        //recurrenceId: { from: "RecurrenceID" },
                        //recurrenceRule: { from: "RecurrenceRule" },
                        //recurrenceException: { from: "RecurrenceException" },
                        locationId: { field: "LocationId"},
                        //isAllDay: { type: "boolean", field: "IsAllDay" },
                        doctors: { field: "Doctors" },
                        timezone: { field: "Timezone" }
                    }
                }
            },
            resources: [
                {
                    field: "locationId",
                    dataTextField: "Name",
                    dataValueField: "PlaceOfServiceId",
                    title: "Location",
                    dataSource: {
                        transport: {
                            read: {
                                url: domainName + "/PlaceOfService/GetLocations",
                                dataType: "json"
                            }                            
                        }
                    }
                },
                {
                    field: "doctors",
                    multiple: true,
                    title: "Doctors",
                    dataValueField: "DoctorId",
                    dataTextField: "FullName",
                    dataSource: {
                        transport: {
                            read: {
                                url: domainName + "/ClinicDoctorTeams/GetDoctorsByLocation", 
                                dataType: "json"
                            }
                        }
                    }
                }
            ]
        }).data("kendoScheduler");       
    };

    //Reset the search params.
    var resetSearchParamenters = function () {
        var corporationDrp = $("#corporations").data("kendoDropDownList"),
            insuranceDrp = $("#insurances").data("kendoDropDownList"),
            locationDrp = $("#locations").data("kendoDropDownList"),
            doctorAuto = $("#doctors").data("kendoAutoComplete");

        corporationDrp.value('');
        insuranceDrp.value('');
        locationDrp.value('');
        doctorAuto.value('');
        doctor = null;
        $("#js-grid-result").html('');
        $("#js-grid-result").hide();
    };

    var filterCorporations = function() {
        return {
            corporationId: $("#corporations").val()
        };
    };

    var onOpenDoctorAutocomplete = function(e) {
        isTheDoctorSelected = false;
    };

    var onSelectDoctorAutocomplete = function(e) {
        e.preventDefault();
        doctor = e.dataItem;
        isTheDoctorSelected = true;
        $("#doctors").data("kendoAutoComplete").value(doctor.FullName);
    };

    var onCloseDoctorAutocomplete = function() {
        if (!isTheDoctorSelected) {
            doctor = null;
            $("#doctors").data("kendoAutoComplete").value('');
            toastr.error("Please select a doctor from the list.");
        }
    };

    var onChangeDoctorAutocomplete = function(e) {
        if(e.sender.value().trim().length === 0)
            doctor = null;
    };

    var onBoundDoctorToListView = function (e) {
        
        var listViewId = e.sender.element[0].id,
            listOfDoctorFound = e.sender.dataSource.data();
        if (e.sender.dataSource.data().length === 0) {
            //custom logic
            var noDoctorFoundTemplate = kendo.template($("#no-doctor-found").html());
            $("#" + listViewId).append(noDoctorFoundTemplate);
        }
    };

    var onErrorHandlerSearchDoctor = function (e) {             
        toastr.error("We are sorry, but something went wrong. Please try again!");
    }; 

    //access to private members
    return {
        init: init,
        filterCorporations: filterCorporations,
        onOpenDoctorAutocomplete: onOpenDoctorAutocomplete,
        onSelectDoctorAutocomplete: onSelectDoctorAutocomplete,
        onCloseDoctorAutocomplete: onCloseDoctorAutocomplete,
        onChangeDoctorAutocomplete: onChangeDoctorAutocomplete,
        onBoundDoctorToListView: onBoundDoctorToListView,
        showDoctorSchedule: showDoctorSchedule,
        onErrorHandlerSearchDoctor: onErrorHandlerSearchDoctor
    };
}();