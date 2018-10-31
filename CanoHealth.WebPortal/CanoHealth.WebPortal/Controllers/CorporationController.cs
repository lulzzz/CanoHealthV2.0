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
    [Authorize(Roles = "ADMIN")]
    public class CorporationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CorporationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Corporation
        public ActionResult Index()
        {
            return View("Corporations");
        }

        [ValidateAntiForgeryToken]
        public ActionResult ReadCorporations([DataSourceRequest] DataSourceRequest request)
        {
            var cookie = Request.Cookies["__RequestVerificationToken"];
            var token = Request.Form["__RequestVerificationToken"];
            try
            {
                var result = _unitOfWork
                             .Corporations
                             .GetAll()
                             .Select(Mapper.Map<Corporation, CorporationViewModel>);
                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (HttpAntiForgeryException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return Redirect("~/Views/Shared/Error.cshtml");
            }
        }

        public ActionResult ReadActiveCorporations([DataSourceRequest] DataSourceRequest request)
        {
            var corporations = _unitOfWork
                .Corporations
                .GetActiveCorporations()
                .Select(Mapper.Map<Corporation, CorporationViewModel>);
            return Json(corporations.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCorporations(string text = null)
        {
            var result = _unitOfWork.Corporations
                .GetActiveCorporations()
                .Select(Mapper.Map<Corporation, CorporationDto>)
                .ToList();
            if (!String.IsNullOrEmpty(text))
                result = result.Where(c => c.CorporationName.ToLower().Contains(text.ToLower())).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByParam = "currentUserId")]
        public ActionResult RenderCorporationInLayout(string currentUserId)
        {
            var corporations = _unitOfWork
                .Corporations
                .GetActiveCorporations()
                .Select(Mapper.Map<Corporation, CorporationViewModel>)
                .ToList();
            return PartialView("_Corporations", corporations);
        }

        [ValidateAntiForgeryToken]
        public ActionResult SaveCorporation([DataSourceRequest] DataSourceRequest request, CorporationFormViewModel corporation)
        {
            var cookie = Request.Cookies["__RequestVerificationToken"]; //from server
            var token = Request.Form["__RequestVerificationToken"]; //from client form

            if (ModelState.IsValid)
            {
                try
                {
                    corporation.CorporationId = corporation.CorporationId ?? Guid.NewGuid();
                    var convert = Mapper.Map(corporation, new Corporation());
                    _unitOfWork.Corporations.SaveCorporations(new List<Corporation> { convert });
                    _unitOfWork.Complete();
                }
                catch (HttpAntiForgeryException ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    return Redirect("~/Views/Shared/Error.cshtml");
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, String.Format("An exception has occurred: {0}", ex));
                    ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again.");
                }
            }
            return Json(new[] { corporation }.ToDataSourceResult(request, ModelState));
        }
    }
}