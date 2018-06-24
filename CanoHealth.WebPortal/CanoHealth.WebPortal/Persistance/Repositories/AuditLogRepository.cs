using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class AuditLogRepository : Repository<AuditLog>, IAuditLogReporitory
    {
        public AuditLogRepository(ApplicationDbContext context) : base(context) { }
    }
}