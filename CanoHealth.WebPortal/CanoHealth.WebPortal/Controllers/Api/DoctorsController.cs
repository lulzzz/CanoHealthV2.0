using AutoMapper;
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
    public class DoctorsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetAvailableDoctors(Guid placeOfServiceId)
        {
            var allActiveDoctors = _unitOfWork.Doctors.GetAllActiveDoctors().ToList();

            //Doctores que estan activos en el systema y que ademas se encuentran trabajando en estos momentos en la clinica
            var activeTakenDoctors = _unitOfWork.ClinicDoctor.GetListOfDoctorsThatWorkInThisPlaceOfServiceToday(placeOfServiceId);

            var availableDoctorsToSelect = allActiveDoctors.Except(activeTakenDoctors).ToList();
            return Ok(availableDoctorsToSelect.Select(Mapper.Map<Doctor, DoctorDto>));
        }

        [HttpGet]
        public IHttpActionResult GetDoctorInfo(Guid doctorId)
        {
            try
            {
                var doctorStoredInDb = _unitOfWork.Doctors.Get(doctorId);
                if (doctorStoredInDb == null)
                {
                    return NotFound();
                }

                return Ok(DoctorDto.Wrap(doctorStoredInDb));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        public IHttpActionResult InactivateDoctor(DoctorDto doctorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var doctor = _unitOfWork.Doctors.Get(doctorDto.DoctorId);

                if (doctor == null)
                {
                    return NotFound();
                }

                var auditLogs = new List<AuditLog>();

                //Get all asociations between this doctor and the clinics where he works.(DoctorClinics)
                var clinicWhereDoctorWorks = _unitOfWork.ClinicDoctor
                    .EnumarableGetAll(dc => dc.DoctorId == doctorDto.DoctorId).ToList();

                //Inactivate all those associations
                var doctorLocationLogs = clinicWhereDoctorWorks.ConvertAll(dc => dc.ReleaseDoctorFromClinic()).ToList();
                auditLogs.AddRange(doctorLocationLogs);

                //get active doctor's files(DoctorFiles)
                var docFiles = _unitOfWork.PersonalFileRepository.GetActivePersonalFiles(doctor.DoctorId).ToList();
                var docFilesLogs = docFiles.ConvertAll(x => x.Inactivate()).ToList();
                auditLogs.AddRange(docFilesLogs);

                //get active doctor's schedules(DoctorSchedules)
                var docSchedules = _unitOfWork.DoctorScheduleRepository.GetSchedulesByDoctorId(doctor.DoctorId).ToList();
                var docSchedulesLogs = docSchedules.ConvertAll(x => x.InactivateDoctorSchedule()).ToList();
                auditLogs.AddRange(docSchedulesLogs);

                //get active doctor's out of network contracts(OutOfNetworkContracts)
                var ooContracts = _unitOfWork.OutofNetworkContractRepository.GetActiveOutOfNetworkContractsByDoctor(doctor.DoctorId).ToList();
                var oocontractLogs = ooContracts.ConvertAll(x => x.InactiveOutofNetworkContract()).ToList();
                auditLogs.AddRange(oocontractLogs);

                //get active doctor's indivudual providers(DoctorIndividualProviders)
                var individualProviders = _unitOfWork.IndividualProviderRepository.GetIndividualProviders(doctor.DoctorId).ToList();
                var individualProvidersLogs = individualProviders.ConvertAll(x => x.InactivateDoctorInsuranceRelationship()).ToList();
                auditLogs.AddRange(individualProvidersLogs);

                //get active doctor's providers by locations(ProviderByLocations)
                var providerByLocations = _unitOfWork.ProviderByLocationRepository.ProviderByLocationsAndDoctor(doctor.DoctorId).ToList();
                var providerByLocationLogs = providerByLocations.ConvertAll(x => x.Inactivate()).ToList();
                auditLogs.AddRange(providerByLocationLogs);

                //get active doctor's contracts(DoctorCorporationContractLinks) 
                var docLinkedContract = _unitOfWork.DoctorLinkedContracts.DoctorCorporationContractLinksByDoctor(doctor.DoctorId).ToList();
                var docLinkedContractLogs = docLinkedContract.ConvertAll(x => x.InactiveDoctorCorporationContractLinkRecord()).ToList();
                auditLogs.AddRange(docLinkedContractLogs);

                //Inactivate the doctor
                var inactiveDocLog = doctor.Inactivate();
                auditLogs.Add(inactiveDocLog);

                _unitOfWork.AuditLogs.AddRange(auditLogs);

                _unitOfWork.Complete();
                doctorDto.Active = false;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(doctorDto);
        }

        [HttpPost]
        public IHttpActionResult UpdateDoctorInfo(DoctorDto doctorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var doctor = _unitOfWork.Doctors.Get(doctorDto.DoctorId);
                if (doctor == null)
                {
                    return NotFound();
                }

                var logs = _unitOfWork.Doctors.SaveDoctors(new List<Doctor> { Mapper.Map(doctorDto, new Doctor()) });
                _unitOfWork.AuditLogs.AddRange(logs);

                _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return InternalServerError(e);
            }
            return Ok(doctorDto);
        }
    }
}