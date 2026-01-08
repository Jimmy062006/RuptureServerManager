using System;
using System.Windows.Forms;

namespace DSApp
{
    /// <summary>
    /// Entry point for the application. Sets up high DPI mode and launches the main form.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}