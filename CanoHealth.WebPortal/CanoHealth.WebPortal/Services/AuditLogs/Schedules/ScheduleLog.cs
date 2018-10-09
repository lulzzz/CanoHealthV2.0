using CanoHealth.WebPortal.CommonTools.CurrentDateTime;
using CanoHealth.WebPortal.CommonTools.User;
using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Services.AuditLogs.Schedules
{
    public class ScheduleLog : IScheduleLog
    {
        private readonly IUserService _user;
        private readonly ICurrentDateTimeService _date;

        public ScheduleLog(IUserService user, ICurrentDateTimeService date)
        {
            _user = user;
            _date = date;
        }

        public IEnumerable<AuditLog> GenerateLogs(IEnumerable<Schedule> schedules)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var schedule in schedules)
            {
                auditLogs.AddRange(new List<AuditLog>
                {
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "Title",
                        NewValue = schedule.Title,
                        AuditAction = "Insert",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "StartDateTime",
                        NewValue = schedule.Start.ToString(),
                        AuditAction = "Insert",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "EndDateTime",
                        NewValue = schedule.End.ToString(),
                        AuditAction = "Insert",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "StartTimezone",
                        NewValue = schedule.StartTimezone,
                        AuditAction = "Insert",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "EndTimeZone",
                        NewValue = schedule.EndTimezone,
                        AuditAction = "Insert",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "Description",
                        NewValue = schedule.Description,
                        AuditAction = "Insert",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "IsAllDay",
                        NewValue = schedule.IsAllDay.ToString(),
                        AuditAction = "Insert",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "PlaceOfServiceId",
                        NewValue = schedule.PlaceOfServiceId.ToString(),
                        AuditAction = "Insert",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    }
                });
            }
            return auditLogs;
        }

        public IEnumerable<AuditLog> GenerateLogsWhenDelete(IEnumerable<Schedule> schedules)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var schedule in schedules)
            {
                auditLogs.AddRange(new List<AuditLog>
                {
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "Title",
                        OldValue = schedule.Title,
                        AuditAction = "Delete",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "StartDateTime",
                        OldValue = schedule.Start.ToString(),
                        AuditAction = "Delete",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "EndDateTime",
                        OldValue = schedule.End.ToString(),
                        AuditAction = "Delete",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "StartTimezone",
                        OldValue = schedule.StartTimezone,
                        AuditAction = "Delete",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "EndTimeZone",
                        OldValue = schedule.EndTimezone,
                        AuditAction = "Delete",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "Description",
                        OldValue = schedule.Description,
                        AuditAction = "Delete",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "IsAllDay",
                        OldValue = schedule.IsAllDay.ToString(),
                        AuditAction = "Delete",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "Schedule",
                        ColumnName = "PlaceOfServiceId",
                        OldValue = schedule.PlaceOfServiceId.ToString(),
                        AuditAction = "Delete",
                        ObjectId = schedule.ScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    }
                });
            }
            return auditLogs;
        }
    }
}