using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.ViewModels
{
    public class PlaceOfServiceFormViewModel
    {
        public Guid? PlaceOfServiceId { get; set; }

        public Guid CorporationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [Phone]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Phone]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string FaxNumber { get; set; }

        public bool Active { get; set; }
    }
}