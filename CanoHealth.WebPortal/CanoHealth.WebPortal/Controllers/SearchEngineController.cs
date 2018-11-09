using CanoHealth.WebPortal.Core;
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
            var doctors = new List<SearchDoctorResultViewModel>();
            try
            {
                //Get the current list of doctors who work in this location
                var activeDoctorByLocation = _unitOfWork.ClinicDoctor
                                            .GetListOfDoctorsThatWorkInThisPlaceOfServiceToday(locationId)
                                            .ToList();

                //get the list of doctors that are already linked to this contract.
                var doctorCorporationContractLink = _unitOfWork.DoctorLinkedContracts
                                    .GetDoctorsLinkedToLineOfBusiness(contractLineofBusinessId)
                                    .ToList();

                //get the active doctors
                var activeLinkedDoctors = doctorCorporationContractLink
                                    .Select(d => d.Doctor)
                                    .Where(d => d.Active)
                                    .ToList();

                //Get the list of doctors who work in this location but also are linked to this contract
                doctors = activeDoctorByLocation.Intersect(activeLinkedDoctors)
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
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again!");
            }
            return Json(doctors.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActiveDoctors([DataSourceRequest] DataSourceRequest request)
        {
            var doctors = new List<DoctorDto>();
            try
            {
                doctors = _unitOfWork
                         .Doctors
                         .GetAllActiveDoctors()
                         .Select(doctor => new DoctorDto
                         {
                             DoctorId = doctor.DoctorId,
                             FullName = $"{doctor.FirstName} {doctor.LastName}",
                             NpiNumber = doctor.NpiNumber
                         })
                         .ToList();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again!");
            }            
            return Json(doctors.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}