using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
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
            var businessLines = _unitOfWork.LineOfBusinesses.GetBusinessLines().ToList();
            if (insuranceId != null)
            {
                //get the business lines associated to a specific insurance
                var insuranceBusinessLines = _unitOfWork.InsuranceBusinessLineRepository
                                                        .GetBusinessLines(insuranceId.Value)
                                                        .Select(bl => bl.BusinessLine)
                                                        .Where(bl => bl.Active.HasValue && bl.Active.Value)
                                                        .ToList();
                //get the business lines that are not already associated to specific insurance
                businessLines = businessLines.Except(insuranceBusinessLines).ToList();
            }
            //Convert the busines lines entitities to view model
            var result = businessLines.Select(BusinessLineViewModel.Wrap).ToList();

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBusinessLinesJson(Guid? insuranceId)
        {
            //get all the business lines in the system
            var businessLines = _unitOfWork.LineOfBusinesses.GetBusinessLines().ToList();
            if (insuranceId != null)
            {
                //get the business lines associated to a specific insurance
                var insuranceBusinessLines = _unitOfWork.InsuranceBusinessLineRepository
                                                        .GetBusinessLines(insuranceId.Value)
                                                        .Select(bl => bl.BusinessLine)
                                                        .Where(bl => bl.Active.HasValue && bl.Active.Value)
                                                        .ToList();
                //get the business lines that are not already associated to specific insurance
                businessLines = businessLines.Except(insuranceBusinessLines).ToList();
            }
            //Convert the busines lines entitities to view model
            var result = businessLines.Select(BusinessLineViewModel.Wrap).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CreateLineofBusiness([DataSourceRequest] DataSourceRequest request, BusinessLineViewModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}