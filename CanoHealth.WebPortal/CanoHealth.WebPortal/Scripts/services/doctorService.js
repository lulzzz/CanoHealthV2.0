var DoctorService = function() {
    //Private fields

    //Private methods
    var getMedicalLicenses = function(doctorId, successCall, errorCall) {
        AjaxCallGet("/api/medicallicenses/", { doctorId: doctorId }, successCall, errorCall);
    }
    var createMedicalLicense = function(formData, successCall, failCall) {
        $.ajax({
            type: "POST",
            url: domainName + '/MedicalLicenses/CreateMedicalLicense/',
            contentType: false,
            processData: false,
            data: formData,
            success: successCall,
            error: failCall
        });
    }

    var updateMedicalLicense = function(formData, success, fail) {
        $.ajax({
            type: "POST",
            url: domainName + '/MedicalLicenses/UpdateMedicalLicense/',
            contentType: false,
            processData: false,
            data: formData,
            success: success,
            error: fail
        });
    };

    var inactivateLicense = function(medicalLicense, inactivateLicenseSuccess, inactivateLicenseFail) {
        AjaxCallDelete("/api/medicallicenses", JSON.stringify(medicalLicense), inactivateLicenseSuccess, inactivateLicenseFail);
    }

    var getProviderInfo = function(url, success, fail) {
        $.ajax({
            method: 'GET',
            url: url,
            //contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: success,
            error: fail
        });
    }

    //Access to private members
    return {
        getMedicalLicenses: getMedicalLicenses,
        createMedicalLicense: createMedicalLicense,
        updateMedicalLicense: updateMedicalLicense,
        inactivateLicense: inactivateLicense,
        getProviderInfo: getProviderInfo
    }
}();