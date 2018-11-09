var CorporationsController = function () {

    //Private fields
    var validationMessageTmpl = kendo.template($("#serverSideErrorHandlerTemp").html());

    //Private methods
    var serverSideErrorHandlers = function (args) {
        if (args.errors) {
            var grid = $("#Corporations").data("kendoGrid");
            grid.one("dataBinding", function (e) {
                e.preventDefault();   // cancel grid rebind if error occurs 
                for (var error in args.errors) {
                    showMessage(grid.editable.element, error, args.errors[error].errors, grid);
                }
            });
        }
    };

    var showMessage = function (container, name, errors, grid) {
        //add the validation message to the form
        if (name) {
            console.log($("#" + name).parent());
            $("#" + name).parent().append(validationMessageTmpl({ field: name, message: errors[0] }));

            container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
                .replaceWith(validationMessageTmpl({ field: name, message: errors[0] }));
        } else {
            toastr.error(errors[0]);
            grid.cancelChanges();
        }
    };

    //Access to private members
    return {
        serverSideErrorHandlers: serverSideErrorHandlers
    };
}();