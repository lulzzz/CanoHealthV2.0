using CanoHealth.WebPortal.CommonTools.ExtensionMethods;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.Services.AuditLogs.DoctorProviderByLocation;
using Elmah;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class ContractBusinessLinesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IProviderByLocationLog _providerByLocationLog;

        public ContractBusinessLinesController(IUnitOfWork unitOfWork, IProviderByLocationLog providerByLocationLog)
        {
            _unitOfWork = unitOfWork;
            _providerByLocationLog = providerByLocationLog;
        }

        [HttpGet]
        public IHttpActionResult GetContractBusinessLines(string contractId = null)
        {
            try
            {
                var result = _unitOfWork.ContracBusinessLineRepository
                    .GetContractBusinessLinesWithClinics(contractId)                    
                    .Select(ContractBusinessLinesFormsDto.WrapContractBusinessLines);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult CreateContractBusinessLinesRelation(ContractBusinessLinesFormsDto contractBusinessLinesDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                contractBusinessLinesDto.ContractLineofBusinessId = Guid.NewGuid();

                var contractBusinessLine = contractBusinessLinesDto.CreateContractBusinessLineItem();
                if (_unitOfWork.ContracBusinessLineRepository.ExistItem(contractBusinessLine.ContractId, contractBusinessLine.PlanTypeId) != null)
                {
                    ModelState.AddModelError("", $"{contractBusinessLinesDto.Name} is already associated to the contract.");
                    return BadRequest(ModelState);
                }

                var contractBusinessLinesClinics = contractBusinessLinesDto.CreateContractBusinessLinesClinicsItems()
                    .ToList();
                _unitOfWork.ContracBusinessLineRepository.Add(contractBusinessLine);

                _unitOfWork.ContracBusinessLineClinicRepository.AddRange(contractBusinessLinesClinics);

                var auditLogs = contractBusinessLinesDto.GetLogsForCreation(contractBusinessLinesClinics);

                _unitOfWork.AuditLogs.AddRange(auditLogs);

                _unitOfWork.Complete();


                var clinics = contractBusinessLinesClinics.Convert(contractBusinessLinesDto.Clinics.Select(x => new PlaceOfServiceExtension
                {
                    Name = x.Name,
                    PlaceOfServiceId = x.PlaceOfServiceId
                }));
                contractBusinessLinesDto.Clinics = clinics;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Created(new Uri(Request.RequestUri + "/" + contractBusinessLinesDto.ContractLineofBusinessId), contractBusinessLinesDto);
        }

        [HttpPut]
        public IHttpActionResult UpdateContractBusinessLinesRelation(ContractBusinessLinesFormsDto contractBusinessLinesDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var contractBusinessLineStoredInDb = _unitOfWork.ContracBusinessLineRepository
                    .GetContractLineofBusinessAndLocations(contractBusinessLinesDto.ContractLineofBusinessId);

                if (contractBusinessLineStoredInDb == null)
                    return NotFound();

                var contractBusinessLine = contractBusinessLinesDto.CreateContractBusinessLineItem();
                if (_unitOfWork.ContracBusinessLineRepository.ExistItem(contractBusinessLine.ContractId, contractBusinessLine.PlanTypeId, contractBusinessLine.ContractLineofBusinessId) != null)
                {
                    ModelState.AddModelError("", $"{contractBusinessLinesDto.Name} is already associated to the contract.");
                    return BadRequest(ModelState);
                }

                var contractBusinessLinesClinics = contractBusinessLinesDto.CreateContractBusinessLinesClinicsItems()
                       .ToList();

                contractBusinessLine.ClinicLineofBusiness = contractBusinessLinesClinics;

                var result = contractBusinessLineStoredInDb.Modify(contractBusinessLine);

                _unitOfWork.ContracBusinessLineClinicRepository.RemoveRange(result.Cliniccontractlineofbusiness);

                _unitOfWork.AuditLogs.AddRange(result.Logs);

                _unitOfWork.Complete();

                var clinics = contractBusinessLinesClinics.Convert(contractBusinessLinesDto.Clinics.Select(x => new PlaceOfServiceExtension
                {
                    Name = x.Name,
                    PlaceOfServiceId = x.PlaceOfServiceId
                }));

                contractBusinessLinesDto.Clinics = clinics;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(contractBusinessLinesDto);
        }

        [HttpDelete]
        public IHttpActionResult InactivateRelationBetweenContractAndLineofBusiness(ContractBusinessLinesFormsDto contractBusinessLinesDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //Get the Contract and the specific Line of Business. Table: ContractLineofBusinesses
                var contractLineofBusinessStoredInDb = _unitOfWork.ContracBusinessLineRepository
                    .GetContractLineofBusinessAndLocations(contractBusinessLinesDto.ContractLineofBusinessId);

                if (contractLineofBusinessStoredInDb == null)
                    return NotFound();

                //Inactivate all the ClinicLineofBusinessContracts items related to contractLineofBusinessStoredInDb
                var auditLogs = contractLineofBusinessStoredInDb.InactivateRelationAmongContractLineofBusinessLocation().ToList();

                //Get the list of doctors with their provider by locations linked to this contract and this specific line of business.
                //Table: DoctorCorporationContractLinks
                var getDoctorsLinkedToContract = _unitOfWork.DoctorLinkedContracts
                    .GetDoctorsLinkedToLineOfBusiness(contractLineofBusinessStoredInDb.ContractLineofBusinessId)
                    .ToList();

                //Inactivate all ProviderByLocations for each getDoctorsLinkedToContract(table: DoctorCorporationContractLinks)
                foreach (DoctorCorporationContractLink linked in getDoctorsLinkedToContract)
                {
                    var logs = linked.InactivateRelationAmongContractLineofBusinessDoctor();
                    auditLogs.AddRange(logs);
                }

                _unitOfWork.AuditLogs.AddRange(auditLogs);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }

            return Content(HttpStatusCode.OK, contractBusinessLinesDto);
        }

        //[HttpDelete]
        //public IHttpActionResult DeleteContractBusinessLinesRelation(ContractBusinessLinesFormsDto contractBusinessLinesDto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        //Get the Contract and the specific Line of Business. Table: ContractLineofBusinesses
        //        var contractLineofBusinessStoredInDb = _unitOfWork.ContracBusinessLineRepository
        //            .GetContractLineofBusinessAndLocations(contractBusinessLinesDto.ContractLineofBusinessId);

        //        if (contractLineofBusinessStoredInDb == null)
        //            return NotFound();

        //        //Get the Locations associated to this specific Line of Business throw the Contract. Table: ClinicLineofBusinessContracts
        //        var logs = _unitOfWork.ContracBusinessLineClinicRepository
        //            .GetLogsWhileRemoveItems(contractLineofBusinessStoredInDb.ClinicLineofBusiness)
        //            .ToList();

        //        //Get the list of doctors with their provider by locations linked to this contract and this specific line of business.
        //        //Table: DoctorCorporationContractLinks
        //        var doctorsLinkedToContract = _unitOfWork.DoctorLinkedContracts
        //            .GetDoctorsLinkedToLineOfBusiness(contractLineofBusinessStoredInDb.ContractLineofBusinessId).ToList();

        //        foreach (var doctor in doctorsLinkedToContract)
        //        {
        //            var providersByLocation = doctor.ProvidersByLocations.ToList();
        //            logs.AddRange(_providerByLocationLog.GenerateLogsWhenDelete(providersByLocation));
        //            _unitOfWork.ProviderByLocationRepository.RemoveRange(providersByLocation);
        //        }

        //        var doctorsLinkedContractLogs = _unitOfWork.DoctorLinkedContracts.RemoveLinkedContracts(doctorsLinkedToContract);

        //        var contractBusinessLinesLogs = _unitOfWork.ContracBusinessLineRepository
        //            .GetLogsWhileRemoveItems(new List<ContractLineofBusiness> { contractLineofBusinessStoredInDb })
        //            .ToList();

        //        logs.AddRange(contractBusinessLinesLogs);
        //        logs.AddRange(doctorsLinkedContractLogs);

        //        _unitOfWork.AuditLogs.AddRange(logs);

        //        _unitOfWork.Complete();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorSignal.FromCurrentContext().Raise(ex);
        //        return InternalServerError(ex);
        //    }
        //    return Content(HttpStatusCode.OK, contractBusinessLinesDto);
        //}
    }
}
