using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Collections.Generic;
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
                {
                    return BadRequest(ModelState);
                }

                var contractStoredInDb = _unitOfWork.Contracts.Get(contract.ContractId);

                if (contractStoredInDb == null)
                {
                    return NotFound();
                }
                var auditLogs = new List<AuditLog>();

                //get active doctor provider by location and contract(DoctorProviderByLocation)
                var providerByLocations = _unitOfWork.ProviderByLocationRepository
                    .ProviderByLocationsAndContract(contract.ContractId)
                    .ToList();
                var providerByLocationLogs = providerByLocations.ConvertAll(x => x.Inactivate()).ToList();
                auditLogs.AddRange(providerByLocationLogs);

                //get active doctor corporation contract links by contract([dbo].[DoctorCorporationContractLinks])
                var docLinkedContracts = _unitOfWork.DoctorLinkedContracts
                    .DoctorCorporationContractLinksByContract(contract.ContractId)
                    .ToList();
                var docLinkedContractLogs = docLinkedContracts.ConvertAll(x => x.InactiveDoctorCorporationContractLinkRecord()).ToList();
                auditLogs.AddRange(docLinkedContractLogs);

                //get active contract -> line of business -> locations(ClinicLineofBusinessContracts)
                var locLinkedContracts = _unitOfWork.ContracBusinessLineClinicRepository
                    .GetContractLineofBusinessLocationsByContract(contract.ContractId)
                    .ToList();
                var locLinkedContractsLogs = locLinkedContracts.ConvertAll(x => x.InactivateClinicLineofBusinessContract()).ToList();
                auditLogs.AddRange(locLinkedContractsLogs);

                //get active contract line of business by contractid
                var contractLineofBusiness = _unitOfWork.ContracBusinessLineRepository
                    .ContractLineofBusinessesByContract(contract.ContractId).ToList();
                var contractLineofBusinessLogs = contractLineofBusiness.ConvertAll(x => x.InactivateContractLineofBusinessRecord()).ToList();
                auditLogs.AddRange(contractLineofBusinessLogs);

                //get active contract addendums by contractid
                var addendums = _unitOfWork.Addendums.GetContractAddendumsByContract(contract.ContractId).ToList();
                var addendumsLogs = addendums.ConvertAll(x => x.InactiveContractAddendum()).ToList();
                auditLogs.AddRange(addendumsLogs);

                var log = contractStoredInDb.InactivateContract();
                auditLogs.Add(log);

                _unitOfWork.AuditLogs.AddRange(auditLogs);

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
                {
                    return BadRequest(ModelState);
                }

                var contractStoredInDb = _unitOfWork.Contracts.Get(contract.ContractId);

                if (contractStoredInDb == null)
                {
                    return NotFound();
                }

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
                .Where(x => x.Corporation.Active && x.Insurance.Active)
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
