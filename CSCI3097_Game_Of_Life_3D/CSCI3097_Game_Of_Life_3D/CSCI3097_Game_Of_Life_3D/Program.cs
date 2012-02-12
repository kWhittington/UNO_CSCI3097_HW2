using System;

namespace CSCI3097_Game_Of_Life_3D
{
#if WINDOWS || XBOX
  static class Program
  {
    //The main entry point for the application
    static void Main(string[] args)
    {
      using (GameOfLife game = new GameOfLife())
      {
        game.Run();
      }
    }
  }
#endif
}

