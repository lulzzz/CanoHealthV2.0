using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IInsuranceBusinessLineRepository : IRepository<InsuranceBusinessLine>
    {
        IEnumerable<InsuranceBusinessLine> GetBusinessLines(Guid insuranceId);
        IEnumerable<AuditLog> Save(IEnumerable<InsuranceBusinessLine> insuranceBusinessLines);
    }
}