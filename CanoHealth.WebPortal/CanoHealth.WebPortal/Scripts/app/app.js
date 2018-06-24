var domainName = "http://" + window.location.host.toString();

function AjaxCallGet(url, json, onSuccess, onError) {
    $.ajax({
        type: "GET", url: url, data: json, contentType: "application/json; charset=utf-8", dataType: "json",
        success: function (result) {
            if (onSuccess != undefined) onSuccess(result);
            else console.log("HTTP Get Success!");
        },
        error: function (result) {
            if (onError != undefined) onError(result);
            else {
                toastr.error("Something unexpected happened. Please contact your system administrator.");
                console.log("HTTP Get Fail!:", JSON.stringify(result));
            }
        }
    });
}

function AjaxCallPost(url, json, onSuccess, onError) {
    $.ajax({
        type: "POST",
        url: url,
        data: json,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(result) {
            if (onSuccess != undefined) onSuccess(result);
            else console.log("HTTP Post Success!");
        },
        error: function(result) {
            if (onError != undefined) onError(result);
            else {
                toastr.error("Something unexpected happened. Please contact your system administrator.");
                console.log("HTTP Post Fail!:", JSON.stringify(result));
            }
        }
    });
}

function AjaxCallPut(url, json, onSuccess, onError) {
    $.ajax({
        type: "PUT",
        url: url,
        data: json,
        contentType: "application/json; charset=utf-8", dataType: "json",
        success: function (result) {
            if (onSuccess != undefined) onSuccess(result);
            else console.log("HTTP Put Success!");
        },
        error: function (result) {
            if (onError != undefined) onError(result);
            else {
                toastr.error("Something unexpected happened. Please contact your system administrator.");
                console.log("HTTP Put Fail!:", JSON.stringify(result));
            }
        }
    });
}

function AjaxCallDelete(url, json, onSuccess, onError) {
    $.ajax({
        type: "DELETE",
        url: url,
        data: json,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (onSuccess != undefined) onSuccess(result);
            else console.log("HTTP Delete Success!");
        },
        error: function (result) {
            if (onError != undefined) onError(result);
            else {
                toastr.error("Something unexpected happened. Please contact your system administrator.");
                console.log("HTTP Delete Fail!:", JSON.stringify(result));
            }
        }
    });
    
}

function isValidDate(date) {
    return !(date === undefined || date === null || date === "" || date === '');
}

function isEmptyValue(value) {
    return value === "" || value === undefined || value === null || value === '' || value.trim().length === 0;
}

function isValidEmail(email) {
    var regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return regex.test(email);
}

function isValidUSZip(sZip) {
    return /^\d{5}(-\d{4})?$/.test(sZip);
}

function isValidFile(ext, size) {
    var validExtensions = [".gif", ".jpg", ".jpeg", ".png", ".img", ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".zip", ".rar", ".rtf"], //Array of String
        maxFileSize = 50000000;

    return validExtensions.filter(function (extension) {
        return extension === ext.toLowerCase();
    }).length > 0 && size <= maxFileSize;
}

var formDataSupport = function() {
    if (window.FormData == undefined) {
        toastr.error("This browser doesn't support HTML5 file uploads! Please upgrade it.");
        return false;
    } else return true;
};

var processFormsFiles = function (kendoUploadComponent) {
    var clinicalForms = kendoUploadComponent.getFiles();

    var goodFiles = clinicalForms.filter(function (file) {
        return isValidFile(file.extension, file.size);
    });

    kendoUploadComponent.removeFile(function (file) {
        return !isValidFile(file[0].extension, file[0].size);
    });

    return goodFiles;
}

var processArrayOfFiles = function (files) {
    //var files = kendoUploadComponent.getFiles();

    var goodFiles = files.filter(function (file) {
        return isValidFile(file.extension, file.size);
    });

    var badFiles = files.filter(function (file) {
        return !isValidFile(file.extension, file.size);
    });

    if (badFiles.length > 0)
        toastr.error("The file you are trying to upload is not valid!");

    return goodFiles;
}

var generateGuid = function () {
    var id = '', i, random;
    for (i = 0; i < 32; i++) {
        random = Math.random() * 16 | 0;
        if (i == 8 || i == 12 || i == 16 || i == 20) {
            id += '-';
        }
        id += (i == 12 ? 4 : i == 16 ? random & 3 | 8 : random).toString(16);
    }
    return id;
}