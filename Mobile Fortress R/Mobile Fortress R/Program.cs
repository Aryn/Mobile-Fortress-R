using System;
using Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mobile_Fortress_R
{
#if WINDOWS || XBOX
    static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                using (CoreGame game = new CoreGame())
                {
                    game.Run();
                }
            }
            catch (NoSuitableGraphicsDeviceException)
            {
                MessageBox(new IntPtr(0), "Mobile Fortress requires a GPU compatible with Shader Model 2.0 or higher.", "Graphics Device Error", 0);
            }
        }
    }
#endif
}

