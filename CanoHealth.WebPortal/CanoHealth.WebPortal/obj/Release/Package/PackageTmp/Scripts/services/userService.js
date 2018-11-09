var UserService = function () {

    var getUserInfo = function (obj, success, fail) {
        AjaxCallGet("/api/users", obj, success, fail);
    };

    return {
        getUserInfo: getUserInfo
    }
}();