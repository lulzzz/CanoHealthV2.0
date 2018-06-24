using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Repositories;
using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CanoHealth.WebPortal.Persistance.Repositories
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<Contract> GetContractWithInsurance()
        {
            return EnumarableGetAll(includeProperties: i => i.Insurance).ToList();
        }

        public IEnumerable<Contract> GetActiveContractWithInsuranceCorporationInfo()
        {
            var activeContracts = EnumarableGetAll(
                    filter: c => c.Active,
                    includeProperties: new Expression<Func<Contract, object>>[]
                    {
                        c => c.Corporation,
                        i => i.Insurance
                    })
                .ToList();
            return activeContracts;
        }

        /*Pending: monitor the performance of this query*/
        public IEnumerable<Contract> GetActiveContractsWithInsuranceCorporationBusinessLines()
        {
            var activeContracts = EnumarableGetAll(
                   filter: c => c.Active && c.ContractBusinessLines.Any(),
                   includeProperties: new Expression<Func<Contract, object>>[]
                   {
                        c => c.Corporation,
                        i => i.Insurance,
                        //cbl => cbl.ContractBusinessLines.Select(bl => bl.LineOfBusiness)
                   })
               .ToList();
            return activeContracts;
        }

        public Contract GetContractByGroupNumber(string groupNumber)
        {
            return SingleOrDefault(x => x.GroupNumber.Equals(groupNumber, StringComparison.InvariantCultureIgnoreCase));
        }

        public Contract GetContractWithBusinessLines(Guid ContractId)
        {
            var contract = QueryableGetAll(includeProperties:
                    new Expression<Func<Contract, object>>[] { bl => bl.ContractBusinessLines
                    .Select(l => l.LineOfBusiness) })
                .SingleOrDefault(c => c.ContractId == ContractId);
            return contract;
        }

        public Contract GetContractByCorporationAndInsurance(Guid corporationId,
            Guid insuranceId)
        {
            return QueryableGetAll(c => c.Active, includeProperties:
                    new Expression<Func<Contract, object>>[] { bl => bl.ContractBusinessLines.Select(p => p.LineOfBusiness) })
                .SingleOrDefault(c => c.CorporationId == corporationId && c.InsuranceId == insuranceId);
        }

        public IEnumerable<AuditLog> SaveContracts(IEnumerable<Contract> contracts)
        {
            return SaveItems(contracts,
                (contractCollection, contract) => contractCollection.Any(c => c.ContractId == contract.ContractId));
        }

        private IEnumerable<AuditLog> SaveItems(IEnumerable<Contract> contracts,
            Func<DbSet<Contract>, Contract, bool> exitsContract)
        {
            var auditLogs = new List<AuditLog>();
            foreach (var contract in contracts)
            {
                if (exitsContract(Entities, contract))
                {
                    var contractStoredInDb = Get(contract.ContractId);
                    var logs = contractStoredInDb.ModifyContract(contract);
                    auditLogs.AddRange(logs);
                }
                else
                {
                    Add(contract);
                    auditLogs.AddRange(new List<AuditLog>
                    {
                        AuditLog.AddLog("Contracts", "GroupNumber",null,contract.GroupNumber,contract.ContractId,"Create"),
                        AuditLog.AddLog("Contracts", "CorporationId",null,contract.CorporationId.ToString(),contract.ContractId,"Create"),
                        AuditLog.AddLog("Contracts", "InsuranceId",null,contract.InsuranceId.ToString(),contract.ContractId,"Create"),
                        AuditLog.AddLog("Contracts", "Active",null,contract.Active.ToString(),contract.ContractId,"Create"),
                    });
                }
            }
            return auditLogs;
        }
    }
}