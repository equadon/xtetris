using System;

namespace XTetris
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (XTetrisGame game = new XTetrisGame())
            {
                game.Run();
            }
        }
    }
#endif
}

