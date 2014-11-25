using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ch01
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


            // Create a GameWindow object.
            SlimFramework.GameWindow gameWindow = new SlimFramework.GameWindow("Our First Game Window", 640, 480, false);

            // Start up the game loop.
            gameWindow.StartGameLoop();

        }
    }
}
