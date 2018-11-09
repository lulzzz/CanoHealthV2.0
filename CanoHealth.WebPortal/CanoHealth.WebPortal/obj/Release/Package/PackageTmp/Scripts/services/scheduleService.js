var ScheduleService = function () {
    var getSchedule = function (scheduleId, success, fail) {
        AjaxCallGet("/api/Schedules/", { scheduleId: scheduleId }, success, fail);
    };

    return {
        getSchedule: getSchedule
    };
}();