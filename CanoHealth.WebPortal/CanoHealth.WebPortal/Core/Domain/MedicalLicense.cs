using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class MedicalLicense
    {
        public Guid MedicalLicenseId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid MedicalLicenseTypeId { get; set; }

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

        //Navegation Properties
        public Doctor Doctor { get; set; }

        public MedicalLicenseType LicenseType { get; set; }

        public AuditLog Inactivate()
        {
            var auditlog = AuditLog.AddLog("MedicalLicenses", "Active", Active.ToString(), false.ToString(), MedicalLicenseId, "Update");
            Active = false;
            return auditlog;
        }

        public bool BelongsToDoctor(Guid doctorId)
        {
            return DoctorId == doctorId && Active == false;
        }
    }
}