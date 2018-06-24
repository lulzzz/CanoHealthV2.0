using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class BusinessLinesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BusinessLinesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult GetBusinessLines([DataSourceRequest] DataSourceRequest request, Guid? insuranceId)
        {
            //get all the business lines in the system
            var businessLines = _unitOfWork.LineOfBusinesses.GetBusinessLines();
            if (insuranceId != null)
            {
                //get the business lines associated to a specific insurance
                var insuranceBusinessLines = _unitOfWork.InsuranceBusinessLineRepository
                                                        .GetBusinessLines(insuranceId.Value)
                                                        .Select(bl => bl.BusinessLine);
                //get the business lines that are not already associated to specific insurance
                businessLines = businessLines.Except(insuranceBusinessLines);
            }
            //Convert the busines lines entitities to view model
            var result = businessLines.Select(BusinessLineViewModel.Wrap).ToList();

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBusinessLinesJson(Guid? insuranceId)
        {
            //get all the business lines in the system
            var businessLines = _unitOfWork.LineOfBusinesses.GetBusinessLines();
            if (insuranceId != null)
            {
                //get the business lines associated to a specific insurance
                var insuranceBusinessLines = _unitOfWork.InsuranceBusinessLineRepository
                                                        .GetBusinessLines(insuranceId.Value)
                                                        .Select(bl => bl.BusinessLine);
                //get the business lines that are not already associated to specific insurance
                businessLines = businessLines.Except(insuranceBusinessLines);
            }
            //Convert the busines lines entitities to view model
            var result = businessLines.Select(BusinessLineViewModel.Wrap).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}