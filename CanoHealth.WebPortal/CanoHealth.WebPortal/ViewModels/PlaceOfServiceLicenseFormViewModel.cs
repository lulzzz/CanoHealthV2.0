using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Infraestructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels
{
    public class PlaceOfServiceLicenseFormViewModel
    {
        public Guid? PosLicenseId { get; set; }

        [Required]
        public Guid PlaceOfServiceId { get; set; }

        public string LicenseTypeName { get; set; }

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

        public PosLicense Convert()
        {
            var posLicense = new PosLicense
            {
                PosLicenseId = PosLicenseId ?? Guid.NewGuid(),
                PlaceOfServiceId = PlaceOfServiceId,
                LicenseNumber = LicenseNumber,
                EffectiveDate = EffectiveDate,
                ExpireDate = ExpireDate,
                Note = Note,
                ServerLocation = ConfigureSettings.ServerLicFiles,
                OriginalFileName = OriginalFileName,
                FileExtension = FileExtension,
                FileSize = FileSize,
                ContentType = ContentType,
                UploadBy = UploadBy,
                UploaDateTime = UploaDateTime,
                Active = Active
            };
            if (!String.IsNullOrWhiteSpace(UniqueFileName))
                posLicense.UniqueFileName = UniqueFileName;
            else if (!String.IsNullOrWhiteSpace(FileExtension))
                posLicense.UniqueFileName = Guid.NewGuid() + FileExtension;
            return posLicense;
        }
    }
}