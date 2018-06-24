using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class OutofNetworkContractRepository : Repository<OutOfNetworkContract>, IOutofNetworkContractRepository
    {
        public OutofNetworkContractRepository(ApplicationDbContext context) : base(context) { }

        public OutOfNetworkContract GetOutOfNetworkContractByDoctorAndInsurnace(Guid doctorId, Guid insuranceId)
        {
            return FirstOrDefault(oo => oo.DoctorId == doctorId && oo.InsurnaceId == insuranceId && oo.ExpirationDate == null);
        }

        public IEnumerable<OutOfNetworkContract> GetActiveOutOfNetworkContractsByDoctor(Guid doctorId)
        {
            return EnumarableGetAll(oo => oo.DoctorId == doctorId && oo.ExpirationDate == null).ToList();
        }

        public IEnumerable<AuditLog> SaveContracts(IEnumerable<OutOfNetworkContract> contracts)
        {
            return SaveItems(contracts, (collection, item) => collection.Any(c => c.OutOfNetworkContractId == item.OutOfNetworkContractId));
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<OutOfNetworkContract> contracts, Func<DbSet<OutOfNetworkContract>, OutOfNetworkContract, bool> existContract)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var contract in contracts)
            {
                if (existContract(Entities, contract))
                {
                    var contractStoredInDb = Get(contract.OutOfNetworkContractId);
                    List<AuditLog> updateLogs = contractStoredInDb.Modify(contract);
                    auditLogs.AddRange(updateLogs);
                }
                else
                {
                    Add(contract);
                    auditLogs.Add(AuditLog.AddLog("OutOfNetworkContracts", "DoctorId", null, contract.DoctorId.ToString(), contract.OutOfNetworkContractId, "Insert"));
                    auditLogs.Add(AuditLog.AddLog("OutOfNetworkContracts", "InsurnaceId", null, contract.InsurnaceId.ToString(), contract.OutOfNetworkContractId, "Update"));
                    auditLogs.Add(AuditLog.AddLog("OutOfNetworkContracts", "EffectiveDate", null, contract.EffectiveDate.ToString(), contract.OutOfNetworkContractId, "Update"));
                    auditLogs.Add(AuditLog.AddLog("OutOfNetworkContracts", "ExpirationDate", null, contract.ExpirationDate.ToString(), contract.OutOfNetworkContractId, "Update"));
                }
            }
            return auditLogs;
        }
    }

}