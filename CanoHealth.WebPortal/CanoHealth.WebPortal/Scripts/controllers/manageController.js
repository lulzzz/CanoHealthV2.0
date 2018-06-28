var ManageController = function () {
    //private fields

    //private methods
    var init = function (container) {
        $(container).on("click", "#js-btn-changeusernamepassword", activateUsernamePasswordForm);
        $(container).on("click", "#js-btn-cancelChangePassword", activateUsernamePasswordDetail);
        $(container).on("click", ".js-changepassword-submit", validateInputFields)

        $("#NewPassword").popover({
            html: true,
            content: '<ul><li>At least 6 characters</li><li>Include all of these elements:<ol><li>One UPPER CASE letter</li><li>One lower case letter</li><li>One special character</li><li>One number</li></ol></li></ul>',
            toggle: "popover",
            title: '<strong>Secure Password</strong>',
            container: "body",
            trigger: "focus",
            tabindex: "0",
            role: "password-input",
            placement: "top"
        });
    };

    var activateUsernamePasswordForm = function () {
        $(".js-changeusernamepassword-form").show();
        $(".js-usernamepassword-details").hide();
    }

    var activateUsernamePasswordDetail = function () {
        $(".js-changeusernamepassword-form").hide();
        $(".js-usernamepassword-details").show();
    };

    var validateInputFields = function () {
        if (checkRequires()) {
            return checkNewAndConfirmPassword();
        } else
            return false;        
    };

    var checkRequires = function () {
        var isValid = true,
            oldPassword = "",
            newPassWord = "",
            confirmPassword = "";

        if (!$("#OldPassword").val()) {
            isValid = false;
            oldPassword = "<li>The Old Password field is required.</li>";
        }
        if (!$("#NewPassword").val()) {
            isValid = false;
            newPassWord = "<li>The New Password field is required.</li>";
        }
        if (!$("#ConfirmPassword").val()) {
            isValid = false;
            confirmPassword = "<li>The Confirm Password field is required.</li>";
        }
        if (!isValid)
            toastr.error(`<ul>${oldPassword}${newPassWord}${confirmPassword}</ul>`);

        return isValid;
    };

    var checkNewAndConfirmPassword = function () {
        if ($("#ConfirmPassword").val() === $("#NewPassword").val())
            return true;
        else {
            toastr.error(`New password and confirm password does not match. Please try again.`);
            return false;
        }
    };

    //access to private members
    return {
        init: init
    }
}();