using System;

namespace Ship_Builder
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ShipBuilderEngine game = new ShipBuilderEngine())
            {
                game.Run();
            }
        }
    }
#endif
}

