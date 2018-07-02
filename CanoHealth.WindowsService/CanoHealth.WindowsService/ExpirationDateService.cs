using CanoHealth.WindowsService.Infrastructure;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace CanoHealth.WindowsService
{
    public partial class ExpirationDateService : ServiceBase
    {
        private int eventId = 1;
        private static System.Timers.Timer timer = new System.Timers.Timer();

        public ExpirationDateService()
        {
            InitializeComponent();

            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("CanoHealthSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "CanoHealthSource", "CanoHealthNewLog");
            }
            eventLog1.Source = "CanoHealthSource";
            eventLog1.Log = "CanoHealthNewLog";
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart Expiration Dates Window Service");

            // Set up a timer to trigger every minute.  
            //System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.  
            eventLog1.WriteEntry("Monitoring the System to detect licenses expiration dates.", EventLogEntryType.Information, eventId++);

            try
            {
                var scheduler = new Scheduler();
                double interval = (double)scheduler.GetNextInterval();
                timer.Interval = interval;
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Crit Error: " + ex.Message + Environment.NewLine + "Inner Exception: " + ex.InnerException + Environment.NewLine + Environment.NewLine + ex.ToString(), EventLogEntryType.Error, eventId++);
            }
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
        }
    }
}
