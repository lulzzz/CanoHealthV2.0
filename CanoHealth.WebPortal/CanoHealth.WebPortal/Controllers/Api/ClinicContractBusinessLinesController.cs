using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class ClinicContractBusinessLinesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClinicContractBusinessLinesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpDelete]
        public IHttpActionResult ReleaseClinicContractBusinessLineItem(ClinicLineofBusinessContractDto clinicLineofBusinessContractDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var clinicLineofBusinessContract = _unitOfWork.ContracBusinessLineClinicRepository
                    .Get(clinicLineofBusinessContractDto.Id);

                if (clinicLineofBusinessContract == null)
                    return NotFound();

                var logs = clinicLineofBusinessContract.CreateReleaseLogs();

                _unitOfWork.ContracBusinessLineClinicRepository.Remove(clinicLineofBusinessContract);

                _unitOfWork.AuditLogs.AddRange(logs);

                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(clinicLineofBusinessContractDto);
        }
    }
}
