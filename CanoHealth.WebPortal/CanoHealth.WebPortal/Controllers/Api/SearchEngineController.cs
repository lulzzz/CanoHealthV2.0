using CanoHealth.WebPortal.Core;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class SearchEngineController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchEngineController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //When just Corporation and Insurance are set
        [HttpGet]
        public IHttpActionResult GetResultByCorporationAndInsurance(
            Guid corporationId, Guid insuranceId)
        {
            var corporation = _unitOfWork.Corporations.Get(corporationId);
            if (corporation == null)
                return Content(HttpStatusCode.NotFound, "Corporation not found.");

            var insurance = _unitOfWork.Insurances.Get(insuranceId);
            if (insurance == null)
                return Content(HttpStatusCode.NotFound, "Insurance not found.");

            var contract = _unitOfWork.Contracts
                .GetContractByCorporationAndInsurance(corporationId, insuranceId);
            if (contract == null)
                return Content(HttpStatusCode.NotFound, "Contract not found.");

            var result = contract.ContractBusinessLines.Select(x => new
            {
                insuranceId,
                contract.GroupNumber,
                x.ContractLineofBusinessId,
                x.LineOfBusiness.Code,
                x.LineOfBusiness.Name
            });

            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetSearchResultByInsuranceLocationAndDoctorSql(
            Guid corporationId, Guid insuranceId, Guid locationId, Guid doctorId)
        {
            var corporation = _unitOfWork.Corporations.Get(corporationId);
            if (corporation == null)
                return Content(HttpStatusCode.NotFound, "Corporation not found.");

            var insurance = _unitOfWork.Insurances.Get(insuranceId);
            if (insurance == null)
                return Content(HttpStatusCode.NotFound, "Insurance not found.");

            var location = _unitOfWork.PlaceOfServices.Get(locationId);
            if (location == null)
                return Content(HttpStatusCode.NotFound, "Location not found.");

            var doctor = _unitOfWork.Doctors.Get(doctorId);
            if (doctor == null)
                return Content(HttpStatusCode.NotFound, "Doctor not found.");

            var contract = _unitOfWork.Contracts
                .SingleOrDefault(c => c.CorporationId == corporationId && c.InsuranceId == insuranceId);
            if (contract == null)
                return Content(HttpStatusCode.NotFound, "Contract not found.");

            var result = _unitOfWork.SearchCredentialsRepository
                .GetSearchResultsByInsuranceAndDoctor(corporationId, insuranceId, doctorId)
                .Where(r => r.PlaceOfServiceId == locationId);

            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetSearchResultByInsuranceLocationAndDoctor(
            Guid corporationId, Guid insuranceId, Guid locationId, Guid doctorId)
        {
            var corporation = _unitOfWork.Corporations.Get(corporationId);
            if (corporation == null)
                return Content(HttpStatusCode.NotFound, "Corporation not found.");

            var insurance = _unitOfWork.Insurances.Get(insuranceId);
            if (insurance == null)
                return Content(HttpStatusCode.NotFound, "Insurance not found.");

            var location = _unitOfWork.PlaceOfServices.GetPlaceOfService(locationId);
            if (location == null)
                return Content(HttpStatusCode.NotFound, "Location not found.");

            var doctor = _unitOfWork.Doctors.Get(doctorId);
            if (doctor == null)
                return Content(HttpStatusCode.NotFound, "Doctor not found.");

            var contract = _unitOfWork.Contracts
                .GetContractByCorporationAndInsurance(corporationId, insuranceId);
            if (contract == null)
                return Content(HttpStatusCode.NotFound, "Contract not found.");

            //ContractLineOfBusiness --> ClinicLineOfBusinessContracts --> PlaceOfService
            var locationLinkedContract = (from a in contract.ContractBusinessLines
                                          join b in _unitOfWork.ContracBusinessLineClinicRepository.EnumarableGetAll()
                                              on a.ContractLineofBusinessId equals b.ContractLineofBusinessId
                                          join c in _unitOfWork.PlaceOfServices.EnumarableGetAll()
                                              on b.PlaceOfServiceId equals c.PlaceOfServiceId
                                          where c.PlaceOfServiceId == locationId && c.Active
                                          select a).ToList();
            if (!locationLinkedContract.Any())
                return Content(HttpStatusCode.NotFound, "This location is not linked to the contract.");

            //ContractLineOfBusiness --> DoctorCorporationContractLinks --> Doctors
            var doctorLinkedContract = (from a in contract.ContractBusinessLines
                                        join b in _unitOfWork.DoctorLinkedContracts.EnumarableGetAll()
                                            on a.ContractLineofBusinessId equals b.ContractLineofBusinessId
                                        join c in _unitOfWork.Doctors.EnumarableGetAll()
                                            on b.DoctorId equals c.DoctorId
                                        where c.DoctorId == doctorId && c.Active
                                        select b).ToList();
            if (!doctorLinkedContract.Any())
                return Content(HttpStatusCode.NotFound, "This doctor is not linked to the contract.");

            var lineOfBusiness = (from a in doctorLinkedContract
                                  join b in contract.ContractBusinessLines
                                    on a.ContractLineofBusinessId equals b.ContractLineofBusinessId
                                  join c in _unitOfWork.LineOfBusinesses.EnumarableGetAll()
                                    on b.PlanTypeId equals c.PlanTypeId
                                  select new
                                  {
                                      c.Code,
                                      c.Name,
                                      a.EffectiveDate,
                                      a.Note
                                  }).ToList();

            return Ok(lineOfBusiness);
        }

        [HttpGet]
        public IHttpActionResult GetSearchResultByInsuranceLocation(
            Guid corporationId, Guid insuranceId, Guid locationId)
        {
            var corporation = _unitOfWork.Corporations.Get(corporationId);
            if (corporation == null)
                return Content(HttpStatusCode.NotFound, "Corporation not found.");

            var insurance = _unitOfWork.Insurances.Get(insuranceId);
            if (insurance == null)
                return Content(HttpStatusCode.NotFound, "Insurance not found.");

            var location = _unitOfWork.PlaceOfServices
                .Get(locationId);
            if (location == null)
                return Content(HttpStatusCode.NotFound, "Location not found.");

            var contract = _unitOfWork.Contracts
                .SingleOrDefault(c => c.CorporationId == corporationId && c.InsuranceId == insuranceId);
            if (contract == null)
                return Content(HttpStatusCode.NotFound, "Contract not found.");

            var result = _unitOfWork.SearchCredentialsRepository
                .GetSearchResultsByInsuranceAndLocation(corporationId, insuranceId, locationId);

            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetSearchByInsuranceDoctor(
            Guid corporationId, Guid insuranceId, Guid doctorId)
        {
            var corporation = _unitOfWork.Corporations.Get(corporationId);
            if (corporation == null)
                return Content(HttpStatusCode.NotFound, "Corporation not found.");

            var insurance = _unitOfWork.Insurances.Get(insuranceId);
            if (insurance == null)
                return Content(HttpStatusCode.NotFound, "Insurance not found.");

            var doctor = _unitOfWork.Doctors.Get(doctorId);
            if (doctor == null)
                return Content(HttpStatusCode.NotFound, "Doctor not found.");

            var contract = _unitOfWork.Contracts
               .SingleOrDefault(c => c.CorporationId == corporationId && c.InsuranceId == insuranceId);
            if (contract == null)
                return Content(HttpStatusCode.NotFound, "Contract not found.");

            var result = _unitOfWork.SearchCredentialsRepository
                .GetSearchResultsByInsuranceAndDoctor(corporationId, insuranceId, doctorId);

            return Ok(result);
        }
    }
}