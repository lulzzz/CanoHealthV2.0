using CanoHealth.WebPortal.CommonTools.User;
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

        public bool? Active { get; set; }

        //Navegation Properties.
        public ContractLineofBusiness ContractLineofBusiness { get; set; }

        public Doctor Doctor { get; set; }

        public ICollection<ProviderByLocation> ProvidersByLocations { get; set; }

        //Constructor
        public DoctorCorporationContractLink()
        {
            ProvidersByLocations = new Collection<ProviderByLocation>();
        }

        //Behavioral methods
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

        public IEnumerable<AuditLog> InactivateRelationAmongContractLineofBusinessDoctor()
        {
            var auditLogs = new List<AuditLog>();

            Active = false;
            auditLogs.Add(new AuditLog
            {
                TableName = "DoctorCorporationContract",
                ColumnName = "Active",
                AuditAction = "Update",
                OldValue = "true",
                NewValue = "false",
                ObjectId = DoctorCorporationContractLinkId,
                UpdatedOn = DateTime.Now,
                UpdatedBy = new UserService().GetUserName()
            });

            foreach (var provider in ProvidersByLocations)
            {
                var log = provider.Inactivate();
                auditLogs.Add(log);
            }
            return auditLogs;
        }

    }
}