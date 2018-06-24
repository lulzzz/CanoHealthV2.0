using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class ErrorResponseDto
    {
        public string ErrorResponse { get; set; }
        public Object DomainModel { get; set; }
    }
}