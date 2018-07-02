using System;

namespace CanoHealth.WindowsService.Infrastructure
{
    public class Scheduler
    {
        public ScheduleInterval _interval = ScheduleInterval.EveryDay;
        public string _timeString = "10";
        public DayOfWeek _dayOfWeek = DayOfWeek.Thursday;
        public int _dayOfMonth;

        public double GetNextInterval()
        {

            int hours = Convert.ToInt32(_timeString);
            DateTime t = DateTime.Now.Date.Add(TimeSpan.FromHours(hours));
            //
            TimeSpan ts = new TimeSpan();
            int x;

            switch (_interval)
            {
                case ScheduleInterval.EveryDay:

                    ts = t - DateTime.Now;
                    if (ts.TotalMilliseconds < 0)
                    {
                        ts = t.AddDays(1) - DateTime.Now;
                    }
                    break;

                case ScheduleInterval.EveryMonth:
                    int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    if (DateTime.Now.Day > _dayOfMonth)
                    {
                        t = t.AddDays((daysInMonth - DateTime.Now.Day) + _dayOfMonth);
                    }
                    else if (_dayOfMonth == DateTime.Now.Day)
                    {
                        if (t < DateTime.Now)
                        {
                            t = t.AddDays(daysInMonth);
                        }
                    }
                    else
                    {
                        x = _dayOfMonth - DateTime.Now.Day;
                        t = t.AddDays(x);
                    }

                    ts = (TimeSpan)(t - DateTime.Now);
                    break;
                case ScheduleInterval.Every4Week:
                    t = t.AddDays(28);
                    ts = (TimeSpan)(t - DateTime.Now);

                    break;

                case ScheduleInterval.EveryWeek:
                    if (DateTime.Now.DayOfWeek > _dayOfWeek)
                    {
                        x = DateTime.Now.DayOfWeek - _dayOfWeek;
                        t = t.AddDays(7 - x);

                    }
                    else if (DateTime.Now.DayOfWeek == _dayOfWeek)
                    {
                        if (t < DateTime.Now)
                        {
                            t = t.AddDays(7);
                        }
                    }
                    else
                    {
                        x = _dayOfWeek - DateTime.Now.DayOfWeek;
                        t = t.AddDays(x);
                    }

                    ts = (TimeSpan)(t - DateTime.Now);
                    break;
            }

            return ts.TotalMilliseconds;
        }
    }
}
