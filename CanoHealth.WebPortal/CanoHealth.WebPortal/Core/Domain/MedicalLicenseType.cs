using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class MedicalLicenseType
    {
        public Guid MedicalLicenseTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Classification { get; set; }
    }
}