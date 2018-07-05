﻿using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class PlaceOfServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlaceOfServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            ViewData["Corporations"] = _unitOfWork.Corporations.GetActiveCorporations()
                    .Select(c => new { c.CorporationId, c.CorporationName });
            return View("PlaceOfService");
        }

        public ActionResult ReadPlaceOfServices([DataSourceRequest] DataSourceRequest request)
        {
            var result = _unitOfWork.PlaceOfServices.GetAll()
                        .Select(Mapper.Map<PlaceOfService, PlaceOfServiceFormViewModel>).ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetLocations(Guid? corporationId)
        {
            var result = _unitOfWork
                .PlaceOfServices
                .GetAllActivePlaceOfServices(corporationId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddPlaceOfService([Bind(Exclude = "PlaceOfServiceId",
            Include = "CorporationId,Name,Address,PhoneNumber,FaxNumber,Active")]
            [DataSourceRequest] DataSourceRequest request,
            PlaceOfServiceFormViewModel placeOfService)
        {
            placeOfService.PlaceOfServiceId = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                try
                {
                    var pos = _unitOfWork.PlaceOfServices.GetPlaceOfServiceByName(placeOfService.Name);
                    if (pos != null)
                    {
                        ModelState.AddModelError("Name", "Duplicate Location.");
                        return Json(new[] { placeOfService }.ToDataSourceResult(request, ModelState));
                    }
                    var posconverted = Mapper.Map(placeOfService, new PlaceOfService());
                    _unitOfWork.PlaceOfServices.Add(posconverted);
                    _unitOfWork.Complete();
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("", "Something failed. Please contact your system administrator.");
                }
            }
            return Json(new[] { placeOfService }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult UpdatePlaceOfService([DataSourceRequest] DataSourceRequest request,
            PlaceOfServiceFormViewModel placeOfService)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Wrong Data.");
                var placeOfServiceStoredInDb = _unitOfWork.PlaceOfServices.Get(placeOfService.PlaceOfServiceId.Value);

                if (placeOfServiceStoredInDb == null)
                    return HttpNotFound("This Location is no longer in our system. Please try again.");

                var duplicatePlaceOfService =
                    _unitOfWork.PlaceOfServices.FindOtherPlaceOfServiceWithSameName(placeOfService.Name,
                        placeOfService.PlaceOfServiceId.Value);

                if (duplicatePlaceOfService != null)
                    return new HttpStatusCodeResult(HttpStatusCode.Created,
                        "There is another Location with the same name. Please avoid duplicated data.");

                var auditLogs = placeOfServiceStoredInDb.Modify(placeOfService.CorporationId, placeOfService.Name,
                    placeOfService.Address,
                    placeOfService.PhoneNumber, placeOfService.FaxNumber, placeOfService.Active);

                _unitOfWork.AuditLogs.AddRange(auditLogs);

                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, String.Format("An exception has occurred: {0}", ex));
            }
            return Json(new[] { placeOfService }.ToDataSourceResult(request, ModelState));
        }
    }
}