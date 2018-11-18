using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class OutOfNetworkContract
    {
        public Guid OutOfNetworkContractId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid InsurnaceId { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool? Active { get; set; }

        public List<AuditLog> Modify(OutOfNetworkContract contract)
        {
            var auditLogs = new List<AuditLog>();
            if (DoctorId != contract.DoctorId)
            {
                auditLogs.Add(AuditLog.AddLog("OutOfNetworkContracts", "DoctorId", DoctorId.ToString(), contract.DoctorId.ToString(), OutOfNetworkContractId, "Update"));
                DoctorId = contract.DoctorId;
            }
            if (InsurnaceId != contract.InsurnaceId)
            {
                auditLogs.Add(AuditLog.AddLog("OutOfNetworkContracts", "InsurnaceId", InsurnaceId.ToString(), contract.InsurnaceId.ToString(), OutOfNetworkContractId, "Update"));
                InsurnaceId = contract.InsurnaceId;
            }
            if (EffectiveDate != contract.EffectiveDate)
            {
                auditLogs.Add(AuditLog.AddLog("OutOfNetworkContracts", "EffectiveDate", EffectiveDate.ToString(), contract.EffectiveDate.ToString(), OutOfNetworkContractId, "Update"));
                EffectiveDate = contract.EffectiveDate;
            }
            if (ExpirationDate != contract.ExpirationDate)
            {
                auditLogs.Add(AuditLog.AddLog("OutOfNetworkContracts", "ExpirationDate", ExpirationDate.ToString(), contract.ExpirationDate.ToString(), OutOfNetworkContractId, "Update"));
                ExpirationDate = contract.ExpirationDate;
            }
            return auditLogs;
        }
    }
}