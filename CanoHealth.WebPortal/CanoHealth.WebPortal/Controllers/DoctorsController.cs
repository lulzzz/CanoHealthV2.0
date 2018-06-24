using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public ActionResult Index()
        {
            return View("Doctors");
        }

        public ActionResult ReadDoctors([DataSourceRequest] DataSourceRequest request)
        {
            var result = _unitOfWork.Doctors.GetAllDoctorsInTheSystem()
                                    .Select(Mapper.Map<Doctor, DoctorFormViewModel>)
                                    .ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActiveDoctors([DataSourceRequest] DataSourceRequest request)
        {
            var result = _unitOfWork
                .Doctors
                .GetAllActiveDoctors()
                .Select(DoctorDto.Wrap)
                .ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult CreateDoctor([DataSourceRequest] DataSourceRequest request, DoctorFormViewModel doctor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    doctor.DoctorId = Guid.NewGuid();

                    var duplicateSsn = _unitOfWork.Doctors.DuplicateSocialSecurityNumber(doctor.DoctorId.Value, doctor.SocialSecurityNumber);
                    if (duplicateSsn != null)
                    {
                        ModelState.AddModelError("SocialSecurityNumber", $"Duplicate Data. There is a doctor with this Social Security Number. Doctor details: {duplicateSsn.GetFullName()}");
                        return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
                    }

                    var duplicateNpi = _unitOfWork.Doctors.DuplicateNationalProviderIdentifier(doctor.DoctorId.Value, doctor.NpiNumber);
                    if (duplicateNpi != null)
                    {
                        ModelState.AddModelError("NpiNumber", $"Duplicate Data. There is a doctor with this NPI Number. Doctor details: {duplicateNpi.GetFullName()}");
                        return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
                    }

                    var duplicateCaqh = _unitOfWork.Doctors.DuplicateCaqh(doctor.DoctorId.Value, doctor.CaqhNumber);
                    if (duplicateCaqh != null)
                    {
                        ModelState.AddModelError("CaqhNumber", $"Duplicate Data. There is a doctor with this CAQH Number. Doctor details: {duplicateCaqh.GetFullName()}");
                        return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
                    }

                    var doctorToStore = Mapper.Map(doctor, new Doctor());
                    doctorToStore.Active = true;

                    var logs = _unitOfWork.Doctors.SaveDoctors(new List<Doctor> { doctorToStore });

                    _unitOfWork.AuditLogs.AddRange(logs);

                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("", "Something failed. Please contact your system administrator.");
            }
            return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult UpdateDoctor([DataSourceRequest] DataSourceRequest request, DoctorFormViewModel doctor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var duplicateSsn = _unitOfWork.Doctors.DuplicateSocialSecurityNumber(doctor.DoctorId.Value, doctor.SocialSecurityNumber);
                    if (duplicateSsn != null)
                    {
                        ModelState.AddModelError("SocialSecurityNumber", $"Duplicate Data. There is a doctor with this Social Security Number. Doctor details: {duplicateSsn.GetFullName()}");
                        return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
                    }

                    var duplicateNpi = _unitOfWork.Doctors.DuplicateNationalProviderIdentifier(doctor.DoctorId.Value, doctor.NpiNumber);
                    if (duplicateNpi != null)
                    {
                        ModelState.AddModelError("NpiNumber", $"Duplicate Data. There is a doctor with this NPI Number. Doctor details: {duplicateNpi.GetFullName()}");
                        return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
                    }

                    var duplicateCaqh = _unitOfWork.Doctors.DuplicateCaqh(doctor.DoctorId.Value, doctor.CaqhNumber);
                    if (duplicateCaqh != null)
                    {
                        ModelState.AddModelError("CaqhNumber", $"Duplicate Data. There is a doctor with this CAQH Number. Doctor details: {duplicateCaqh.GetFullName()}");
                        return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
                    }

                    var doctorToStore = Mapper.Map(doctor, new Doctor());

                    var logs = _unitOfWork.Doctors.SaveDoctors(new List<Doctor> { doctorToStore });

                    _unitOfWork.AuditLogs.AddRange(logs);

                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("", "Something failed. Please contact your system administrator.");
            }
            return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult InactivateDoctor([DataSourceRequest] DataSourceRequest request, DoctorFormViewModel doctor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var doctorStoredInDb = _unitOfWork.Doctors.Get(doctor.DoctorId.Value);
                    if (doctorStoredInDb == null)
                    {
                        ModelState.AddModelError("", "This doctor is not in our system. Please try again.");
                        return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
                    }
                    var log = doctorStoredInDb.Inactivate();

                    _unitOfWork.AuditLogs.Add(log);

                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("", "Something failed. Please contact your system administrator.");
            }
            return Json(new[] { doctor }.ToDataSourceResult(request, ModelState));
        }
    }
}