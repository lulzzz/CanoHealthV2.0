using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class PlaceOfService
    {
        public Guid PlaceOfServiceId { get; set; }

        public Guid CorporationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [Phone]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Phone]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string FaxNumber { get; set; }

        public bool Active { get; set; }

        public Corporation Corporation { get; set; }

        public ICollection<PosLicense> PosLicenses { get; set; }

        public ICollection<ClinicLineofBusinessContract> ClinicLineofBusiness { get; set; }

        public ICollection<DoctorClinic> Doctors { get; set; }

        public PlaceOfService()
        {
            PosLicenses = new Collection<PosLicense>();
            ClinicLineofBusiness = new Collection<ClinicLineofBusinessContract>();
            Doctors = new Collection<DoctorClinic>();
        }

        public IEnumerable<AuditLog> Modify(Guid corporationId, string name, string address, string phoneNumber, string faxNumber,
            bool active)
        {
            var auditLogs = new List<AuditLog>();
            if (CorporationId != corporationId)
            {
                var log = AuditLog.AddLog("PlaceOfServices", "CorporationId", CorporationId.ToString(),
                    corporationId.ToString(), PlaceOfServiceId, "Update");
                CorporationId = corporationId;
                auditLogs.Add(log);
            }

            if (Name != name)
            {
                var log = AuditLog.AddLog("PlaceOfServices", "Name", Name, name, PlaceOfServiceId, "Update");
                Name = name;
                auditLogs.Add(log);
            }
            if (Address != address)
            {
                var log = AuditLog.AddLog("PlaceOfServices", "Address", Address, address, PlaceOfServiceId, "Update");
                Address = address;
                auditLogs.Add(log);
            }
            if (PhoneNumber != phoneNumber)
            {
                var log = AuditLog.AddLog("PlaceOfServices", "PhoneNumber", PhoneNumber, phoneNumber, PlaceOfServiceId, "Update");
                auditLogs.Add(log);
                PhoneNumber = phoneNumber;
            }
            if (FaxNumber != faxNumber)
            {
                var log = AuditLog.AddLog("PlaceOfServices", "FaxNumber", FaxNumber, faxNumber, PlaceOfServiceId, "Update");
                auditLogs.Add(log);
                FaxNumber = faxNumber;
            }
            if (Active != active)
            {
                var log = AuditLog.AddLog("PlaceOfServices", "Active", Active.ToString(), active.ToString(), PlaceOfServiceId, "Update");
                auditLogs.Add(log);
                Active = active;
            }
            return auditLogs;
        }
    }
}