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
                    return NotFound();
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
                    return BadRequest(ModelState);

                var doctor = _unitOfWork.Doctors.Get(doctorDto.DoctorId);

                if (doctor == null)
                    return NotFound();

                //Get all asociations between this doctor and the clinics where he works.
                var clinicWhereDoctorWorks = _unitOfWork.ClinicDoctor
                    .EnumarableGetAll(dc => dc.DoctorId == doctorDto.DoctorId).ToList();

                //Inactivate all those associations
                clinicWhereDoctorWorks.ForEach(dc => dc.ReleaseDoctorFromClinic());

                //Get all doctor contracts and inactivate them.

                //Inactivate the doctor
                doctor.Inactivate();

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
                    return BadRequest(ModelState);

                var doctor = _unitOfWork.Doctors.Get(doctorDto.DoctorId);
                if (doctor == null)
                    return NotFound();

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
