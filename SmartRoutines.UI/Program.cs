using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Data;
using SmartRoutines.Data.Context;
using SmartRoutines.Logic;
using SmartRoutines.UI.Controls;
using SmartRoutines.UI.Controls.AddRoutine.UC_Step3;
using SmartRoutines.UI.Forms;
using System.Text;

namespace SmartRoutines.UI
{
    internal class Program
    {
        private static ILogger<Program>? _logger;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // 1. UI Thread Exceptions
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) =>
            {
                HandleException(e.Exception, "UI Thread Exception");
            };

            // 2. Non-UI Thread / Background Exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                HandleException(ex, "FATAL Non-UI Thread Exception");
            };

            // 3. Unobserved Task Exceptions
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                HandleException(e.Exception, "Unobserved Task Exception");
                e.SetObserved(); // Prevent process crash
            };

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    services.AddDataServices(connectionString);
                    services.AddLogicServices();
                    services.AddTransient<FrmMain>();
                    services.AddTransient<UC_Dashboard>();
                    services.AddTransient<UC_ActionsMain>();
                    services.AddTransient<UC_Settings>();
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                
                // 4. Logging
                _logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    var context = services.GetRequiredService<SmartRoutinesDbContext>();
                    var pendingMigrations = context.Database.GetPendingMigrations();
                    if (pendingMigrations.Any())
                        context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    HandleException(ex, "Database Migration");
                    return;
                }

                var engine = services.GetRequiredService<IAutomationEngine>();
                var mainForm = services.GetRequiredService<FrmMain>();
                
                engine.StartAsync().GetAwaiter().GetResult();
                Application.Run(mainForm);
                
                try { engine.StopAsync().GetAwaiter().GetResult(); } catch { }
            }
        }

        private static void HandleException(Exception? ex, string source)
        {
            // Log the exception 
            _logger?.LogError(ex, "[{Source}] An unhandled exception occurred.", source);

            // Show error to user via the thread-safe dialog
            ShowErrorDialogSafe(ex, source);
        }

        private static void ShowErrorDialogSafe(Exception? ex, string source)
        {
            var exceptionDetails = GetFullExceptionDetails(ex);
            var message = $"[{source}]\n\n{exceptionDetails}";

            // 6. Thread Safety for ShowErrorDialog
            if (Application.OpenForms.Count > 0)
            {
                var mainForm = Application.OpenForms[0];
                if (mainForm.InvokeRequired)
                {
                    mainForm.Invoke(() => SmartDialog.Show(null, "Application Error", message, DialogIconType.Error));
                }
                else
                {
                    SmartDialog.Show(null, "Application Error", message, DialogIconType.Error);
                }
            }
            else
            {
                SmartDialog.Show(null, "Application Error", message, DialogIconType.Error);
            }
        }

        // 5. GetFullExceptionDetails recursively
        private static string GetFullExceptionDetails(Exception? ex)
        {
            if (ex == null)
            {
                return "An unknown error occurred.\nNo stack trace available.";
            }

            var sb = new StringBuilder();
            BuildExceptionDetails(ex, sb, 0);
            return sb.ToString();
        }

        private static void BuildExceptionDetails(Exception ex, StringBuilder sb, int level)
        {
            string indent = new string(' ', level * 2);
            
            sb.AppendLine($"{indent}Message: {ex.Message}");
            sb.AppendLine($"{indent}Type: {ex.GetType().FullName}");
            
            if (!string.IsNullOrWhiteSpace(ex.StackTrace))
            {
                sb.AppendLine($"{indent}Stack Trace:");
                sb.AppendLine(ex.StackTrace);
            }
            sb.AppendLine();

            if (ex is AggregateException aggEx)
            {
                foreach (var inner in aggEx.InnerExceptions)
                {
                    sb.AppendLine($"{indent}--- Inner Exception ---");
                    BuildExceptionDetails(inner, sb, level + 1);
                }
            }
            else if (ex.InnerException != null)
            {
                sb.AppendLine($"{indent}--- Inner Exception ---");
                BuildExceptionDetails(ex.InnerException, sb, level + 1);
            }
        }
    }
}