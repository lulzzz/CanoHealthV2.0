using System.ServiceProcess;

namespace CanoHealth.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ExpirationDateService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
