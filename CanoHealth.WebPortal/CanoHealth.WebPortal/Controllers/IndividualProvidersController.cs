using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
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
    public class IndividualProvidersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndividualProvidersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult GetIndividualProviders(Guid? doctorId)
        {
            var individualProviders = _unitOfWork.IndividualProviderRepository
                                        .GetIndividualProviders(doctorId)
                                        .Select(IndividualProviderFormViewModel.Wrap)
                                        .ToList();
            return Json(individualProviders, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveIndividualProvider([DataSourceRequest] DataSourceRequest request,
            IndividualProviderFormViewModel individualProvider, Guid? parentGridDoctorId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (parentGridDoctorId.HasValue)
                        individualProvider.DoctorId = parentGridDoctorId.Value;

                    var doctorIndividualProvider = individualProvider.Convert();

                    var individualProviderWithSameNumber = _unitOfWork.IndividualProviderRepository.GetIndividualProviderByProviderNumber(doctorIndividualProvider.DoctorIndividualProviderId, individualProvider.ProviderNumber);
                    if (individualProviderWithSameNumber != null)
                    {
                        ModelState.AddModelError("ProviderNumber", @"Duplicate Provider Number. Please try again.");
                        return Json(new[] { individualProvider }.ToDataSourceResult(request, ModelState));
                    }

                    var duplicateIndividualProvider = _unitOfWork.IndividualProviderRepository
                        .GetIndividualProviderByDoctorAndInsurance(doctorIndividualProvider);

                    if (duplicateIndividualProvider != null)
                    {
                        ModelState.AddModelError("", @"Duplicate individual Provider. Please try again!");
                        return Json(new[] { individualProvider }.ToDataSourceResult(request, ModelState));
                    }

                    var logs = _unitOfWork.IndividualProviderRepository
                        .SaveIndividualProviders(new List<DoctorIndividualProvider> { doctorIndividualProvider });

                    _unitOfWork.AuditLogs.AddRange(logs);

                    individualProvider.InsuranceName = _unitOfWork.Insurances.GetInsuranceById(individualProvider.InsuranceId).Name;

                    _unitOfWork.Complete();

                    individualProvider.DoctorIndividualProviderId = doctorIndividualProvider.DoctorIndividualProviderId;
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("", @"We are sorry, but something went wrong. Please try again.");
                }
            }
            return Json(new[] { individualProvider }.ToDataSourceResult(request, ModelState));
        }
    }
}