using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using System;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class BusinessLinesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public BusinessLinesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetBusinessLines()
        {
            var result = _unitOfWork.LineOfBusinesses.GetAll()
                .Select(Mapper.Map<PlanType, LineofBusinessDto>)
                .ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetAvailableBusinessLinesByContract(string contractId)
        {
            try
            {
                var allBusinessLines = _unitOfWork.LineOfBusinesses.GetAll().ToList();
                if (!String.IsNullOrEmpty(contractId))
                {
                    var guidContractId = Guid.Parse(contractId);
                    var contractWithBusinessLines = _unitOfWork.Contracts.GetContractWithBusinessLines(guidContractId);
                    if (contractWithBusinessLines != null)
                    {
                        var takenBusinessLines = contractWithBusinessLines.ContractBusinessLines
                            .Select(l => l.LineOfBusiness).ToList();
                        allBusinessLines = allBusinessLines.Except(takenBusinessLines).ToList();
                    }
                }
                var result = allBusinessLines.Select(Mapper.Map<PlanType, LineofBusinessDto>).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult CreateLineofBusiness(BusinessLineViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var lineofBusinessStoredInDb = _unitOfWork.LineOfBusinesses.FirstOrDefault(lb => lb.Name.Equals(viewModel.Name, StringComparison.InvariantCultureIgnoreCase) &&
                         lb.Active.HasValue && lb.Active.Value);
                if (lineofBusinessStoredInDb != null)
                {
                    ModelState.AddModelError("Name", "Duplicate Line of Business. Please try again!");
                    return BadRequest(ModelState);
                }

                lineofBusinessStoredInDb = _unitOfWork.LineOfBusinesses.FirstOrDefault(lb => !String.IsNullOrEmpty(lb.Code) &&
                                lb.Code.Equals(viewModel.Code, StringComparison.InvariantCultureIgnoreCase) &&
                                lb.Active.HasValue && lb.Active.Value);
                if (lineofBusinessStoredInDb != null)
                {
                    ModelState.AddModelError("Code", "Duplicate Line of Business. Please try again!");
                    return BadRequest(ModelState);
                }

                viewModel.PlanTypeId = Guid.NewGuid();
                viewModel.Active = true;
                var lineofBusinessToCreate = Mapper.Map(viewModel, new PlanType());
                _unitOfWork.LineOfBusinesses.Add(lineofBusinessToCreate);
                _unitOfWork.Complete();

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }

            return Ok(viewModel);
        }
    }
}
