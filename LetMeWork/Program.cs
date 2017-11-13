using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace LetMeWork
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
#if DEBUG
                args = new[] { "macmnsvc", "macompatsvc", "masvc", "mcshield", "mctray", "mfecanary", "mfeesp", "mfemactl", "mfetp" };
# else
                args = new[] { "mcshield" };
#endif
            }
            Application.Run(new LetMeWork(args));
        }
    }
}