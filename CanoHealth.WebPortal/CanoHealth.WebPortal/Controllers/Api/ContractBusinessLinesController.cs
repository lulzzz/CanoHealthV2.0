using CanoHealth.WebPortal.CommonTools.ExtensionMethods;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class ContractBusinessLinesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractBusinessLinesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                    .GetContractBusinessLineItemWithClinics(contractBusinessLinesDto.ContractLineofBusinessId);

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
        public IHttpActionResult DeleteContractBusinessLinesRelation(ContractBusinessLinesFormsDto contractBusinessLinesDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var contractBusinessLineStoredInDb = _unitOfWork.ContracBusinessLineRepository
                    .GetContractBusinessLineItemWithClinics(contractBusinessLinesDto.ContractLineofBusinessId);

                if (contractBusinessLineStoredInDb == null)
                    return NotFound();

                var logs = _unitOfWork.ContracBusinessLineClinicRepository
                    .GetLogsWhileRemoveItems(contractBusinessLineStoredInDb.ClinicLineofBusiness)
                    .ToList();

                var contractBusinessLinesLogs = _unitOfWork.ContracBusinessLineRepository
                    .GetLogsWhileRemoveItems(new List<ContractLineofBusiness> { contractBusinessLineStoredInDb })
                    .ToList();

                logs.AddRange(contractBusinessLinesLogs);

                _unitOfWork.AuditLogs.AddRange(logs);

                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Content(HttpStatusCode.OK, contractBusinessLinesDto);
        }
    }
}
