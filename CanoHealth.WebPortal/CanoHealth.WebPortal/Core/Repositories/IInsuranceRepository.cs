using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IInsuranceRepository : IRepository<Insurance>
    {
        IEnumerable<Insurance> GetActiveInsurances();
        IEnumerable<Insurance> GetInsurancesByNames(IEnumerable<string> names);
        Insurance GetByName(string name);
        Insurance GetWithContracts(Guid insurancesId);
        Insurance GetOtherInsuranceWithSameName(string insuranceName, Guid insuranceId);
        IEnumerable<AuditLog> SaveItems(IEnumerable<Insurance> insurances);
        Insurance GetInsuranceById(Guid insuranceId);
    }
}