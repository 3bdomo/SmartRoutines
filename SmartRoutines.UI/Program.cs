using SmartRoutines.UI.Controls;
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
            ApplicationConfiguration.Initialize();

            //var dashboard = new UC_Dashboard();
            //dashboard.Controls.Add(new UC_Dashboard());

            //Application.Run(new Form { Controls = { dashboard } });

            Application.Run(new FrmMain());
        }
    }
}