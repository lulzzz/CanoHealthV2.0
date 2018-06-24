var UsersController = function () {
    //Private properties
    var uservalidationMessageTmpl = kendo.template($("#serverside-errorhandler-Template")
        .html());

    //Private methods
    var init = function (container) {
        $(container).on("click", ".js-email", validateEmail);
    }

    var validateEmail = function () {
        alert("email is changing.");
        //var email = $(this).val();
        //var emailpattern = new RegExp("@@canohealth.com");
        //if (!emailpattern.test(email.toString())) {
        //    alert("invalid email address. It should be part of canohealth domain.");
        //    return;
        //} else {
            
        //}
    };

    var onEditUserItem = function(e) {
        if (e.model.isNew()) {
            e.model.Active = true;
            $("#js-password").show();
            $("#js-confirmpassword").show();
            $("#js-active").hide();
        } else {
            console.log(e.model);
            e.model.dirty = true;
            $("#js-password").hide();
            $("#js-confirmpassword").hide();
            $("#js-active").show();
        }
    };

    var displayErrorMessage = function (container, name, errors) {
        //add the validation message to the form
        if (name) {
            console.log($("#" + name).parent());
            $("#" + name).parent().append(uservalidationMessageTmpl({ field: name, message: errors[0] }));

            container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
                     .replaceWith(uservalidationMessageTmpl({ field: name, message: errors[0] }));
        } else {
            toastr.error(errors[0]);
            var grid = $("#Users").data("kendoGrid");
            grid.cancelChanges();
        }
    }

    var userserverSideErrorHandlers = function(args) {
        if (args.errors) {
            var grid = $("#Users").data("kendoGrid");
            grid.one("dataBinding", function (e) {
                e.preventDefault();   // cancel grid rebind if error occurs 
                for (var error in args.errors) {
                    displayErrorMessage(grid.editable.element, error, args.errors[error].errors);
                }
            });
        }
    };

    return {
        init: init,
        onEditUserItem: onEditUserItem,
        userserverSideErrorHandlers: userserverSideErrorHandlers
    }
}();