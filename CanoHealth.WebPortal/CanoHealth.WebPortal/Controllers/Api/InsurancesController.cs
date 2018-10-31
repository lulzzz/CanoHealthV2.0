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
    public class InsurancesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsurancesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetInsurances()
        {
            var result = _unitOfWork.Insurances.GetActiveInsurances()
                .Select(Mapper.Map<Insurance, InsuranceDto>)
                .ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetAllInsurances()
        {
            var result = _unitOfWork
                .Insurances
                .EnumarableGetAll(orderBy: iqo => iqo.OrderBy(i => i.Name))
                .ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetInsurance(string insuranceName)
        {
            var insurance = _unitOfWork.Insurances.GetByName(insuranceName);
            if (insurance != null)
                return Ok(InsuranceWithContractsDto.Wrap(insurance));
            return Ok(new InsuranceWithContractsDto());
        }

        [HttpPut]
        public IHttpActionResult UpdateInsurance(InsuranceFormDto insurance)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var insuranceStoredInDb = _unitOfWork.Insurances.Get(insurance.InsuranceId);
                if (insuranceStoredInDb == null)
                    return NotFound();
                var logs = insuranceStoredInDb.ModifyInsurance(Mapper.Map(insurance, new Insurance()));
                _unitOfWork.AuditLogs.AddRange(logs);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(insurance);
        }

        [HttpDelete]
        public IHttpActionResult InactivateInsurance(InsuranceFormDto insurance)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //Get the insurance and all the active contracts associated to it.
                var insuranceStoredInDb = _unitOfWork.Insurances.GetWithContracts(insurance.InsuranceId);
                if (insuranceStoredInDb == null)
                    return NotFound();

                //Inactivate insurance and all the active contracts associated to it.
                var logs = insuranceStoredInDb.InactivateInsurance();

                _unitOfWork.AuditLogs.AddRange(logs);

                _unitOfWork.Complete();

                insurance.Active = false;

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(insurance);
        }
    }
}
