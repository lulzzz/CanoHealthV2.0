using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
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
    }
}
