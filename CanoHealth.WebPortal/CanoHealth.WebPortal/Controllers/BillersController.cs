using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    [Authorize(Roles = "BILLER")]
    public class BillersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BillersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Corporations
        public ActionResult Corporations()
        {
            return View();
        }
        public ActionResult ReadCorporations([DataSourceRequest] DataSourceRequest request)
        {
            var result = _unitOfWork
                .Corporations
                .GetAll()
                .Select(Mapper.Map<Corporation, CorporationViewModel>);
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // GET: Locations
        public ActionResult Locations()
        {
            ViewData["Corporations"] = _unitOfWork.Corporations.GetActiveCorporations()
                    .Select(c => new { c.CorporationId, c.CorporationName });
            return View();
        }
        public ActionResult ReadLocations([DataSourceRequest] DataSourceRequest request)
        {
            var result = _unitOfWork.PlaceOfServices.GetAll()
                        .Select(Mapper.Map<PlaceOfService, PlaceOfServiceFormViewModel>).ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // GET: Doctors
        public ActionResult Doctors()
        {
            return View();
        }
        public ActionResult ReadDoctors([DataSourceRequest] DataSourceRequest request)
        {
            var result = _unitOfWork.Doctors.GetAllDoctorsInTheSystem()
                                    .Select(Mapper.Map<Doctor, DoctorFormViewModel>)
                                    .ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // GET: Contracts
        public ActionResult Contracts()
        {
            ViewData["Corporations"] = _unitOfWork.Corporations.GetActiveCorporations()
               .Select(x => new { x.CorporationId, x.CorporationName });
            return View();
        }
        public ActionResult ReadContracts([DataSourceRequest] DataSourceRequest request)
        {
            var result = _unitOfWork.Contracts.GetContractWithInsurance()
                .Select(ContractFormViewModel.Wrap).ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}