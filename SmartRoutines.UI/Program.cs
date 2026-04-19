using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Data;
using SmartRoutines.Logic;
using SmartRoutines.UI.Controls;
using SmartRoutines.UI.Controls.AddRoutine.UC_Step3;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDataServices(connectionString);
                    services.AddLogicServices();

                    // Forms & Pages
                    services.AddTransient<FrmMain>();
                    services.AddTransient<UC_Dashboard>();
                    services.AddTransient<UC_ActionsMain>();
                    services.AddTransient<UC_Settings>();
                    services.AddTransient<UC_LogsPage>();
                })
                .Build();

            var engine = host.Services.GetRequiredService<IAutomationEngine>();
            _ = engine.StartAsync();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var mainForm = services.GetRequiredService<FrmMain>();
                Application.Run(mainForm);
                engine.StopAsync();
            }
        }
    }
}