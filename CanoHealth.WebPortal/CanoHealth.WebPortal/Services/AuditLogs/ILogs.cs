using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Services.AuditLogs
{
    public interface ILogs<TEntity> where TEntity : class
    {
        IEnumerable<AuditLog> GenerateLogs(IEnumerable<TEntity> entities);
    }
}
