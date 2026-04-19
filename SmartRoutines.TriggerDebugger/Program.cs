using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using SmartRoutines.Logic.TriggerMonitors;

namespace SmartRoutines.TriggerDebugger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "SmartRoutines | Standalone Trigger Debugger";
            Console.Clear();
            Console.WriteLine("=================================================");
            Console.WriteLine("   SmartRoutines External Trigger Debugger       ");
            Console.WriteLine("=================================================");
            Console.WriteLine("Use this tool to test all triggers independently.");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("--- CORE TRIGGERS ---");
                Console.WriteLine("1. Time Trigger (Daily schedule)");
                Console.WriteLine("2. WiFi Trigger (Network connectivity)");
                Console.WriteLine("3. Battery Trigger (Power states)");
                Console.WriteLine("4. Idle Trigger (User inactivity)");
                Console.WriteLine("\n--- APP & SYSTEM ---");
                Console.WriteLine("5. App Launched (e.g. chrome, notepad)");
                Console.WriteLine("6. Startup Trigger (Simulated)");
                Console.WriteLine("7. Shutdown Trigger (Simulated)");
                Console.WriteLine("8. File Changed Trigger (e.g. test.txt)");
                Console.WriteLine("\n0. Exit");
                Console.Write("\nChoice: ");

                var choice = Console.ReadLine();
                if (choice == "0") break;

                switch (choice)
                {
                    case "1": await TestTimeTrigger(); break;
                    case "2": await TestWiFiTrigger(); break;
                    case "3": await TestBatteryTrigger(); break;
                    case "4": await TestIdleTrigger(); break;
                    case "5": await TestAppLaunched(); break;
                    case "6": await TestStartup(); break;
                    case "7": await TestShutdown(); break;
                    case "8": await TestFileChanged(); break;
                    default: Console.WriteLine("Invalid choice."); break;
                }

                Console.WriteLine("\nTest finished. Press any key to return to menu...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static async Task TestTimeTrigger()
        {
            Console.Write("Enter target time (HH:mm, e.g. 14:30): ");
            string timeStr = Console.ReadLine() ?? "12:00";
            if (timeStr.Contains(":") && timeStr.Length == 4) timeStr = "0" + timeStr;
            
            string isoTime = $"2026-01-01T{timeStr}:00";
            
            // Helpful hint for AM/PM confusion
            if (DateTime.TryParse(isoTime, out var parsed))
            {
                if (DateTime.Now.TimeOfDay > parsed.TimeOfDay)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Hint: This time has already passed for today. Use 24h format (e.g. 23:12) if you meant PM.");
                    Console.ResetColor();
                }
            }

            string json = "{\"ScheduledTime\": \"" + isoTime + "\", \"RepeatDays\": 127}";
            
            var trigger = new TimeTrigger();
            trigger.Configure(json);

            Console.WriteLine($"\nMonitoring Time Trigger (Target: {timeStr})");
            Console.WriteLine("Press 'Q' to stop monitoring.");

            await RunTestLoop(trigger);
        }

        static async Task TestWiFiTrigger()
        {
            Console.Write("Enter target SSID (e.g. HaGeR): ");
            string ssid = Console.ReadLine() ?? "";
            
            string json = "{\"SsidName\": \"" + ssid + "\"}";
            var trigger = new WiFiTrigger();
            trigger.Configure(json);

            Console.WriteLine($"\nMonitoring WiFi (Target: {ssid})");
            Console.WriteLine("Press 'Q' to stop monitoring.");

            await RunTestLoop(trigger);
        }

        static async Task TestBatteryTrigger()
        {
            Console.Write("Enter threshold percentage (1-100): ");
            string pct = Console.ReadLine() ?? "20";
            
            string json = "{\"BatteryThreshold\": " + pct + "}";
            var trigger = new BatteryTrigger();
            trigger.Configure(json);

            Console.WriteLine($"\nMonitoring Battery (Fires below {pct}%)");
            Console.WriteLine("Press 'Q' to stop monitoring.");

            await RunTestLoop(trigger);
        }

        static async Task TestIdleTrigger()
        {
            Console.Write("Enter idle timeout in minutes (e.g. 5): ");
            string minutes = Console.ReadLine() ?? "1";
            
            string json = "{\"IdleMinutes\": " + minutes + "}";
            var trigger = new IdleTrigger();
            trigger.Configure(json);

            Console.WriteLine($"\nMonitoring Idle State (Target: {minutes}m)");
            Console.WriteLine("Press 'Q' to stop monitoring.");

            await RunTestLoop(trigger);
        }

        static async Task TestAppLaunched()
        {
            Console.Write("Enter app name (only name, e.g. chrome, notepad): ");
            string appName = Console.ReadLine() ?? "";
            
            string json = "{\"SsidName\": \"" + appName + "\"}";
            var trigger = new AppLaunchedTrigger();
            trigger.Configure(json);

            Console.WriteLine($"\nMonitoring App Launch (Target: {appName})");
            Console.WriteLine("Press 'Q' to stop monitoring.");

            await RunTestLoop(trigger);
        }

        static async Task TestStartup()
        {
            var trigger = new StartupTrigger();
            Console.WriteLine("\nTesting Startup Trigger (Standard logic: Fires once if enabled)");
            bool shouldFire = await trigger.ShouldFireAsync();
            Console.WriteLine($"Initial Status: {trigger.GetDiagnosticInfo()} | ShouldFire: {shouldFire}");
            
            if (shouldFire) trigger.OnFired();
            Console.WriteLine($"After OnFired: {trigger.GetDiagnosticInfo()}");
        }

        static async Task TestShutdown()
        {
            var trigger = new ShutdownTrigger();
            Console.WriteLine("\nTesting Shutdown Trigger");
            bool shouldFire = await trigger.ShouldFireAsync();
            Console.WriteLine($"Status: {trigger.GetDiagnosticInfo()} | ShouldFire: {shouldFire}");
        }

        static async Task TestFileChanged()
        {
            Console.Write("Enter file name or full path (e.g. test.txt): ");
            string input = Console.ReadLine() ?? "";
            string filePath = input;

            if (!System.IO.Path.IsPathRooted(input))
            {
                string downloads = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", input);
                string desktop = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), input);

                if (System.IO.File.Exists(downloads)) filePath = downloads;
                else if (System.IO.File.Exists(desktop)) filePath = desktop;
            }

            string json = "{\"SsidName\": \"" + filePath.Replace("\\", "\\\\") + "\"}";
            var trigger = new FileChangedTrigger();
            trigger.Configure(json);

            Console.WriteLine($"\nMonitoring File Changes");
            Console.WriteLine($"Full Path: {filePath}");
            Console.WriteLine("Press 'Q' to stop monitoring.");
            Console.WriteLine("-------------------------------------------------");

            await RunTestLoop(trigger);
        }

        static async Task RunTestLoop(Core.Interfaces.Logic.ITrigger trigger)
        {
            while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Q)
            {
                bool shouldFire = await trigger.ShouldFireAsync();
                bool hasFired = trigger.HasFired;

                Console.Write($"\r[{DateTime.Now:HH:mm:ss}] ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{trigger.GetDiagnosticInfo()}".PadRight(30));
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" | ShouldFire: ");
                WriteStatus(shouldFire);
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" | HasFired: ");
                WriteStatus(hasFired);
                Console.Write("    ");

                // Emulate the Automation Engine logic
                if (shouldFire && !hasFired) 
                {
                    trigger.OnFired();
                }
                
                Console.ResetColor();
                await Task.Delay(1000);
            }
        }

        static void WriteStatus(bool status)
        {
            Console.ForegroundColor = status ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write(status ? "True " : "False");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
