using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class MedicalLicenseTypeRepository : Repository<MedicalLicenseType>, IMedicalLicenseTypeRepository
    {
        public MedicalLicenseTypeRepository(ApplicationDbContext context) : base(context) { }

        public MedicalLicenseType GetMedicalLicenseTypeByName(string medicalLicenseTypeName)
        {
            return SingleOrDefault(mlt => mlt.Classification.Equals(medicalLicenseTypeName, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<AuditLog> SaveMadicalLicenseTypes(List<MedicalLicenseType> medicalLicenseTypes)
        {
            return SaveItems(medicalLicenseTypes, (collection, item) => collection.Any(x => x.MedicalLicenseTypeId == item.MedicalLicenseTypeId));
        }

        private IEnumerable<AuditLog> SaveItems(List<MedicalLicenseType> medicalLicenseTypes,
            Func<DbSet<MedicalLicenseType>, MedicalLicenseType, bool> existMedicalLicenseType)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var type in medicalLicenseTypes)
            {
                if (existMedicalLicenseType(Entities, type))
                {
                    var medicalLicenseTypeStoredInDb = Get(type.MedicalLicenseTypeId);

                    if (medicalLicenseTypeStoredInDb.Classification != type.Classification)
                    {
                        auditLogs.Add(AuditLog.AddLog("MedicalLicenseTypes", "Classification", medicalLicenseTypeStoredInDb.Classification, type.Classification, type.MedicalLicenseTypeId, "Update"));
                        medicalLicenseTypeStoredInDb.Classification = type.Classification;
                    }
                }
                else
                {
                    Add(type);
                    auditLogs.Add(AuditLog.AddLog("MedicalLicenseTypes", "Classification", null, type.Classification, type.MedicalLicenseTypeId, "Insert"));
                }
            }
            return auditLogs;
        }

        public IEnumerable<MedicalLicenseType> GetAllOrderByClassification()
        {
            return EnumarableGetAll(orderBy: ioq => ioq.OrderBy(mlt => mlt.Classification)).ToList();
        }
    }
}