using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ch1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SlimFramework.HarWindow mainWindow = new
            SlimFramework.HarWindow("Our First Game Window", 640, 480,
            false);
            mainWindow.StartMainLoop();
        }
    }
}
