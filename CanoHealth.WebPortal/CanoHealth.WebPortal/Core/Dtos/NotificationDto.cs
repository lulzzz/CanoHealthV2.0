using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class NotificationDto
    {
        public string Source { get; set; }

        public DateTime ExpirationDate { get; set; }

        public Guid? DoctorId { get; set; }

        public string DoctorFullName { get; set; }

        public Guid? InsurnaceId { get; set; }

        public string InsuranceName { get; set; }

        public string LicenseType { get; set; }

        public string Location { get; set; }
    }
}