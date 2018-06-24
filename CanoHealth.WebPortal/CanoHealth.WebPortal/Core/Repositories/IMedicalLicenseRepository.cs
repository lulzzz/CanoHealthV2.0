using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IMedicalLicenseRepository : IRepository<MedicalLicense>
    {
        IEnumerable<MedicalLicense> GetActiveMedicalLicenses(Guid? doctorId);

        MedicalLicense GetLicenseByNumber(string licenseNumber, Guid? medicalLicenseId = null);

        MedicalLicense GetLicenseByDoctorAndType(Guid doctorId, Guid medicalLicenseTypeId, Guid? medicalLicenseId = null);

        IEnumerable<AuditLog> SaveMedicalLicenses(IEnumerable<MedicalLicense> licenseToStore);
    }
}