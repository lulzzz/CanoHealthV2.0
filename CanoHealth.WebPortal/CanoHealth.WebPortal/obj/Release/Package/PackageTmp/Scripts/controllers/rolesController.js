var RolesController = function () {
    //Private fields
    var rolevalidationMessageTmpl = kendo.template($("#serverside-errorhandler-Template")
        .html());

    //Private methods
    var displayErrorMessage = function (container, name, errors) {
        //add the validation message to the form
        if (name) {
            console.log($("#" + name).parent());
            $("#" + name).parent().append(rolevalidationMessageTmpl({ field: name, message: errors[0] }));

            container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
                .replaceWith(rolevalidationMessageTmpl({ field: name, message: errors[0] }));
        } else {
            toastr.error(errors[0]);
            var grid = $("#Roles").data("kendoGrid");
            grid.cancelChanges();
        }
    };

    var roleserverSideErrorHandlers = function (args) {
        if (args.errors) {
            var grid = $("#Roles").data("kendoGrid");
            grid.one("dataBinding", function (e) {
                e.preventDefault();   // cancel grid rebind if error occurs 
                for (var error in args.errors) {
                    displayErrorMessage(grid.editable.element, error, args.errors[error].errors);
                }
            });
        }
    };

    //Access to private members
    return {
        roleserverSideErrorHandlers: roleserverSideErrorHandlers
    };
}();