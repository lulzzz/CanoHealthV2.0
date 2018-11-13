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
    public class AddendumsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddendumsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetAddendums(string contractId = null)
        {
            try
            {
                var result = _unitOfWork.Addendums.GetActiveAddendums(contractId)
                    .Select(Mapper.Map<ContractAddendum, ContractAddendumDto>)
                    .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        public IHttpActionResult InactiveAddendum(ContractAddendumDto addendumDto)
        {
            try
            {
                var addendumStoredInDb = _unitOfWork.Addendums.Get(addendumDto.ContractAddendumId);

                if (addendumStoredInDb == null)
                    return NotFound();
                var auditLog = addendumStoredInDb.InactiveContractAddendum();

                _unitOfWork.AuditLogs.Add(auditLog);

                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok();
        }
    }
}
