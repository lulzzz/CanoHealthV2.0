using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface ILicenseRepository : IRepository<PosLicense>
    {
        IEnumerable<PosLicense> GetActiveLicenses(string placeOfServiceId = null);

        PosLicense GetByLicenseNumber(string licenseNumber);

        PosLicense GetByTypeAndPlaceOfService(Guid typeId, Guid placeOfServiceId);

        IEnumerable<AuditLog> SaveLocationLicenses(IEnumerable<PosLicense> posLicenses);
    }
}
