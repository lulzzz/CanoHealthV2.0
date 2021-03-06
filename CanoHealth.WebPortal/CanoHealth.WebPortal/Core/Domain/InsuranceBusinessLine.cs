﻿using System;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class InsuranceBusinessLine
    {
        public Guid InsuranceBusinessLineId { get; set; }

        public Guid InsuranceId { get; set; }

        public Guid PlanTypeId { get; set; }

        public bool? Active { get; set; }

        //Navegation Properties
        public Insurance Insurance { get; set; }

        public PlanType BusinessLine { get; set; }

        public AuditLog InactivateInsuranceLineofBusinessRelation()
        {
            Active = false;
            var log = AuditLog.AddLog("InsuranceBusinessLines", "Active", "true", "false", InsuranceBusinessLineId, "Update");
            return log;
        }
    }
}