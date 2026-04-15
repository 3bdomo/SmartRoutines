using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartRoutines.Data;
using SmartRoutines.Logic;
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
                    // pass IConfiguration to data registration
                    services.AddDataServices();
                    services.AddTransient<FrmMain>();
                })
                .Build();

            // Initialize WinForms application configuration
            ApplicationConfiguration.Initialize();

            // Start the host and run the main form resolved from DI
            host.Start();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var mainForm = services.GetRequiredService<FrmMain>();
                Application.Run(mainForm);
            }

            // Stop the host gracefully when the form closes
            host.StopAsync().GetAwaiter().GetResult();
            host.Dispose();
        }
    }
}