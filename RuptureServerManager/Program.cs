using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuptureServerManager
{
    /// <summary>
    /// Entry point for the application. Sets up high DPI mode and launches the main form.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
			if (args.Length > 0 && args[0] == "--update")
			{
				RunUpdater(args);
				return;
			}

			Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

		static void RunUpdater(string[] args)
		{
			while (!Debugger.IsAttached)
			{
				Thread.Sleep(100);
			}
			string targetDir = args[1];
			string sourceDir = args[2];
			int parentPid = int.Parse(args[3]);

			try
			{
				Process.GetProcessById(parentPid).WaitForExit();
			}
			catch { }

			foreach (var src in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
			{
				string relative = Path.GetRelativePath(sourceDir, src);
				string dest = Path.Combine(targetDir, relative);

				Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
				File.Copy(src, dest, true);
			}

			string exe = Directory.GetFiles(targetDir, "*.exe").First();
			Process.Start(new ProcessStartInfo { FileName = exe, UseShellExecute = true });

			Task.Run(() =>
			{
				Thread.Sleep(1500);
				Directory.Delete(Path.GetDirectoryName(sourceDir)!, true);
			});
		}
	}
}