using IdentitySample.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class UserCorporationAccess
    {
        [Key]
        public Guid AccessId { get; set; }

        public string UserId { get; set; }

        public Guid CorporationId { get; set; }

        //Navegation Properties
        public ApplicationUser User { get; set; }

        public Corporation Corporation { get; set; }
    }
}