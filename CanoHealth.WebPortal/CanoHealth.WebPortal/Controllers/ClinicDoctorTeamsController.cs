﻿using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class ClinicDoctorTeamsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClinicDoctorTeamsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult GetDoctors([DataSourceRequest] DataSourceRequest request, Guid placeOfServiceId)
        {
            var doctors = _unitOfWork.ClinicDoctor.GetListOfDoctorsThatWorkInThisPlaceOfServiceToday(placeOfServiceId)
                                     .Select(Mapper.Map<Doctor, DoctorFormViewModel>)
                                     .ToList();

            return Json(doctors.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDoctorsLocations([DataSourceRequest] DataSourceRequest request, Guid doctorId)
        {
            var locations = _unitOfWork.ClinicDoctor
                             .GetLocationsWhereDoctorWorks(doctorId)
                             .Select(Mapper.Map<PlaceOfService, PlaceOfServiceDto>)
                             .ToList();
            return Json(locations.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}