using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IAddendumRepository : IRepository<ContractAddendum>
    {
        IEnumerable<AuditLog> SaveAddendums(IEnumerable<ContractAddendum> addendums);

        IEnumerable<ContractAddendum> GetActiveAddendums(string contractId = null);

        IEnumerable<ContractAddendum> GetContractAddendumsByInsurance(Guid insuranceId);

        IEnumerable<ContractAddendum> GetContractAddendumsByContract(Guid contractId);
    }
}