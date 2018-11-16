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
                    ModelState.AddModelError("", @"We are sorry, but something went wrong. Please try again.");
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

                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("","We are sorry, but something went wrong. Please try again!");
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