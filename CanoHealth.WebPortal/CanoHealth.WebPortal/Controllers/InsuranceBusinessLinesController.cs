using CanoHealth.WebPortal.CommonTools.ExtensionMethods;
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
    public class InsuranceBusinessLinesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsuranceBusinessLinesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult GetBusinessLineByInsurance([DataSourceRequest] DataSourceRequest request, Guid insuranceId)
        {
            var insuranceBusinessLines = _unitOfWork.InsuranceBusinessLineRepository
                .GetBusinessLines(insuranceId)
                .Select(MapToViewModel())
                .ToList();
            return Json(insuranceBusinessLines.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("Post")]
        public ActionResult SaveInsuranceBusinessLines([DataSourceRequest] DataSourceRequest request,
            IEnumerable<InsuranceBusinessLineViewModel> insuranceBusinessLineViewModels)
        {
            if (insuranceBusinessLineViewModels != null && ModelState.IsValid)
            {
                try
                {
                    var insuranceBusinessLines = insuranceBusinessLineViewModels.ConvertToInsuranceBusinessLineEntity()
                        .ToList();

                    var logs = _unitOfWork.InsuranceBusinessLineRepository.Save(insuranceBusinessLines);

                    _unitOfWork.AuditLogs.AddRange(logs);

                    foreach (var item in insuranceBusinessLines)
                    {
                        insuranceBusinessLineViewModels.First(x => x.InsuranceId == item.InsuranceId &&
                                                          x.PlanTypeId == item.PlanTypeId)
                            .InsuranceBusinessLineId = item.InsuranceBusinessLineId;
                    }

                    _unitOfWork.Complete();
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    ModelState.AddModelError("", @"We are sorry, but something went wrong. Please try again!");
                }
            }

            return Json(insuranceBusinessLineViewModels.ToDataSourceResult(request, ModelState));
        }

        public ActionResult InactivateInsuranceLineofBusinessRelation([DataSourceRequest] DataSourceRequest request, InsuranceBusinessLineViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //get the record from DB
                    var insuranceLineofbusinessStoredIndb = _unitOfWork.InsuranceBusinessLineRepository
                        .Get(viewModel.InsuranceBusinessLineId);
                    //check if the record exist and it is active
                    if (insuranceLineofbusinessStoredIndb == null || !insuranceLineofbusinessStoredIndb.Active.HasValue ||
                        (insuranceLineofbusinessStoredIndb.Active.HasValue && !insuranceLineofbusinessStoredIndb.Active.Value))
                    {
                        ModelState.AddModelError("", "This record is not in our system. Please try again!");
                        return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
                    }
                    var auditLogs = new List<AuditLog>();

                    //get the active doctor's providerbylocation items associated to this insurancelineofbusiness record and inactivate them
                    var providerbyLocation = _unitOfWork.ProviderByLocationRepository
                        .ProviderByLocationsAndLineofbusiness(viewModel.InsuranceId, viewModel.PlanTypeId)
                        .ToList();
                    var inactivateProviderLogs = providerbyLocation.ConvertAll(p => p.Inactivate()).ToList();
                    auditLogs.AddRange(inactivateProviderLogs);

                    //get the active doctorcorporationcontractlink items associated to this insurancelineofbusiness record and inactivate them
                    var docLinkedContracts = _unitOfWork.DoctorLinkedContracts
                        .DoctorCorporationContractLinksByLineofbusiness(viewModel.InsuranceId, viewModel.PlanTypeId)
                        .ToList();
                    var inactivateLinkedContractsLogs = docLinkedContracts
                        .ConvertAll(x => x.InactiveDoctorCorporationContractLinkRecord())
                        .ToList();
                    auditLogs.AddRange(inactivateLinkedContractsLogs);

                    //get the active cliniclineofbusinesscontracts items associated to this insurancelineofbusiness record and inactivate them
                    var contractLineofbusinessLocations = _unitOfWork.ContracBusinessLineClinicRepository
                        .GetContractLineofBusinessLocations(viewModel.InsuranceId, viewModel.PlanTypeId)
                        .ToList();
                    var locLinkedContractLogs = contractLineofbusinessLocations.ConvertAll(x => x.InactivateClinicLineofBusinessContract()).ToList();
                    auditLogs.AddRange(locLinkedContractLogs);

                    //get the active contractlineofbusiness items associated to this insurancelineofbusiness record and inactivate them
                    var contractLineofbusiness = _unitOfWork.ContracBusinessLineRepository
                        .ContractLineofBusinesses(viewModel.InsuranceId, viewModel.PlanTypeId)
                        .ToList();
                    var contractLineofbusinesslogs = contractLineofbusiness.ConvertAll(x => x.InactivateContractLineofBusinessRecord()).ToList();
                    auditLogs.AddRange(contractLineofbusinesslogs);

                    //inactivate the insurancelineofbusiness record
                    var insuranceLineodbusinessLog = insuranceLineofbusinessStoredIndb.InactivateInsuranceLineofBusinessRelation();
                    auditLogs.Add(insuranceLineodbusinessLog);

                    //Store the logs
                    _unitOfWork.AuditLogs.AddRange(auditLogs);

                    //commit the changes to database
                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again!");
            }
            return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
        }

        private Func<InsuranceBusinessLine, InsuranceBusinessLineViewModel> MapToViewModel()
        {
            return x => new InsuranceBusinessLineViewModel
            {
                InsuranceBusinessLineId = x.InsuranceBusinessLineId,
                InsuranceId = x.InsuranceId,
                PlanTypeId = x.PlanTypeId,
                Code = x.BusinessLine.Code,
                Name = x.BusinessLine.Name,
                Active = x.Active
            };
        }
    }
}