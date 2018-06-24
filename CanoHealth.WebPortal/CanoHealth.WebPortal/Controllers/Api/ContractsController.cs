using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class ContractsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpDelete]
        public IHttpActionResult InactivateContract(ContractDto contract)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //Como no voy a inactivar los addendums del contrato ni las lineas de negocio del mismo uso el metodo Get,
                //en caso contrario tendria que usar un metodo que me permita obtener el contrato con los addendums y las lineas de negocio incluidas
                var contractStoredInDb = _unitOfWork.Contracts.Get(contract.ContractId);

                if (contractStoredInDb == null)
                    return NotFound();

                var log = contractStoredInDb.InactivateContract();

                _unitOfWork.AuditLogs.Add(log);

                _unitOfWork.Complete();

                contract.Active = false;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(contract);
        }

        [HttpPost]
        public IHttpActionResult UpdateContract(ContractDto contract)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var contractStoredInDb = _unitOfWork.Contracts.Get(contract.ContractId);

                if (contractStoredInDb == null)
                    return NotFound();

                var updateLogs = contractStoredInDb.ModifyContract(contract.Convert());

                _unitOfWork.AuditLogs.AddRange(updateLogs);

                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(contract);
        }

        [HttpGet]
        public IHttpActionResult GetActiveContractWithInsuranceCorporationInfo(Guid? doctorId = null)
        {
            /*Dame de los contratos que estan activos solamente aquellos que ya tienen lineas de negocios asignadas al mismo*/
            var activeContracts = _unitOfWork.Contracts
                .GetActiveContractsWithInsuranceCorporationBusinessLines()
                .Select(ContractExtDto.Wrap)
                .ToList();

            /*Chequear si dan la informacion del doctor si este tiene algun out of network contract, en caso de tenerlo voy a mostrar de los contratos activos encontrados posteriormente
             * aquellos que no incluyan los seguros de los out of network contracts para dicho doctor.*/
            if (doctorId != null)
            {
                var outOfNetWorkContracts = _unitOfWork.OutofNetworkContractRepository
                    .GetActiveOutOfNetworkContractsByDoctor(doctorId.Value)
                    .Select(i => i.InsurnaceId)
                    .ToList();
                activeContracts = activeContracts.Where(i => !outOfNetWorkContracts.Contains(i.InsuranceId)).ToList();
            }
            return Ok(activeContracts);
            /*Si este querie afectara el rendimiento de la aplicacion seria recomendable
             usar explicity load en lugar de eager load*/
        }
    }
}
