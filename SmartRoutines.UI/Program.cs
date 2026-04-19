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
                })
                .Build();

            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            // Global exception handling
            Application.ThreadException += (s, e) => ShowErrorDialog(e.Exception, "UI Thread Exception");
            AppDomain.CurrentDomain.UnhandledException += (s, e) => ShowErrorDialog(e.ExceptionObject as Exception, "Critical Domain Exception");

            var mainForm = host.Services.GetRequiredService<FrmMain>();
            Application.Run(mainForm);
        }

        private static void ShowErrorDialog(Exception? ex, string source)
        {
            string message = ex?.Message ?? "An unknown error occurred.";
            string stackTrace = ex?.StackTrace ?? "No stack trace available.";
            
            MessageBox.Show($"[SmartRoutines - {source}]\n\n{message}\n\nStack Trace:\n{stackTrace.Substring(0, Math.Min(stackTrace.Length, 500))}...", 
                "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}