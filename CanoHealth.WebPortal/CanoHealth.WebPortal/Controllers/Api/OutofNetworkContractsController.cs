using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class OutofNetworkContractsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public OutofNetworkContractsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public IHttpActionResult SaveOutOfNetworkContract(OutofNetworkContractDto outofNetworkContractDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var insurance = _unitOfWork.Insurances.GetInsuranceById(outofNetworkContractDto.InsurnaceId);
                if (insurance == null)
                {
                    ModelState.AddModelError("InsurnaceId", "This insurance doesn't exist in our system. Please try again.");
                    return BadRequest(ModelState);
                }

                var doctor = _unitOfWork.Doctors.GetAllActiveDoctors()
                        .SingleOrDefault(d => d.DoctorId == outofNetworkContractDto.DoctorId);
                if (doctor == null)
                {
                    ModelState.AddModelError("DoctorId", "This doctor doesn't work with us. Please try again.");
                    return BadRequest(ModelState);
                }

                var outOfNetworkContract = outofNetworkContractDto.Convert();
                var logs = _unitOfWork.OutofNetworkContractRepository.SaveContracts(new List<OutOfNetworkContract> { outOfNetworkContract });
                _unitOfWork.AuditLogs.AddRange(logs);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(outofNetworkContractDto);
        }
    }
}
