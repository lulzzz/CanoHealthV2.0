var SchedulersController = function(scheduleService) {

    //Private fields
    var validationMessageTmpl = kendo.template($("#serverSideErrorHandlerTemp").html());

    //Private methods
    var init = function (container) {
        //set the tooltip component when user focus on schedule. Reference: https://docs.telerik.com/kendo-ui/controls/scheduling/scheduler/how-to/appearance/show-tooltip-with-additional-information-over-events
        $("#DoctorSchedule").kendoTooltip({
            filter: ".k-event:not(.k-event-drag-hint) > div, .k-task",
            position: "right",
            width: 250,
            content: kendo.template($('#scheduler-tooltip-template').html()),
            show: function (e) {
                e.sender.popup.element.addClass('grey-tooltip');
            }
        });

        //handle double click event on schedule
        $(container).on("dblclick", ".k-event", displayScheduleDetails);
    };

    var displayScheduleDetails = function (e) {
        var scheduler = $("#DoctorSchedule").getKendoScheduler();
        var schedule = scheduler.occurrenceByUid($(this).data("uid"));
        scheduleService.getSchedule(schedule.ScheduleId, getScheduleSuccess, getScheduleFails);       
    };

    var getScheduleSuccess = function (response) {
        console.log("Get Schedule success: ", response);
        //set the window content 
        var detailsTemplate = kendo.template($("#schedule-detail-template").html());
        //get the window instance
        var wnd = $("#Details").data("kendoWindow");
        //set windows options
        wnd.setOptions({ width: "20%" });
        //pass the schedule to the window content
        var docs = response.doctors.map(function (doc) {
            return doc.fullName;
        });
        var schedule = {
            scheduleId: response.scheduleId,
            title: response.title,
            start: response.start,
            end: response.end,
            placeOfServiceId: response.placeOfServiceId,
            location: response.location,
            doctors: docs.join( )
        };
        wnd.content(detailsTemplate(schedule));
        //open the window
        wnd.center().open();
    };

    var getScheduleFails = function (response) {
        console.log("Get Schedule fail: ", response);        
        if (response.status === 401)//Not found
            toastr.error(response.statusText);
        else //Anything else
            toastr.error('We are sorry, but something went wrong. Please try again.');
    };  

    var addDropdownToSchedulerToolbar = function () {
        var scheduler = $("#DoctorSchedule").data("kendoScheduler");

        var toolbar = $(scheduler.toolbar);

        var label = $("<label class='control-label col-xs-offset-1 col-sm-offset-1 col-md-offset-8' for='doctor'>Show schedule by doctor:</label>");

        var dropDown = $("<input id='js-activedoctors'/>");          

        toolbar.append(label);
        toolbar.append(dropDown);        

        $("#js-activedoctors").kendoDropDownList({
            dataTextField: "FullName",
            dataValueField: "DoctorId",
            optionLabel: "All",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: "/Doctors/GetActiveDoctorsAsJson"
                    }
                }
            },
            change: function (e) {
                var value = this.value();                
                scheduler.dataSource.read();
            }
        });  
    };

    var serverSideErrorHandlers = function (args) {
        if (args.errors) {
            var scheduler = $("#DoctorSchedule").data("kendoScheduler");
            scheduler.one("dataBinding", function (e) {
                e.preventDefault();   // cancel grid rebind if error occurs 
                for (var error in args.errors) {
                    console.log("error: ", error);
                    showMessage(scheduler.editable.element, error, args.errors[error].errors, scheduler);
                }
            });
        }
    };

    var showMessage = function (container, name, errors, scheduler) {
        //add the validation message to the form
        if (name && name !== "CancelChanges") {
            console.log($("#" + name).parent());
            $("#" + name).parent().append(validationMessageTmpl({ field: name, message: errors[0] }));

            container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
                .replaceWith(validationMessageTmpl({ field: name, message: errors[0] }));
        } else if (name && name === "CancelChanges") {
            toastr.error(errors[0]);
            scheduler.cancelEvent();
        }
        else
            toastr.error(errors[0]);        
    };

    //onChange event handler for Location DropdownList
    var onChangeLocation = function (e) {
        onReadDoctorMultiselectDataSource();
    };

    //onDataBound event handler for Location DropDownList
    var onDataBoundLocation = function (e) {       
        var locationId = e.sender.value();
        if (locationId)
            onReadDoctorMultiselectDataSource();
    };

    var onReadDoctorMultiselectDataSource = function () {
        var multiselect = $("#Doctors").data("kendoMultiSelect");
        multiselect.dataSource.read();
    };

    //pass the selected locationId to datasource read method for Doctor multiselect component
    var filterLocations = function() {
        return {
            locationId: $("#LocationId").val()
        };
    };

    //pass the selected doctorId to datasource read method for scheduler component
    var filterDoctor = function () {
        return {
            doctorId: $("#js-activedoctors").val()
        };
    };    

    //Access to private members
    return {
        serverSideErrorHandlers: serverSideErrorHandlers,
        onChangeLocation: onChangeLocation,
        filterLocations: filterLocations,
        filterDoctor: filterDoctor,
        onDataBoundLocation: onDataBoundLocation,
        addDropdownToSchedulerToolbar: addDropdownToSchedulerToolbar,
        init: init
    };
}(ScheduleService);