using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class DoctorCorporationContractLink
    {
        public Guid DoctorCorporationContractLinkId { get; set; }

        public Guid ContractLineofBusinessId { get; set; }

        public Guid DoctorId { get; set; }

        public string Note { get; set; }

        public DateTime? EffectiveDate { get; set; }

        //Navegation Properties.
        public ContractLineofBusiness ContractLineofBusiness { get; set; }

        public Doctor Doctor { get; set; }

        public ICollection<ProviderByLocation> ProvidersByLocations { get; set; }

        public DoctorCorporationContractLink()
        {
            ProvidersByLocations = new Collection<ProviderByLocation>();
        }

        public IEnumerable<AuditLog> Modify(Guid contractLineofBusinessId,
            Guid doctorId, DateTime? effectiveDate, string note)
        {
            var auditLogs = new List<AuditLog>();
            if (contractLineofBusinessId != ContractLineofBusinessId)
            {
                auditLogs.Add(AuditLog.AddLog(
                    "DoctorCorporationContractLinks",
                    "ContractLineofBusinessId",
                    ContractLineofBusinessId.ToString(),
                    contractLineofBusinessId.ToString(),
                    DoctorCorporationContractLinkId,
                    "Update"));
                ContractLineofBusinessId = contractLineofBusinessId;
            }

            if (DoctorId != doctorId)
            {
                auditLogs.Add(AuditLog.AddLog(
                    "DoctorCorporationContractLinks",
                    "DoctorId",
                    DoctorId.ToString(),
                    doctorId.ToString(),
                    DoctorCorporationContractLinkId,
                    "Update"));
                DoctorId = doctorId;
            }

            if (EffectiveDate != effectiveDate)
            {
                auditLogs.Add(AuditLog.AddLog(
                    "DoctorCorporationContractLinks",
                    "EffectiveDate",
                    EffectiveDate.ToString(),
                    effectiveDate.ToString(),
                    DoctorCorporationContractLinkId,
                    "Update"));
                EffectiveDate = effectiveDate;
            }

            if (Note != note)
            {
                auditLogs.Add(AuditLog.AddLog(
                    "DoctorCorporationContractLinks",
                    "Note",
                    Note,
                    note,
                    DoctorCorporationContractLinkId,
                    "Update"));
                Note = note;
            }
            return auditLogs;
        }
    }
}