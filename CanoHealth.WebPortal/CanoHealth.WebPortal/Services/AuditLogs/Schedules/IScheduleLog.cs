using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Services.AuditLogs.Schedules
{
    public interface IScheduleLog
    {
        IEnumerable<AuditLog> GenerateLogs(IEnumerable<Schedule> schedules);
    }
}
