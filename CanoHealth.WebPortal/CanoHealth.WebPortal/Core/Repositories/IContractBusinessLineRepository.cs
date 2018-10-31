using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IContractBusinessLineRepository : IRepository<ContractLineofBusiness>
    {
        IEnumerable<ContractLineofBusiness> GetContractBusinessLinesWithClinics(string contractId);

        ContractLineofBusiness GetContractLineofBusinessAndLocations(Guid contractLineofBusinessId);

        IEnumerable<AuditLog> GetLogsWhileRemoveItems(List<ContractLineofBusiness> contractLineofBusinesses);

        ContractLineofBusiness ExistItem(Guid contractId, Guid planTypeId, Guid? contractLineofBusinessId = null);

        IEnumerable<ContractLineofBusiness> GetContractBusinessLines(Guid contractId);
    }
}