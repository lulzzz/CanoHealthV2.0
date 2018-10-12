using System;
using System.Collections.Generic;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class ScheduleDto
    {
        public Guid ScheduleId { get; set; }

        public DateTime Start { get; set; } //StartDateTime

        public DateTime End { get; set; }

        public string Title { get; set; }

        public String Description { get; set; }

        public Guid PlaceOfServiceId { get; set; }

        public string Location { get; set; }

        public List<DoctorDto> Doctors { get; set; }

        public ScheduleDto()
        {
            Doctors = new List<DoctorDto>();
        }
    }
}