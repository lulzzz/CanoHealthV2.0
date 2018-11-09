var PlaceOfServiceManager = function() {
    var getPlaceOfServices = function(placeOfServiceId, getPlaceOfServicesSuccess, getPlaceOfServiceFails) {
        AjaxCallGet("/api/PlaceOfServices", { placeOfServiceId: placeOfServiceId }, getPlaceOfServicesSuccess, getPlaceOfServiceFails);
    }

    var readPlaceOfServiceLicenses = function(placeOfServiceId, done, fail) {
        $.ajax({
            method: 'GET',
            url: '/api/PosLicense/',
            data: { placeOfServiceId: placeOfServiceId },
            success: done,
            error: fail
        });
    };

    var createPlaceOfServiceLicenses = function(license, done, fail) {
        $.ajax({
            type: "POST",
            url: domainName + '/PosLicense/CreateLicense/',
            contentType: false,
            processData: false,
            data: license,
            success: done,
            error: fail
        });
    }

    var saveLicenses = function (license, done, fail) {
        $.ajax({
            type: "POST",
            url: domainName + '/PosLicense/UpdateLicense/',
            contentType: false,
            processData: false,
            data: license
        }).done(done).fail(fail);
    };

    var inactivatePlaceOfServiceLicenses = function(license, done, fail) {
        $.ajax({
            method: "DELETE",
            url: '/api/PosLicense/',
            data: license,
            success: done,
            error: fail
        });
    };

    return {
        getPlaceOfServices: getPlaceOfServices,
        readPlaceOfServiceLicenses: readPlaceOfServiceLicenses,
        createPlaceOfServiceLicenses: createPlaceOfServiceLicenses,
        saveLicenses: saveLicenses,
        inactivatePlaceOfServiceLicenses: inactivatePlaceOfServiceLicenses
    }
}();