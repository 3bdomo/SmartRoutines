using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartRoutines.Data;
using SmartRoutines.Data.Context;
using SmartRoutines.Logic;

namespace SmartRoutines.TriggerDebugger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "SmartRoutines | End-to-End Test Orchestrator";
            Console.WriteLine("Initializing Host infrastructure...");

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = false;
                        options.SingleLine = true;
                        options.TimestampFormat = "[HH:mm:ss] ";
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    // 1. Base Setup (Registers UnitOfWork, Entities, ActionRunners, Engine singleton)
                    services.AddDataServices("Server=(localdb)\\mssqllocaldb;Database=BypassDB");
                    services.AddLogicServices();

                    // 2. Override Database configuration purely for isolated InMemory testing
                    services.AddDbContext<SmartRoutinesDbContext>(options =>
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString()), 
                        ServiceLifetime.Scoped, 
                        ServiceLifetime.Scoped);

                    // 3. Register our Testing Class
                    services.AddTransient<EngineIntegrationTest>();
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SmartRoutinesDbContext>();
                
                // Guarantee a clean memory DB state
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                
                // Execute Integration Test
                var tester = scope.ServiceProvider.GetRequiredService<EngineIntegrationTest>();
                await tester.RunTestAsync();
            }

            Console.WriteLine("\n[Host] Test execution finished. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
