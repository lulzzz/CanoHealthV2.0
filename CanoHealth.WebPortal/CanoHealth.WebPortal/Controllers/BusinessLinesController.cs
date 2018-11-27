using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
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

        public ActionResult CreateLineofBusiness([DataSourceRequest] DataSourceRequest request, BusinessLineViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var lineofBusinessStoredInDb = _unitOfWork.LineOfBusinesses.FirstOrDefault(lb => lb.Name.Equals(viewModel.Name, StringComparison.InvariantCultureIgnoreCase) &&
                         lb.Active.HasValue && lb.Active.Value);
                    if (lineofBusinessStoredInDb != null)
                    {
                        ModelState.AddModelError("Name", "Duplicate Line of Business. Please try again!");
                        return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
                    }

                    lineofBusinessStoredInDb = _unitOfWork.LineOfBusinesses.FirstOrDefault(lb => !String.IsNullOrEmpty(lb.Code) &&
                                    lb.Code.Equals(viewModel.Code, StringComparison.InvariantCultureIgnoreCase) &&
                                    lb.Active.HasValue && lb.Active.Value);
                    if (lineofBusinessStoredInDb != null)
                    {
                        ModelState.AddModelError("Code", "Duplicate Line of Business. Please try again!");
                        return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
                    }

                    viewModel.PlanTypeId = Guid.NewGuid();
                    viewModel.Active = true;
                    var lineofBusinessToCreate = Mapper.Map(viewModel, new PlanType());
                    _unitOfWork.LineOfBusinesses.Add(lineofBusinessToCreate);
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
    }
}