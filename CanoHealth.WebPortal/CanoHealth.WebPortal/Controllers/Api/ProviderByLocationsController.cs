using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class ProviderByLocationsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProviderByLocationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetActiveProvidersByLocation(Guid doctorCorporationContractLinkId)
        {
            var result = _unitOfWork.ProviderByLocationRepository
                .GetActiveProvidersByLocation(doctorCorporationContractLinkId)
                .Select(Mapper.Map<ProviderByLocation, ProviderByLocationDto>);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetActualProviderByLocation(
            Guid doctorCorporationContractLinkId,
            Guid contractLineofBusinessId,
            Guid doctorId)
        {
            //Get the doctor info to display in the list of providers by locations
            var doctor = _unitOfWork.Doctors.Get(doctorId);
            if (doctor == null)
                return Content(HttpStatusCode.NotFound,
                    "Doctor not found.");

            var contractLineOfBusinessItem = _unitOfWork.ContracBusinessLineRepository
                .Get(contractLineofBusinessId);
            if (contractLineOfBusinessItem == null)
                return Content(HttpStatusCode.NotFound,
                   "There is not corporation's contract in the system with this line of business.");

            var linkedContract = _unitOfWork.DoctorLinkedContracts
                .Get(doctorCorporationContractLinkId);
            if (linkedContract == null)
                return Content(HttpStatusCode.NotFound, "There is not doctor linked to this corporation's contract.");

            /*Get locations related to a business line of specific contract*/
            var businessLineLocations = _unitOfWork.ContracBusinessLineClinicRepository
                   .GetLocationsByBusinessLines(contractLineofBusinessId)
                   .ToList();
            /*Get locations where the doctor works today*/
            var doctorLocations = _unitOfWork.ClinicDoctor
                .GetLocationsWhereDoctorWorks(doctorId)
                .ToList();
            /*Get the actual location providers stored in db*/
            var providerByLocationStoredInDb = _unitOfWork.ProviderByLocationRepository
                .GetActiveProvidersByLocation(doctorCorporationContractLinkId)
                .ToList();
            /*if at this moment the doctor is not working in any location yet inactivate
             the actual location providers stored in db*/
            if (doctorLocations.Any())
            {
                /*Get the intersection between doctor's locations and business line's locations*/
                var intersection = businessLineLocations
                        .Intersect(doctorLocations)
                        .Select(l => l.PlaceOfServiceId)
                        .ToList();

                var toCreate = intersection.Except(providerByLocationStoredInDb
                     .Select(x => x.PlaceOfServiceId))
                     .Select(p => new ProviderByLocation
                     {
                         ProviderByLocationId = Guid.NewGuid(),
                         PlaceOfServiceId = p,
                         Active = true,
                         DoctorCorporationContractLinkId = doctorCorporationContractLinkId
                     }).ToList();
                var newProviderLogs = _unitOfWork.ProviderByLocationRepository
                     .SaveProviderByLocation(toCreate);
                _unitOfWork.AuditLogs.AddRange(newProviderLogs);

                var toInactivate = providerByLocationStoredInDb
                    .Select(x => x.PlaceOfServiceId)
                    .Except(intersection).ToList();

                foreach (var item in toInactivate)
                {
                    var providerByLocation = providerByLocationStoredInDb.SingleOrDefault(x => x.PlaceOfServiceId == item
                                             && x.DoctorCorporationContractLinkId == doctorCorporationContractLinkId);
                    var log = providerByLocation?.Inactivate();
                    _unitOfWork.AuditLogs.Add(log);
                }
            }
            else
            {
                var inactivateProviderLogs = providerByLocationStoredInDb
                    .Select(x => x.Inactivate()).ToList();
                _unitOfWork.AuditLogs.AddRange(inactivateProviderLogs);
            }
            _unitOfWork.Complete();

            return Ok(DoctorDto.Wrap(doctor));
        }
    }
}
