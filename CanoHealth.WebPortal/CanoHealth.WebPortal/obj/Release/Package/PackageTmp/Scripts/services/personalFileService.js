var PersonalFileService = function () {
    //Private fields

    //Private methods
    var getDoctorPersonalFiles = function (doctorId, success, fail) {
        AjaxCallGet("/api/PersonalFiles", { doctorId: doctorId }, success, fail);
    };

    var createPersonalFile = function (formData, success, fails) {
        $.ajax({
            type: "POST",
            url: domainName + '/PersonalFiles/SavePersonalFile/',
            contentType: false,
            processData: false,
            data: formData,
            success: success,
            error: fails
        });
    };

    var updatePersonalFile = function (formData, success, fails) {
        $.ajax({
            type: "POST",
            url: domainName + '/PersonalFiles/UpdatePersonalFile/',
            contentType: false,
            processData: false,
            data: formData,
            success: success,
            error: fails
        });
    };

    //Access to private members
    return {
        getDoctorPersonalFiles: getDoctorPersonalFiles,
        createPersonalFile: createPersonalFile,
        updatePersonalFile: updatePersonalFile
    }
}();