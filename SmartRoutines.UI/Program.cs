using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project_2.Forms;
using SmartRoutines.Data;
using SmartRoutines.UI.Forms;

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
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Register services and dependencies here
                    services.AddLogicServices();
                    services.AddDataServices();
                    services.AddTransient<FrmMain>();
                })
                .Build();

            ApplicationConfiguration.Initialize();



        }
    }
}