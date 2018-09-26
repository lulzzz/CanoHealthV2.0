var SchedulersController = function () {

    //Private fields
    var validationMessageTmpl = kendo.template($("#serverSideErrorHandlerTemp").html());

    //Private methods
    var serverSideErrorHandlers = function (args) {
        if (args.errors) {
            var scheduler = $("#DoctorSchedule").data("kendoScheduler");
            scheduler.one("dataBinding", function (e) {
                e.preventDefault();   // cancel grid rebind if error occurs 
                for (var error in args.errors) {
                    console.log("error: ", error);
                    showMessage(scheduler.editable.element, error.toLowerCase(), args.errors[error].errors, scheduler);
                }
            });
        }
    };

    var showMessage = function (container, name, errors, scheduler) {
        //add the validation message to the form
        if (name) {
            console.log($("#" + name).parent());
            $("#" + name).parent().append(validationMessageTmpl({ field: name, message: errors[0] }));

            container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
                .replaceWith(validationMessageTmpl({ field: name, message: errors[0] }));
        } else {
            toastr.error(errors[0]);
            scheduler.cancelEvent();
        }
    };

    //Access to private members
    return {
        serverSideErrorHandlers: serverSideErrorHandlers
    };
}();