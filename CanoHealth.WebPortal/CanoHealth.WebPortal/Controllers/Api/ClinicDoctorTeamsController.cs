using AutoMapper;
using CanoHealth.WebPortal.CommonTools.CurrentDateTime;
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
    public class ClinicDoctorTeamsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentDateTimeService _getDateTimeService;

        public ClinicDoctorTeamsController(IUnitOfWork unitOfWork, ICurrentDateTimeService getDateTimeService)
        {
            _unitOfWork = unitOfWork;
            _getDateTimeService = getDateTimeService;
        }

        [HttpPost]
        public IHttpActionResult AssignDoctorToClinic(DoctorClinicDto doctorClinicDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var doctor = _unitOfWork.Doctors.FinDoctor(doctorClinicDto.FirstName, doctorClinicDto.LastName, doctorClinicDto.DateOfBirth,
                                        doctorClinicDto.SocialSecurityNumber, doctorClinicDto.NpiNumber, doctorClinicDto.CaqhNumber);
                /*El usuario puede que no seleccione el doctor en el autocomplete y aun asi decide entrar la misma informacion del doctor no seleccionado*/
                if (doctor == null)
                {
                    doctorClinicDto.DoctorId = doctorClinicDto.DoctorId ?? Guid.NewGuid();
                    doctor = doctorClinicDto.ConvertDoctor();

                    //Verify if exist another doctor with the same ssn
                    var duplicateSsn = _unitOfWork.Doctors.DuplicateSocialSecurityNumber(doctor.DoctorId, doctor.SocialSecurityNumber);
                    if (duplicateSsn != null)
                    {
                        return Content(HttpStatusCode.BadRequest, new ErrorResponseDto
                        {
                            ErrorResponse = "Duplicate Data. here is a doctor with this Social Security Number. Doctor details: ",
                            DomainModel = duplicateSsn
                        });
                    }

                    //verify if exist another doctor with the same npi
                    var duplicateNpi = _unitOfWork.Doctors.DuplicateNationalProviderIdentifier(doctor.DoctorId, doctor.NpiNumber);
                    if (duplicateNpi != null)
                    {
                        return Content(HttpStatusCode.BadRequest, new ErrorResponseDto
                        {
                            ErrorResponse = "Duplicate Data. There is a doctor with this NPI Number. Doctor details: ",
                            DomainModel = duplicateNpi
                        });
                    }

                    //verify if exist another doctor with the same caqh 
                    var duplicateCaqh = _unitOfWork.Doctors.DuplicateCaqh(doctor.DoctorId, doctor.CaqhNumber);
                    if (duplicateCaqh != null)
                    {
                        return Content(HttpStatusCode.BadRequest, new ErrorResponseDto
                        {
                            ErrorResponse = "Duplicate Data. There is a doctor with this CAQH Number. Doctor details: ",
                            DomainModel = duplicateCaqh
                        });
                    }
                }
                else
                {
                    doctorClinicDto.DoctorId = doctor.DoctorId;
                    doctorClinicDto.ActiveDoctorClinicRelationship = true;
                    doctor.Active = true; // Si el doctor esta inactivo por convencion voy a activarlo.
                }

                var saveDoctorLogs = _unitOfWork.Doctors.SaveDoctors(new List<Doctor> { doctor });

                var doctorClinic = _unitOfWork.ClinicDoctor.FindDoctorClinicRelationship(doctor.DoctorId, doctorClinicDto.PlaceOfServiceId);
                if (doctorClinic != null)
                {
                    doctorClinicDto.DoctorClinicId = doctorClinic.DoctorClinicId;
                    doctorClinicDto.ActiveDoctorClinicRelationship = true; //If false activate doctor-clinic association (dto).
                }
                //Asumiendo que la fecha en que empieza un doctor a trabajar en una clinica, va a ser
                //la fecha actual del sistema. En caso de que el usuario quiera poder establecer esta 
                //fecha habra que hacer cambios en el UI y poner un datepicker.
                doctorClinicDto.FromDateTime = _getDateTimeService.GetCurrentDateTime();
                doctorClinic = doctorClinicDto.ConvertDoctorClinic();

                var assignDoctorLogs = _unitOfWork.ClinicDoctor.AssignDoctorsToClinic(new List<DoctorClinic> { doctorClinic });

                /*Everytime a doctor is assigned or re-assigned to a location
                 the system needs to find all the inactive doctor's provider
                 related to that location and activate them. */
                var locationProviders = _unitOfWork.DoctorLinkedContracts
                    .GetLocationProvidersOfThisDoctor(doctorClinicDto.DoctorId,
                        doctorClinicDto.PlaceOfServiceId)
                    .Where(pbl => !pbl.Active);

                var activateProviderLogs = _unitOfWork.ProviderByLocationRepository
                    .ActivateProviders(locationProviders);

                _unitOfWork.AuditLogs.AddRange(saveDoctorLogs);
                _unitOfWork.AuditLogs.AddRange(assignDoctorLogs);
                _unitOfWork.AuditLogs.AddRange(activateProviderLogs);

                doctorClinicDto.DoctorClinicId = doctorClinic.DoctorClinicId;

                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(doctorClinicDto);
        }

        [HttpDelete]
        public IHttpActionResult UnAssignDoctorToClinic(DoctorClinicDto doctorClinicDto)
        {
            try
            {
                var doctorClinincAssociation = _unitOfWork.ClinicDoctor
                    .FindDoctorClinicRelationship(doctorClinicDto.DoctorId.Value, doctorClinicDto.PlaceOfServiceId);
                if (doctorClinincAssociation == null)
                    return NotFound();

                var releaseLog = doctorClinincAssociation.ReleaseDoctorFromClinic();

                /*Everytime a doctor is released from a location
                 the system needs to find all the active doctor's provider
                 related to that location an inactivate them.*/
                var locationProviders = _unitOfWork.DoctorLinkedContracts
                    .GetLocationProvidersOfThisDoctor(doctorClinicDto.DoctorId,
                        doctorClinicDto.PlaceOfServiceId)
                    .Where(pbl => pbl.Active);

                var releaseLocationProviders = _unitOfWork.ProviderByLocationRepository
                    .InactivateProviders(locationProviders);

                _unitOfWork.AuditLogs.Add(releaseLog);
                _unitOfWork.AuditLogs.AddRange(releaseLocationProviders);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(doctorClinicDto);
        }

        [HttpGet]
        public IHttpActionResult GetDoctorsLocations(Guid doctorId)
        {
            try
            {
                var locations = _unitOfWork.ClinicDoctor
                             .GetLocationsWhereDoctorWorks(doctorId)
                             .Select(Mapper.Map<PlaceOfService, PlaceOfServiceDto>)
                             .ToList();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
        }
    }
}