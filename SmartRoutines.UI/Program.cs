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

            //var mainForm = new Form();
            //mainForm.Controls.Add(new UC_ActionsMain());
            //Application.Run(mainForm);

            Application.Run(new FrmMain());
        }
    }
}