using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                    services.AddDataServices();
                    services.AddTransient<FrmMain>();
                })
                .Build();

            ApplicationConfiguration.Initialize();

            // var Settings = new UC_Settings();
            // Settings.Controls.Add(new UC_Settings());

            // Application.Run(new Form { Controls = { Settings } });

            var mainForm = host.Services.GetRequiredService<FrmMain>();
            Application.Run(mainForm);

        }
    }
}