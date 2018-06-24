using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    [Authorize]
    public class SearchEngineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchEngineController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: SearchEngine
        public ActionResult Index()
        {
            return View("Search");
        }

        public ActionResult GetDoctorByLocationAndLineOfBusiness([DataSourceRequest] DataSourceRequest request,
            Guid locationId, Guid contractLineofBusinessId)
        {
            //Get the current list of doctors who work in this location
            var activeDoctorByLocation = _unitOfWork.ClinicDoctor
                                        .GetListOfDoctorsThatWorkInThisPlaceOfServiceToday(locationId)
                                        .ToList();

            //Get list of doctors who are linked to this contract
            var activeLinkedDoctors = _unitOfWork.DoctorLinkedContracts
                                     .GetDoctorsLinkedToLineOfBusiness(contractLineofBusinessId)
                                     .Select(d => d.Doctor)
                                     .Where(d => d.Active)
                                     .ToList();

            //Get the list of doctors who work in this location but also are linked to this contract
            var doctors = activeDoctorByLocation.Intersect(activeLinkedDoctors)
                                     .Select(DoctorDto.Wrap)
                                     .ToList();

            return Json(doctors.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}