using CanoHealth.WebPortal.Core.Domain;
using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Repositories
{
    public interface IContractRepository : IRepository<Contract>
    {
        IEnumerable<Contract> GetContractWithInsurance();

        Contract GetContractByGroupNumber(string groupNumber);

        Contract GetContractWithBusinessLines(Guid ContractId);

        Contract GetContractByCorporationAndInsurance(Guid corporationId, Guid insuranceId);

        IEnumerable<Contract> GetActiveContractWithInsuranceCorporationInfo();

        /*Obtener aquellos contratos que estan activos y que ademas ya tengan lineas de negocios asociados al mismo*/
        IEnumerable<Contract> GetActiveContractsWithInsuranceCorporationBusinessLines();

        IEnumerable<AuditLog> SaveContracts(IEnumerable<Contract> contracts);
    }
}
