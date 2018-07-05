﻿var UsersController = function (userService) {
    //Private properties
    var user;
    var uservalidationMessageTmpl = kendo.template($("#serverside-errorhandler-Template")
        .html());

    //Private methods
    var init = function (container) {
        $(container).on("change", ".js-email", validateEmail);
    }

    var validateEmail = function () {
        
        var email = $(this).val();
        console.log("email: ", email);
        var emailpattern = /^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?(canohealth)\.com$/g;
        if (!emailpattern.test(email.toString())) {
            toastr.error("invalid email address. It should be part of canohealth domain.");
            $(".k-grid-update").hide();
        } else {
            var checkAvailabilitySuccess = function (response) {
                console.log("user: ", response);
                //Authorization has been denied for this request. Returns 200 with an error message instead a 401 unauthorized
                if (response && response.message) {
                    toastr.error(response.message);
                    $(".k-popup-edit-form").data('kendoWindow').close();
                    $("#Users").data("kendoGrid").dataSource.cancelChanges();
                    return;
                }
                //If a user was found with the same username and the admin is creating a new user show message and hide button
                //if a user is found with the same username and the admin is updating the user and the ids are differents then show message an hide the button.
                if ((response && !user) || (response && user && user.id !== response.id)) {
                    toastr.error("There is a user with the same username. Please try again.");
                    $(".k-grid-update").hide();
                } else {
                    $(".k-grid-update").show();
                }
            };
            var checkAvailabilityFail = function (response) {
                toastr.error("We are sorry, but something went wrong. Please try again.");
                $(".k-grid-update").hide();
            };

            userService.getUserInfo({ username: email }, checkAvailabilitySuccess, checkAvailabilityFail);                       
        }
    };

    var onEditUserItem = function (e) {
        user = e.model;
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

    var onOpenUserFormWindow = function () {
        $(".container-fluid").on("change", ".js-email", validateEmail);        
    };

    return {
        init: init,
        onEditUserItem: onEditUserItem,
        userserverSideErrorHandlers: userserverSideErrorHandlers,
        onOpenUserFormWindow: onOpenUserFormWindow
    }
}(UserService);