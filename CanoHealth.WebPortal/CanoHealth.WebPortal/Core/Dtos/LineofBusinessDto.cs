using System;

namespace CanoHealth.WebPortal.Core.Dtos
{
    public class LineofBusinessDto
    {
        public Guid PlanTypeId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}