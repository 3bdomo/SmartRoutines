using SmartRoutines.UI.Forms;
using SmartRoutines.Data.Repositories;
using SmartRoutines.Logic;
using System.Threading;

namespace SmartRoutines.UI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ApplicationConfiguration.Initialize();

            // Wire dependencies
            var routineRepo = new MockRoutineRepository();
            var logRepo = new MockLogRepository();
            var backgroundMonitor = new BackgroundMonitorService(routineRepo, logRepo);

            Application.Run(new Form1());
        }
    }
}