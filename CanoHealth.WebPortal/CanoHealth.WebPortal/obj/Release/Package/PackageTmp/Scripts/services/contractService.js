var ContractService = function () {
    //private fields

    //private methods
    var getInsuranceWithContracts = function (insuranceName, done, fail) {
        AjaxCallGet("/api/Insurances/", { insuranceName: insuranceName }, done, fail);
    };

    var activateInsurance = function (insurance) {
        AjaxCallPut("/api/Insurances/", JSON.stringify(insurance));
    }

    var createContract = function (contract, done, fail) {
        $.ajax({
            type: "POST",
            url: domainName + '/Contracts/CreateContract/',
            contentType: false,
            processData: false,
            data: contract,
            success: done,
            error: fail
        });
    };

    //accessing to private members
    return {
        getInsuranceWithContracts: getInsuranceWithContracts,
        activateInsurance: activateInsurance,
        createContract: createContract
    }
}();