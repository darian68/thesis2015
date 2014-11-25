using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cube
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
            CubeGameWindow gameWindow = new CubeGameWindow("Cube", 1024, 768, false);

            // Start up the game loop.
            gameWindow.StartGameLoop();

        }
    }
}
