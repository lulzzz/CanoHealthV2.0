using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class LinkedContractsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LinkedContractsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetLinkedContractByDoctorAndInsurance(Guid doctorId, string insuranceName)
        {
            var result = _unitOfWork.LinkedContractStoredProcedures
                                    .GetLinkedContractByDoctor(doctorId, insuranceName);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult LinkDoctorToCorporationContract(
            LinkedContractFormDto linkedContractDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var linkedContract = linkedContractDto.Convert();

                /*Verify if there is a previous linked contract for the doctor and business line*/
                var linkedContractFound = _unitOfWork.DoctorLinkedContracts
                    .FindLinkedContract(linkedContract.DoctorCorporationContractLinkId,
                        linkedContract.DoctorId,
                        linkedContract.ContractLineofBusinessId);
                if (linkedContractFound != null)
                {
                    ModelState.AddModelError("", "Duplicate data. Please try again!");
                    return BadRequest(ModelState);
                }
                var linkedContractLogs = _unitOfWork.DoctorLinkedContracts.SaveLinkedContracts(new List<DoctorCorporationContractLink> { linkedContract });

                //var businessLineLocations = _unitOfWork.ContracBusinessLineClinicRepository
                //    .GetLocationsByBusinessLines(linkedContractDto.ContractLineofBusinessId)
                //    .ToList();

                //var doctorLocations = _unitOfWork.ClinicDoctor
                //    .GetLocationsWhereDoctorWorks(linkedContractDto.DoctorId)
                //    .ToList();

                //if (doctorLocations.Any())
                //{
                //    var intersection = businessLineLocations
                //        .Intersect(doctorLocations)
                //        .ToList();
                //    /*Por cada una de las intersecciones se tiene que crear un objecto 
                //     con el DoctorCorporationContractLinkId, PlaceOfServiceId, 
                //     ProviderNUmber = null*/
                //    var providersByLocation = intersection.Select(pbl =>
                //        new ProviderByLocation
                //        {
                //            ProviderByLocationId = Guid.NewGuid(),
                //            Active = true,
                //            DoctorCorporationContractLinkId = linkedContract.DoctorCorporationContractLinkId,
                //            PlaceOfServiceId = pbl.PlaceOfServiceId
                //        }).ToList();
                //    var providerByLocationsLogs = _unitOfWork.ProviderByLocationRepository
                //                             .SaveProviderByLocation(providersByLocation);
                //    _unitOfWork.AuditLogs.AddRange(providerByLocationsLogs);
                //}

                _unitOfWork.AuditLogs.AddRange(linkedContractLogs);
                _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return InternalServerError(e);
            }
            return Ok(linkedContractDto);
        }

        [HttpDelete]
        public IHttpActionResult DeleteLinkedContract(LinkedContractFormDto linkedContractDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var linkedContractStoreInDb = _unitOfWork.DoctorLinkedContracts
                    .Get(linkedContractDto.DoctorCorporationContractLinkId);

                if (linkedContractStoreInDb == null)
                    return Content(HttpStatusCode.NotFound, linkedContractDto);

                var locationProviders = _unitOfWork.ProviderByLocationRepository
                    .EnumarableGetAll(x =>
                        x.DoctorCorporationContractLinkId == linkedContractDto.DoctorCorporationContractLinkId);

                var removeProviderLogs = _unitOfWork.ProviderByLocationRepository
                    .RemoveProviders(locationProviders);
                _unitOfWork.AuditLogs.AddRange(removeProviderLogs);

                var removeLinkedContractsLogs = _unitOfWork.DoctorLinkedContracts
                    .RemoveLinkedContracts(
                    new List<DoctorCorporationContractLink>
                    {
                        linkedContractStoreInDb
                    });
                _unitOfWork.AuditLogs.AddRange(removeLinkedContractsLogs);

                _unitOfWork.Complete();

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(linkedContractDto);
        }
    }
}
