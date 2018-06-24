using System;

namespace CanoHealth.WebPortal.CommonTools.CurrentDateTime
{
    public class CurrentDateTimeService : ICurrentDateTimeService
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}