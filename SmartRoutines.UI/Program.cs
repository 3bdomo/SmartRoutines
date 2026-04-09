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
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            //ApplicationConfiguration.Initialize();

            //// Create the dashboard control
            //var dashboard = new UC_Dashboard();
            //var statcard = new UC_StatCard();
            //var routin = new UC_RoutineCard();

            //// Create a Form to host the control
            //var mainForm = new Form
            //{
            //    Text = "SmartRoutines Dashboard",
            //    StartPosition = FormStartPosition.CenterScreen,
            //    WindowState = FormWindowState.Maximized
            //};

            //dashboard.Dock = DockStyle.Fill;
            //mainForm.Controls.Add(statcard);

            Application.Run(new Form1());
        }
    }
}