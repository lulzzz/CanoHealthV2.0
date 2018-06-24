using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface ILicenseTypeRepository : IRepository<LicenseType>
    {
        Guid GetLicenseTypeId(string licenseTypeName);

        LicenseType GetLicenseTypeByName(string licenseTypeName);

        IEnumerable<AuditLog> SaveLicenseTypes(IEnumerable<LicenseType> licenseTypes);
    }
}
