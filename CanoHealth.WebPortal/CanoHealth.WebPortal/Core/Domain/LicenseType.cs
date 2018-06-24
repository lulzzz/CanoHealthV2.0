using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class LicenseType
    {
        public Guid LicenseTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string LicenseName { get; set; }
    }
}