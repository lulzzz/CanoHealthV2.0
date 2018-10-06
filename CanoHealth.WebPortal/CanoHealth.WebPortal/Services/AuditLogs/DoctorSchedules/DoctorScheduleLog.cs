using CanoHealth.WebPortal.CommonTools.CurrentDateTime;
using CanoHealth.WebPortal.CommonTools.User;
using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Services.AuditLogs.DoctorSchedules
{
    public class DoctorScheduleLog : IDoctorScheduleLog
    {
        private readonly IUserService _user;
        private readonly ICurrentDateTimeService _date;

        public DoctorScheduleLog(IUserService user, ICurrentDateTimeService date)
        {
            _user = user;
            _date = date;
        }

        public IEnumerable<AuditLog> GenerateLogs(IEnumerable<DoctorSchedule> doctorSchedules)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var doctorSchedule in doctorSchedules)
            {
                auditLogs.AddRange(new List<AuditLog>
                {
                    new AuditLog
                    {
                        TableName = "DoctorSchedule",
                        ColumnName = "ScheduleId",
                        NewValue = doctorSchedule.ScheduleId.ToString(),
                        AuditAction = "Insert",
                        ObjectId = doctorSchedule.DoctorScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "DoctorSchedule",
                        ColumnName = "DoctorId",
                        NewValue = doctorSchedule.DoctorId.ToString(),
                        AuditAction = "Insert",
                        ObjectId = doctorSchedule.DoctorScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    }
                });
            }
            return auditLogs;
        }

        public IEnumerable<AuditLog> GenerateLogsWhenDelete(IEnumerable<DoctorSchedule> doctorSchedules)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var doctorSchedule in doctorSchedules)
            {
                auditLogs.AddRange(new List<AuditLog>
                {
                    new AuditLog
                    {
                        TableName = "DoctorSchedule",
                        ColumnName = "ScheduleId",
                        OldValue = doctorSchedule.ScheduleId.ToString(),
                        AuditAction = "Delete",
                        ObjectId = doctorSchedule.DoctorScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    },
                    new AuditLog
                    {
                        TableName = "DoctorSchedule",
                        ColumnName = "DoctorId",
                        OldValue = doctorSchedule.DoctorId.ToString(),
                        AuditAction = "Delete",
                        ObjectId = doctorSchedule.DoctorScheduleId,
                        UpdatedBy = _user.GetUserName(),
                        UpdatedOn = _date.GetCurrentDateTime()
                    }
                });
            }
            return auditLogs;
        }
    }
}