using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class ProviderByLocation
    {
        public Guid ProviderByLocationId { get; set; } //PK

        //FK from DoctorCorporationContractLink
        public Guid DoctorCorporationContractLinkId { get; set; }

        //It's going to be the glue with Locations. It is not a FK
        public Guid PlaceOfServiceId { get; set; }

        public DateTime? ProviderEffectiveDate { get; set; }

        public string LocacionProviderNumber { get; set; }

        public bool Active { get; set; }

        //Navegation Properties
        public DoctorCorporationContractLink DoctorLinkedContract { get; set; }

        public IEnumerable<AuditLog> Modify(ProviderByLocation provider)
        {

            var auditLogs = new List<AuditLog>();
            if (DoctorCorporationContractLinkId != provider.DoctorCorporationContractLinkId)
            {
                auditLogs.Add(
                    AuditLog.AddLog("ProviderByLocations",
                        "DoctorCorporationContractLinkId",
                        DoctorCorporationContractLinkId.ToString(),
                        provider.DoctorCorporationContractLinkId.ToString(),
                        ProviderByLocationId,
                        "Update")
                    );
                DoctorCorporationContractLinkId = provider.DoctorCorporationContractLinkId;
            }

            if (PlaceOfServiceId != provider.PlaceOfServiceId)
            {
                auditLogs.Add(
                    AuditLog.AddLog("ProviderByLocations",
                        "PlaceOfServiceId",
                        PlaceOfServiceId.ToString(),
                        provider.PlaceOfServiceId.ToString(),
                        ProviderByLocationId,
                        "Update")
                    );
                PlaceOfServiceId = provider.PlaceOfServiceId;
            }

            if (ProviderEffectiveDate != provider.ProviderEffectiveDate)
            {
                auditLogs.Add(
                    AuditLog.AddLog("ProviderByLocations",
                        "ProviderEffectiveDate",
                        ProviderEffectiveDate.ToString(),
                        provider.ProviderEffectiveDate.ToString(),
                        ProviderByLocationId,
                        "Update")
                    );
                ProviderEffectiveDate = provider.ProviderEffectiveDate;
            }

            if (LocacionProviderNumber != provider.LocacionProviderNumber)
            {
                auditLogs.Add(
                    AuditLog.AddLog("ProviderByLocations",
                        "LocacionProviderNumber",
                        LocacionProviderNumber,
                        provider.LocacionProviderNumber,
                        ProviderByLocationId,
                        "Update")
                    );
                LocacionProviderNumber = provider.LocacionProviderNumber;
            }

            if (Active != provider.Active)
            {
                auditLogs.Add(
                    AuditLog.AddLog("ProviderByLocations",
                        "Active",
                        Active.ToString(),
                        provider.Active.ToString(),
                        ProviderByLocationId,
                        "Update")
                    );
                Active = provider.Active;
            }

            return auditLogs;
        }

        public AuditLog Inactivate()
        {
            var log = AuditLog.AddLog("ProviderByLocations",
                "Active",
                Active.ToString(),
                false.ToString(),
                ProviderByLocationId,
                "Update");
            Active = false;
            return log;
        }

        public AuditLog Activate()
        {
            var log = AuditLog.AddLog("ProviderByLocations",
                "Active",
                Active.ToString(),
                true.ToString(),
                ProviderByLocationId,
                "Update");
            Active = true;
            return log;
        }
    }
}