using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class PosLicense
    {
        public Guid PosLicenseId { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public Guid LicenseTypeId { get; set; }

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

        public PlaceOfService PlaceOfService { get; set; }

        public LicenseType LicenseType { get; set; }

        public IEnumerable<AuditLog> Modify(PosLicense licenseToStore)
        {
            var auditLogs = new List<AuditLog>();
            if (LicenseTypeId != licenseToStore.LicenseTypeId)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "LicenseTypeId", LicenseTypeId.ToString(),
                    licenseToStore.LicenseTypeId.ToString(), PosLicenseId, "Update"));

                LicenseTypeId = licenseToStore.LicenseTypeId;
            }
            if (LicenseNumber != licenseToStore.LicenseNumber)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "LicenseNumber", LicenseNumber, licenseToStore.LicenseNumber,
                    PosLicenseId, "Update"));
                LicenseNumber = licenseToStore.LicenseNumber;
            }
            if (EffectiveDate != licenseToStore.EffectiveDate)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "EffectiveDate", EffectiveDate.ToString(), licenseToStore.EffectiveDate.ToString(),
                    PosLicenseId, "Update"));
                EffectiveDate = licenseToStore.EffectiveDate;
            }
            if (ExpireDate != licenseToStore.ExpireDate)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "ExpireDate", ExpireDate.ToString(), licenseToStore.ExpireDate.ToString(),
                    PosLicenseId, "Update"));
                ExpireDate = licenseToStore.ExpireDate;
            }
            if (Note != licenseToStore.Note)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "Note", Note, licenseToStore.Note,
                    PosLicenseId, "Update"));
                Note = licenseToStore.Note;
            }
            if (OriginalFileName != licenseToStore.OriginalFileName)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "OriginalFileName", OriginalFileName, licenseToStore.OriginalFileName,
                    PosLicenseId, "Update"));
                OriginalFileName = licenseToStore.OriginalFileName;
            }
            if (UniqueFileName != licenseToStore.UniqueFileName)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "UniqueFileName", UniqueFileName, licenseToStore.UniqueFileName,
                    PosLicenseId, "Update"));
                UniqueFileName = licenseToStore.UniqueFileName;
            }
            if (FileExtension != licenseToStore.FileExtension)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "FileExtension", FileExtension, licenseToStore.FileExtension,
                    PosLicenseId, "Update"));
                FileExtension = licenseToStore.FileExtension;
            }
            if (FileSize != licenseToStore.FileSize)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "FileSize", FileSize, licenseToStore.FileSize,
                    PosLicenseId, "Update"));
                FileSize = licenseToStore.FileSize;
            }
            if (ContentType != licenseToStore.ContentType)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "ContentType", ContentType, licenseToStore.ContentType,
                    PosLicenseId, "Update"));
                ContentType = licenseToStore.ContentType;
            }
            if (UploadBy != licenseToStore.UploadBy)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "UploadBy", UploadBy, licenseToStore.UploadBy,
                    PosLicenseId, "Update"));
                UploadBy = licenseToStore.UploadBy;
            }
            if (UploaDateTime != licenseToStore.UploaDateTime)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "UploaDateTime", UploaDateTime.ToString(), licenseToStore.UploaDateTime.ToString(),
                    PosLicenseId, "Update"));
                UploaDateTime = licenseToStore.UploaDateTime;
            }
            if (Active != licenseToStore.Active)
            {
                auditLogs.Add(AuditLog.AddLog("PosLicenses", "Active", Active.ToString(), licenseToStore.Active.ToString(),
                    PosLicenseId, "Update"));
                Active = licenseToStore.Active;
            }
            return auditLogs;
        }

        public AuditLog InactiveLicense()
        {
            var auditlog = AuditLog.AddLog("PosLicense", "Active", Active.ToString(), false.ToString(), PosLicenseId, "Update");
            Active = false;
            return auditlog;
        }
    }
}