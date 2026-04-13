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

            var Settings = new UC_Settings();
            Settings.Controls.Add(new UC_Settings());

            Application.Run(new Form { Controls = { Settings } });


            //Application.Run(new FrmMain());
        }
    }
}