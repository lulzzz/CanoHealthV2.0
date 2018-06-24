using CanoHealth.WebPortal.Core.Domain;
using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class MedicalLicenseDto
    {
        public Guid MedicalLicenseId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid MedicalLicenseTypeId { get; set; }

        public string MedicalLicenseType { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime EffectiveDate { get; set; }

        public DateTime ExpireDate { get; set; }

        public string Note { get; set; }

        public string ServerLocation { get; set; }

        public string OriginalFileName { get; set; }

        public string UniqueFileName { get; set; }

        public string FileExtension { get; set; }

        public string FileSize { get; set; }

        public string ContentType { get; set; }

        public string UploadBy { get; set; }

        public DateTime? UploaDateTime { get; set; }

        public bool? Active { get; set; }

        public static MedicalLicenseDto Wrap(MedicalLicense medicalLicense)
        {
            return new MedicalLicenseDto
            {
                MedicalLicenseId = medicalLicense.MedicalLicenseId,
                DoctorId = medicalLicense.DoctorId,
                MedicalLicenseTypeId = medicalLicense.MedicalLicenseTypeId,
                MedicalLicenseType = medicalLicense.LicenseType.Classification,
                LicenseNumber = medicalLicense.LicenseNumber,
                EffectiveDate = medicalLicense.EffectiveDate,
                ExpireDate = medicalLicense.ExpireDate,
                Note = medicalLicense.Note,
                ServerLocation = medicalLicense.ServerLocation,
                OriginalFileName = medicalLicense.OriginalFileName,
                UniqueFileName = medicalLicense.UniqueFileName,
                FileExtension = medicalLicense.FileExtension,
                FileSize = medicalLicense.FileSize,
                ContentType = medicalLicense.ContentType,
                UploadBy = medicalLicense.UploadBy,
                UploaDateTime = medicalLicense.UploaDateTime,
                Active = medicalLicense.Active
            };
        }
    }
}