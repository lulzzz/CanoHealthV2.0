using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Infraestructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels
{
    public class MedicalLicenseFormViewModel
    {
        public Guid? MedicalLicenseId { get; set; } // PK

        public Guid DoctorId { get; set; } //FK from doctor table

        public string MedicalLicenseTypeName { get; set; }

        [Required]
        [StringLength(20)]
        public string LicenseNumber { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }

        [StringLength(255)]
        public string Note { get; set; }

        [StringLength(100)]
        public string ServerLocation { get; set; }

        [StringLength(100)]
        public string OriginalFileName { get; set; }

        [StringLength(100)]
        public string UniqueFileName { get; set; }

        [StringLength(10)]
        public string FileExtension { get; set; }

        [StringLength(10)]
        public string FileSize { get; set; }

        [StringLength(255)]
        public string ContentType { get; set; }

        [StringLength(128)]
        public string UploadBy { get; set; }

        public DateTime? UploaDateTime { get; set; }

        public bool? Active { get; set; }

        public MedicalLicenseType ConvertToMedicalLicenseType()
        {
            return new MedicalLicenseType
            {
                MedicalLicenseTypeId = Guid.NewGuid(),
                Classification = MedicalLicenseTypeName
            };
        }

        public MedicalLicense ConvertToMedialLicense()
        {
            return new MedicalLicense
            {
                MedicalLicenseId = MedicalLicenseId ?? Guid.NewGuid(),
                DoctorId = DoctorId,
                LicenseNumber = LicenseNumber,
                EffectiveDate = EffectiveDate,
                ExpireDate = ExpireDate,
                Note = Note,
                ServerLocation = ConfigureSettings.GetMedicalLicensesDirectory,
                OriginalFileName = OriginalFileName,
                UniqueFileName = UniqueFileName ?? Guid.NewGuid() + FileExtension,
                FileExtension = FileExtension,
                FileSize = FileSize,
                ContentType = ContentType,
                Active = Active
            };
        }
    }
}