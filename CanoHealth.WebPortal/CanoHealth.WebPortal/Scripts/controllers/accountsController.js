var AccountsController = function (userService) {
    var getUserInfoResponse;

    var init = function (container) {
       
        $(container).on("click", "#js-btn-submit", submitLogginCredentials)
    };

    var submitLogginCredentials = function () {
        var getUserSuccess = function (response) {
            console.log(response);
            //if email is not confirmed means that is the first attempt to login
            if (response && response.active && response.emailConfirmed === false) {
                
                var changePasswordWindow = $("#js-changepassword-window").kendoWindow({
                    title: "Change password",
                    modal: true,
                    width: 600,
                    actions: [],

                }).data('kendoWindow');
                changePasswordWindow.open().center();
                AccountsController.getUserInfoResponse = false;
            } else
                AccountsController.getUserInfoResponse = true;
        };
        var getUserFails = function (response) {
            AccountsController.getUserInfoResponse = true;
        }

        userService.getUserInfo({ username: $("#Email").val() }, getUserSuccess, getUserFails);
        return false;
    };

   

    return {
        init: init
    }
}(UserService);