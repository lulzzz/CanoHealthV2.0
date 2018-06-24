using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class Contract
    {
        public Guid ContractId { get; set; }

        [Required]
        [StringLength(20)]
        public string GroupNumber { get; set; }

        public Guid CorporationId { get; set; }

        public Guid InsuranceId { get; set; }

        public bool Active { get; set; }

        //Navegation Properties.
        public Corporation Corporation { get; set; }

        public Insurance Insurance { get; set; }

        public ICollection<ContractLineofBusiness> ContractBusinessLines { get; set; }

        public ICollection<ContractAddendum> Addendums { get; set; }

        public Contract()
        {
            ContractBusinessLines = new Collection<ContractLineofBusiness>();
            Addendums = new Collection<ContractAddendum>();
        }

        public AuditLog InactivateContract()
        {
            var log = AuditLog.AddLog("Contract", "Active", true.ToString(), false.ToString(), ContractId, "Update");
            Active = false;
            return log;
        }

        public IEnumerable<AuditLog> ModifyContract(Contract contract)
        {
            var auditLogs = new List<AuditLog>();
            if (GroupNumber != contract.GroupNumber)
            {
                auditLogs.Add(AuditLog.AddLog("Contracts", "GroupNumber", GroupNumber, contract.GroupNumber, ContractId, "Update"));
                GroupNumber = contract.GroupNumber;
            }
            if (CorporationId != contract.CorporationId)
            {
                auditLogs.Add(AuditLog.AddLog("Contracts", "CorporationId", CorporationId.ToString(), contract.CorporationId.ToString(), ContractId, "Update"));
                CorporationId = contract.CorporationId;
            }
            if (InsuranceId != contract.InsuranceId)
            {
                auditLogs.Add(AuditLog.AddLog("Contracts", "InsuranceId", InsuranceId.ToString(), contract.InsuranceId.ToString(), ContractId, "Update"));
                InsuranceId = contract.InsuranceId;
            }
            if (Active != contract.Active)
            {
                if (Active)
                    auditLogs.Add(InactivateContract());
                else
                {
                    auditLogs.Add(AuditLog.AddLog("Contracts", "Active", Active.ToString(), contract.Active.ToString(), ContractId, "Update"));
                    Active = contract.Active;
                }
            }
            return auditLogs;
        }
    }
}