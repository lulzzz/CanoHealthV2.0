using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Services.AuditLogs.DoctorProviderByLocation
{
    public interface IProviderByLocationLog
    {
        IEnumerable<AuditLog> GenerateLogsWhenDelete(IEnumerable<ProviderByLocation> providerByLocations);
    }
}
