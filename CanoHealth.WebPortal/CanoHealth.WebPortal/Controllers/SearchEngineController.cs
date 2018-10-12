using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.ViewModels;
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

        public ActionResult GetDoctorByLocationAndLineOfBusiness(
            [DataSourceRequest] DataSourceRequest request,
            Guid locationId, Guid contractLineofBusinessId, Guid? insuranceId = null)
        {
            //Get the current list of doctors who work in this location
            var activeDoctorByLocation = _unitOfWork.ClinicDoctor
                                        .GetListOfDoctorsThatWorkInThisPlaceOfServiceToday(locationId)
                                        .ToList();

            var doctorCorporationContractLink = _unitOfWork.DoctorLinkedContracts
                                     .GetDoctorsLinkedToLineOfBusiness(contractLineofBusinessId)
                                     .ToList();

            //Get list of doctors who are linked to this contract
            var activeLinkedDoctors = doctorCorporationContractLink
                                     .Select(d => d.Doctor)
                                     .Where(d => d.Active)
                                     .ToList();

            //Get the list of doctors who work in this location but also are linked to this contract
            var doctors = activeDoctorByLocation.Intersect(activeLinkedDoctors)
                                     .Select(SearchDoctorResultViewModel.Wrap)
                                     .ToList();
            foreach (var doctor in doctors)
            {
                if (insuranceId != null)
                {
                    //Get the provider number of the doctor for the Insurance: insuranceId
                    var individualProvider = _unitOfWork
                        .IndividualProviderRepository
                        .ExistIndividualProvider(doctor.DoctorId, insuranceId.Value);
                    if (individualProvider != null)
                        doctor.IndividualProviderNumber = individualProvider.ProviderNumber;
                    else
                        doctor.IndividualProviderNumber = "NONE.";
                }

                var individualProviderByLocation = doctorCorporationContractLink
                    .First(d => d.DoctorId == doctor.DoctorId)
                    .ProvidersByLocations
                    .FirstOrDefault(loc => loc.PlaceOfServiceId == locationId);

                if (individualProviderByLocation != null)
                    doctor.IndividualProviderByLocation = individualProviderByLocation.LocacionProviderNumber ?? "N/A";
                else
                    doctor.IndividualProviderByLocation = "N/A";
            }
            return Json(doctors.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActiveDoctors([DataSourceRequest] DataSourceRequest request)
        {
            var result = _unitOfWork
                .Doctors
                .GetAllActiveDoctors()
                .Select(doctor => new
                {
                    DoctorId = doctor.DoctorId,
                    FullName = $"{doctor.FirstName} {doctor.LastName}",
                    NpiNumber = doctor.NpiNumber
                })
                .ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}