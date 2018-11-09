var InsuranceService = function() {
    //private fields

    //private methods
    var saveInsuranceBusinessLines = function (obj, success, fail) {
        AjaxCallPost("/api/InsuranceBusinessLines", JSON.stringify(obj), success, fail);
    };

    var availableBusinessLinesForTheInsurance = function (obj, success, fail) {
        AjaxCallGet(domainName + "/BusinessLines/GetBusinessLinesJson/", { insuranceId: obj }, success, fail);
    };

    //access to private members
    return {
        saveInsuranceBusinessLines: saveInsuranceBusinessLines,
        availableBusinessLinesForTheInsurance: availableBusinessLinesForTheInsurance
    };
}();