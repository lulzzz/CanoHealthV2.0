var SchedulersController = function () {

    //Private fields
    var validationMessageTmpl = kendo.template($("#serverSideErrorHandlerTemp").html());

    //Private methods
    var setSchedulerSettings = function () {
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
                        url: "/Doctors/GetActiveDoctorsAsJson",
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
        setSchedulerSettings: setSchedulerSettings        
    };
}();