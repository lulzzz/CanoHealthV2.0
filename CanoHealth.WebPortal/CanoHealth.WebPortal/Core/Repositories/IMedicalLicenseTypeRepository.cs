using CanoHealth.WebPortal.Core.Domain;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IMedicalLicenseTypeRepository : IRepository<MedicalLicenseType>
    {
        MedicalLicenseType GetMedicalLicenseTypeByName(string medicalLicenseTypeName);
        IEnumerable<AuditLog> SaveMadicalLicenseTypes(List<MedicalLicenseType> medicalLicenseTypes);
        IEnumerable<MedicalLicenseType> GetAllOrderByClassification();
    }
}
