using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SmartRoutines.Data;
using SmartRoutines.UI.Forms;
using SmartRoutines.UI.Core.Theme;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Logic.Services;
using SmartRoutines.UI.Controls;
using SmartRoutines.UI.Core.Helper;

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
                    
                    // Logic Services
                    services.AddScoped<IRoutineService, RoutineService>();
                    services.AddScoped<IDashboardService, DashboardService>();

                    // Forms & Pages
                    services.AddTransient<FrmMain>();
                    services.AddTransient<UC_Dashboard>();
                    services.AddTransient<UC_ActionsMain>();
                    services.AddTransient<UC_Settings>();
                })
                .Build();

            //ApplicationConfiguration.Initialize();

            //var frm = new FrmWizard();
            ////Settings.Controls.Add(new fr());

            //Application.Run(frm);

            var mainForm = host.Services.GetRequiredService<FrmMain>();
            Application.Run(mainForm);

        }
    }
}