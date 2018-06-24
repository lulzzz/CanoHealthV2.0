using CanoHealth.WebPortal.Core.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class OutofNetworkContractDto
    {
        public Guid OutOfNetworkContractId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid InsurnaceId { get; set; }

        [Required]
        public DateTime? EffectiveDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public OutOfNetworkContract Convert()
        {
            return new OutOfNetworkContract
            {
                OutOfNetworkContractId = OutOfNetworkContractId == Guid.Empty ? Guid.NewGuid() : OutOfNetworkContractId,
                DoctorId = DoctorId,
                InsurnaceId = InsurnaceId,
                EffectiveDate = EffectiveDate,
                ExpirationDate = ExpirationDate
            };
        }
    }
}