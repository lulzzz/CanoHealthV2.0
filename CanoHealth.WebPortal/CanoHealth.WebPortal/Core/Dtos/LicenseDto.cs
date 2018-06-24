using CanoHealth.WebPortal.Core.Domain;
using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class LicenseDto
    {
        public Guid PosLicenseId { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public Guid LicenseTypeId { get; set; }

        public string LicenseName { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime EffectiveDate { get; set; }

        public DateTime ExpireDate { get; set; }

        public string Note { get; set; }

        public string ServerLocation { get; set; }

        public string UniqueFileName { get; set; }

        public string OriginalFileName { get; set; }

        public string FileExtension { get; set; }

        public string FileSize { get; set; }

        public string ContentType { get; set; }

        public static LicenseDto Wrap(PosLicense license)
        {
            return new LicenseDto
            {
                PosLicenseId = license.PosLicenseId,
                PlaceOfServiceId = license.PlaceOfServiceId,
                LicenseTypeId = license.LicenseTypeId,
                LicenseName = license.LicenseType.LicenseName,
                LicenseNumber = license.LicenseNumber,
                EffectiveDate = license.EffectiveDate,
                ExpireDate = license.ExpireDate,
                Note = license.Note,
                ServerLocation = license.ServerLocation,
                UniqueFileName = license.UniqueFileName,
                OriginalFileName = license.OriginalFileName,
                FileExtension = license.FileExtension,
                FileSize = license.FileSize,
                ContentType = license.ContentType
            };
        }
    }
}