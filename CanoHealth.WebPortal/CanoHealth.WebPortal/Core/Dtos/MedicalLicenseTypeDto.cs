using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class MedicalLicenseTypeDto
    {
        public Guid MedicalLicenseTypeId { get; set; }

        public string Classification { get; set; }
    }
}