using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ch03
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
            GameWindow2D gameWindow = new GameWindow2D("Our Direct2D Game Window", 1280, 960, false);

            // Start up the game loop.
            gameWindow.StartGameLoop();

        }
    }
}
