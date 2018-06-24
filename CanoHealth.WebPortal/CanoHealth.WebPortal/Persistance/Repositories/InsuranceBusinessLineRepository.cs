using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class InsuranceBusinessLineRepository : Repository<InsuranceBusinessLine>, IInsuranceBusinessLineRepository
    {
        public InsuranceBusinessLineRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<InsuranceBusinessLine> GetBusinessLines(Guid insuranceId)
        {
            return EnumarableGetAll(ibl => ibl.InsuranceId == insuranceId, includeProperties: bl => bl.BusinessLine);
        }

        public IEnumerable<AuditLog> Save(IEnumerable<InsuranceBusinessLine> insuranceBusinessLines)
        {
            return SaveItems(insuranceBusinessLines,
                (collection, item) => collection.Any(x => x.InsuranceBusinessLineId == item.InsuranceBusinessLineId));
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<InsuranceBusinessLine> items,
            Func<DbSet<InsuranceBusinessLine>, InsuranceBusinessLine, bool> existItem)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var item in items)
            {
                if (existItem(Entities, item))
                {
                    var itemStoredInDb = Get(item.InsuranceBusinessLineId); //Load the item to the context.
                    if (itemStoredInDb.InsuranceId != item.InsuranceId)
                    {
                        auditLogs.Add(AuditLog.AddLog("InsuranceBusinessLines",
                            "InsuranceId",
                            itemStoredInDb.InsuranceId.ToString(),
                            item.InsuranceId.ToString(),
                            item.InsuranceBusinessLineId,
                            "Update"));
                        itemStoredInDb.InsuranceId = item.InsuranceId;
                    }
                    if (itemStoredInDb.PlanTypeId != item.PlanTypeId)
                    {
                        auditLogs.Add(AuditLog.AddLog("InsuranceBusinessLines",
                            "PlanTypeId",
                            itemStoredInDb.PlanTypeId.ToString(),
                            item.PlanTypeId.ToString(),
                            item.InsuranceBusinessLineId,
                            "Update"));
                        itemStoredInDb.PlanTypeId = item.PlanTypeId;
                    }
                }
                else
                {
                    Add(item);
                    auditLogs.AddRange(new List<AuditLog>
                    {
                        AuditLog.AddLog("InsuranceBusinessLines",
                            "PlanTypeId",
                            null,
                            item.PlanTypeId.ToString(),
                            item.InsuranceBusinessLineId,
                            "Insert"),
                        AuditLog.AddLog("InsuranceBusinessLines",
                            "InsuranceId",
                            null,
                            item.InsuranceId.ToString(),
                            item.InsuranceBusinessLineId,
                            "Insert")
                    });
                }
            }

            return auditLogs;
        }
    }
}