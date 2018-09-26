using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Services.AuditLogs.DoctorSchedules
{
    public interface IDoctorScheduleLog
    {
        IEnumerable<AuditLog> GenerateLogs(IEnumerable<DoctorSchedule> doctorSchedules);
    }
}
