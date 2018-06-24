using CanoHealth.WebPortal.CommonTools.ExtensionMethods;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class InsuranceBusinessLinesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsuranceBusinessLinesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public IHttpActionResult SaveInsuranceBusinessLines(IEnumerable<InsuranceBusinessLineViewModel> insuranceBusinessLineViewModels)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var insuranceBusinessLines = insuranceBusinessLineViewModels.ConvertToInsuranceBusinessLineEntity()
                    .ToList();

                var logs = _unitOfWork.InsuranceBusinessLineRepository.Save(insuranceBusinessLines);

                _unitOfWork.AuditLogs.AddRange(logs);

                _unitOfWork.Complete();

                foreach (var item in insuranceBusinessLines)
                {
                    insuranceBusinessLineViewModels.First(x => x.InsuranceId == item.InsuranceId &&
                                                               x.PlanTypeId == item.PlanTypeId)
                        .InsuranceBusinessLineId = item.InsuranceBusinessLineId;
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }

            return Ok(insuranceBusinessLineViewModels);
        }
    }
}
