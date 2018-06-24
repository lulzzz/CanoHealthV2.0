using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class LicenseTypeRepository : Repository<LicenseType>, ILicenseTypeRepository
    {
        public LicenseTypeRepository(ApplicationDbContext context) : base(context) { }

        public Guid GetLicenseTypeId(string licenseTypeName)
        {
            var licenseType = SingleOrDefault(
                        t => t.LicenseName.Equals(licenseTypeName, StringComparison.CurrentCultureIgnoreCase));

            if (licenseType == null)
            {
                var newLicenseType = new LicenseType
                {
                    LicenseTypeId = Guid.NewGuid(),
                    LicenseName = licenseTypeName
                };
                Add(newLicenseType);
                return newLicenseType.LicenseTypeId;
            }
            return licenseType.LicenseTypeId;
        }

        public LicenseType GetLicenseTypeByName(string licenseTypeName)
        {
            return SingleOrDefault(t => t.LicenseName.Equals(licenseTypeName, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerable<AuditLog> SaveLicenseTypes(IEnumerable<LicenseType> licenseTypes)
        {
            var auditLogs = SaveItems(licenseTypes,
                (collectionLicenseTypes, licenseType) =>
                    collectionLicenseTypes.Any(lt => lt.LicenseTypeId == licenseType.LicenseTypeId));

            return auditLogs;
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<LicenseType> licenseTypes, Func<DbSet<LicenseType>, LicenseType, bool> existLicenseType)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var licenseType in licenseTypes)
            {
                if (existLicenseType(Entities, licenseType))
                {
                    var licenseTypeStoredInDb = Get(licenseType.LicenseTypeId);
                    if (licenseTypeStoredInDb.LicenseName != licenseType.LicenseName)
                    {
                        auditLogs.Add(AuditLog.AddLog(
                            "LicenseTypes",
                            "LicenseName",
                            licenseTypeStoredInDb.LicenseName,
                            licenseType.LicenseName,
                            licenseType.LicenseTypeId,
                            "Update"));
                        licenseTypeStoredInDb.LicenseName = licenseType.LicenseName;
                    }
                }
                else
                {
                    Add(licenseType);
                    auditLogs.Add(AuditLog.AddLog(
                            "LicenseTypes",
                            "LicenseName",
                            null,
                            licenseType.LicenseName,
                            licenseType.LicenseTypeId,
                            "Insert"));
                }
            }

            return auditLogs;
        }
    }
}