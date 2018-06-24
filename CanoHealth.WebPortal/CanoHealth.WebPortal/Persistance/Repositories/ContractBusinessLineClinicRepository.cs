using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class ContractBusinessLineClinicRepository : Repository<ClinicLineofBusinessContract>,
        IContractBusinessLineClinicRepository
    {
        public ContractBusinessLineClinicRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<AuditLog> GetLogsWhileRemoveItems(
            IEnumerable<ClinicLineofBusinessContract> clinicLineofBusiness)
        {
            var auditLogs = new List<AuditLog>();
            var collection = clinicLineofBusiness.Select(x => new List<AuditLog>
            {
                AuditLog.AddLog("ClinicLineofBusinessContract", "ContractLineofBusinessId",
                    x.ContractLineofBusinessId.ToString(), null, x.Id, "Delete"),
                AuditLog.AddLog("ClinicLineofBusinessContract", "PlaceOfServiceId", x.PlaceOfServiceId.ToString(), null,
                    x.Id, "Delete"),
            });

            foreach (var item in collection)
            {
                auditLogs.AddRange(item);
            }

            RemoveRange(clinicLineofBusiness);

            return auditLogs;
        }

        public IEnumerable<PlaceOfService> GetLocationsByBusinessLines(Guid contractLineofBusinessId)
        {
            var result = EnumarableGetAll(x => x.ContractLineofBusinessId == contractLineofBusinessId,
                includeProperties: new Expression<Func<ClinicLineofBusinessContract, object>>[]
                {
                    pos => pos.Clinic
                })
                .Select(x => x.Clinic)
                .Where(l => l.Active).ToList();

            return result;
        }
    }
}
