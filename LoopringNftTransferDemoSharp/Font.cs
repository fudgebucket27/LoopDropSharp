using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopringNftTransferDemoSharp
{
    class Font
    {
       public static string SetTextToBlue(string str) { // Set the Foreground color to blue 
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
            return str;
        }


    }
}
