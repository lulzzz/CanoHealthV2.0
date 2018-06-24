using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class ClinicLineofBusinessContract
    {
        public Guid Id { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public Guid ContractLineofBusinessId { get; set; }

        public PlaceOfService Clinic { get; set; }

        public ContractLineofBusiness ContractLineofBusiness { get; set; }

        public IEnumerable<AuditLog> CreateReleaseLogs()
        {
            var auditLogs = new List<AuditLog>();
            auditLogs.AddRange(new List<AuditLog>
            {
                AuditLog.AddLog("ClinicLineofBusinessContract", "PlaceOfServiceId", PlaceOfServiceId.ToString(), null, Id, "Delete"),
                AuditLog.AddLog("ClinicLineofBusinessContract", "ContractLineofBusinessId", ContractLineofBusinessId.ToString(), null, Id, "Delete")
            });
            return auditLogs;
        }
    }
}