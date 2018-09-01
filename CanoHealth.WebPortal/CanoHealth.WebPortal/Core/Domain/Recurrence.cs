using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    /*to detect the recurrence of an schedule. Ex: Never, Daily, Weekly, Yearly*/
    public class Recurrence
    {
        public Guid RecurrenceId { get; set; }

        [StringLength(50)]
        public string RecurrenceName { get; set; }

        public bool Active { get; set; }
    }
}