using AutoMapper;
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
    [Authorize(Roles = "ADMIN,CREDENTIALING")]
    public class InsurancesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsurancesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        public ActionResult ReadInsurances([DataSourceRequest] DataSourceRequest request)
        {
            var insurances = _unitOfWork.Insurances
                .EnumarableGetAll(orderBy: ioq => ioq.OrderBy(i => i.Name))
                .Select(Mapper.Map<Insurance, InsuranceFormViewModel>)
                .ToList();
            return Json(insurances.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ReadActiveInsurances([DataSourceRequest] DataSourceRequest request)
        {
            var activeInsurances = _unitOfWork.Insurances
                .GetActiveInsurances()
                .Select(Mapper.Map<Insurance, InsuranceFormViewModel>)
                .ToList();
            return Json(activeInsurances.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAvailableInsurance(Guid? doctorId)
        {
            //get all insurances
            var insurances = _unitOfWork.Insurances.GetAll().ToList();

            if (doctorId.HasValue)
            {
                //get list of individual providers of specific doctor
                var individualProviders = _unitOfWork.IndividualProviderRepository
                    .GetIndividualProviders(doctorId)
                    .Select(i => i.Insurance).ToList();

                insurances = insurances.Except(individualProviders)
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            return Json(insurances.Select(x => new { InsuranceId = x.InsuranceId, Name = x.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateInsurance([DataSourceRequest] DataSourceRequest request,
            InsuranceFormViewModel insuranceViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existInsurance = _unitOfWork.Insurances.GetByName(insuranceViewModel.Name);
                    if (existInsurance != null)
                    {
                        ModelState.AddModelError("Name", "Duplicate Data. Please try again!");
                        return Json(new[] { insuranceViewModel }.ToDataSourceResult(request, ModelState));
                    }

                    existInsurance = _unitOfWork.Insurances.FirstOrDefault(ins => !String.IsNullOrEmpty(ins.Code) &&
                                    ins.Code.Equals(insuranceViewModel.Code, StringComparison.InvariantCultureIgnoreCase) &&
                                    ins.Active);
                    if (existInsurance != null)
                    {
                        ModelState.AddModelError("Code", "Duplicate Data. Please try again!");
                        return Json(new[] { insuranceViewModel }.ToDataSourceResult(request, ModelState));
                    }

                    insuranceViewModel.InsuranceId = Guid.NewGuid();
                    insuranceViewModel.Active = true;
                    var insurance = Mapper.Map(insuranceViewModel, new Insurance());

                    var lineofBusiness = insuranceViewModel.LineofBusiness.Select(item => new InsuranceBusinessLine
                    {
                        InsuranceBusinessLineId = Guid.NewGuid(),
                        InsuranceId = insurance.InsuranceId,
                        PlanTypeId = item.PlanTypeId,
                        Active = true
                    }).ToList();

                    var logs = _unitOfWork.InsuranceBusinessLineRepository.Save(lineofBusiness).ToList();

                    var auditlogs = _unitOfWork.Insurances.SaveItems(new List<Insurance> { insurance }).ToList();

                    auditlogs.AddRange(logs);

                    _unitOfWork.AuditLogs.AddRange(auditlogs);
                    _unitOfWork.Complete();
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again!");
                }
            }
            return Json(new[] { insuranceViewModel }.ToDataSourceResult(request,ModelState));
        }

        public ActionResult UpdateInsurance([DataSourceRequest] DataSourceRequest request,
            InsuranceFormViewModel insuranceViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var insuranceStoredInDb =
                        _unitOfWork.Insurances.GetWithContracts(insuranceViewModel.InsuranceId.Value);
                    if (insuranceStoredInDb == null)
                    {
                        ModelState.AddModelError("", "Not found.");
                        return Json(new[] { insuranceViewModel }.ToDataSourceResult(request, ModelState));
                    }
                    var otherInsuranceFound =
                        _unitOfWork.Insurances.GetOtherInsuranceWithSameName(insuranceViewModel.Name,
                            insuranceViewModel.InsuranceId.Value);
                    if (otherInsuranceFound != null)
                    {
                        ModelState.AddModelError("Name", "Duplicate Data. Please try again!");
                        return Json(new[] { insuranceViewModel }.ToDataSourceResult(request, ModelState));
                    }
                    var auditLogs = insuranceStoredInDb.ModifyInsurance(Mapper.Map(insuranceViewModel, new Insurance()));
                    _unitOfWork.AuditLogs.AddRange(auditLogs);
                    _unitOfWork.Complete();
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again!");
                }
            }
            return Json(new[] { insuranceViewModel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Index()
        {
            return View("Insurances");
        }
    }
}