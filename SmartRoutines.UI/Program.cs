using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Data;
using SmartRoutines.Data.Context;
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
                })
                .Build();

            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // --- Automatic Database Migration ---
                try
                {
                    var context = services.GetRequiredService<SmartRoutinesDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    ShowErrorDialog(ex, "Database Migration");
                    return; // Stop app if database cannot be initialized
                }

                var engine = services.GetRequiredService<IAutomationEngine>();
                var mainForm = services.GetRequiredService<FrmMain>();
                
                // Start the engine
                engine.StartAsync().GetAwaiter().GetResult();

                Application.Run(mainForm);
                
                try { engine.StopAsync().GetAwaiter().GetResult(); } catch { }
            }
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