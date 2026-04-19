using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Data;
using SmartRoutines.Logic;
using SmartRoutines.Logic.Services;
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
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Register services and dependencies here
                    services.AddDataServices(context.Configuration);

                    // Logic layer registrations (action executors, engine, loggers)
                    services.AddLogicServices();

                    // Logic Services (domain services)
                    services.AddScoped<IRoutineService, RoutineService>();
                    services.AddScoped<IDashboardService, DashboardService>();

                    // Forms & Pages
                    services.AddTransient<FrmMain>();
                    services.AddTransient<UC_Dashboard>();
                    services.AddTransient<UC_ActionsMain>();
                    services.AddTransient<UC_Settings>();
                    services.AddTransient<UC_LogsPage>();
                })
                .Build();

            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = host.Services.GetRequiredService<FrmMain>();
            Application.Run(mainForm);
        }
    }
}