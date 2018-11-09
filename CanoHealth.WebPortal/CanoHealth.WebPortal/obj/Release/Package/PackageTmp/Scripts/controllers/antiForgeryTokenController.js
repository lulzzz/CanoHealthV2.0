var AntiForgeryTokenController = function () {

    var sendAntiForgery = function () {
        return { "__RequestVerificationToken": $('input[name=__RequestVerificationToken]').val() }
    };

    return {
        sendAntiForgery: sendAntiForgery
    };

}();