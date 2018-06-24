using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Core.Dtos;
using System;
using System.Linq;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class CorporationController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CorporationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetCorporations(string text = null)
        {
            var result = _unitOfWork.Corporations
                .GetActiveCorporations()
                .Select(Mapper.Map<Corporation, CorporationDto>)
                .ToList();
            if (!String.IsNullOrEmpty(text))
                result = result.Where(c => c.CorporationName.ToLower().Contains(text.ToLower())).ToList();
            return Ok(result);

        }
    }
}
