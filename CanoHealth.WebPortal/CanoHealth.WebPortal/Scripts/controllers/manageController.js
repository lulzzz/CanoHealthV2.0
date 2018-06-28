var ManageController = function () {
    //private fields

    //private methods
    var init = function (container) {
        $(container).on("click", "#js-btn-changeusernamepassword", activateUsernamePasswordForm);
        $(container).on("click", "#js-btn-cancelChangePassword", activateUsernamePasswordDetail);
    };

    var activateUsernamePasswordForm = function () {
        $(".js-changeusernamepassword-form").show();
        $(".js-usernamepassword-details").hide();
    }

    var activateUsernamePasswordDetail = function () {
        $(".js-changeusernamepassword-form").hide();
        $(".js-usernamepassword-details").show();
    };

    //access to private members
    return {
        init: init
    }
}();