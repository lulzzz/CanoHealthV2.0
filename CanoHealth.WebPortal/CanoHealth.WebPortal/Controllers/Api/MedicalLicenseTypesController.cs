using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class MedicalLicenseTypesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicalLicenseTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetMedicalLicenseTypes()
        {
            var result = _unitOfWork.MedicalLicenseTypes.GetAllOrderByClassification()
                .Select(Mapper.Map<MedicalLicenseType, MedicalLicenseTypeDto>)
                .ToList();
            return Ok(result);
        }
    }
}
